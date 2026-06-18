import { useEffect, useMemo, useState } from 'react';
import { CircleMarker, GeoJSON, MapContainer, TileLayer, Tooltip, useMap, useMapEvents } from 'react-leaflet';
import type { GeoJsonObject } from 'geojson';
import { api, RouteCalculation } from '../../api/client';
import 'leaflet/dist/leaflet.css';
import './RoutePlanner.css';

export interface EditableRoutePoint {
  address: string;
  lat?: number;
  lon?: number;
}

interface RoutePlannerProps {
  points: EditableRoutePoint[];
  calculation: RouteCalculation | null;
  onPointsChange: (points: EditableRoutePoint[]) => void;
  onCalculationChange: (calculation: RouteCalculation | null) => void;
  fuelConsumption?: number;
}

const defaultCenter: [number, number] = [56.8389, 60.6057];

const FitRoute = ({ points }: { points: EditableRoutePoint[] }) => {
  const map = useMap();

  useEffect(() => {
    const coordinates = points
      .filter((point) => point.lat !== undefined && point.lon !== undefined)
      .map((point) => [point.lat!, point.lon!] as [number, number]);

    if (coordinates.length === 1) map.setView(coordinates[0], 10);
    if (coordinates.length > 1) map.fitBounds(coordinates, { padding: [32, 32] });
  }, [map, points]);

  return null;
};

const MapClick = ({
  onSelect,
}: {
  onSelect: (lat: number, lon: number) => void;
}) => {
  useMapEvents({
    click(event) {
      onSelect(event.latlng.lat, event.latlng.lng);
    },
  });
  return null;
};

export const RoutePlanner = ({
  points,
  calculation,
  onPointsChange,
  onCalculationChange,
  fuelConsumption = 30,
}: RoutePlannerProps) => {
  const [activeIndex, setActiveIndex] = useState(0);
  const [isCalculating, setIsCalculating] = useState(false);
  const [error, setError] = useState('');

  const displayedPoints: EditableRoutePoint[] = calculation?.resolvedPoints?.length
    ? calculation.resolvedPoints.map((point) => ({ address: point.address ?? '', lat: point.lat, lon: point.lon }))
    : points;

  const geoJson = useMemo(() => calculation?.geometry.geoJson ?? null, [calculation]);

  const updatePoint = (index: number, patch: Partial<EditableRoutePoint>) => {
    onPointsChange(points.map((point, pointIndex) => pointIndex === index ? { ...point, ...patch } : point));
    onCalculationChange(null);
  };

  const addIntermediate = () => {
    if (points.length >= 10) return;
    const next = [...points];
    next.splice(next.length - 1, 0, { address: '' });
    onPointsChange(next);
    setActiveIndex(next.length - 2);
    onCalculationChange(null);
  };

  const removeIntermediate = (index: number) => {
    const next = points.filter((_, pointIndex) => pointIndex !== index);
    onPointsChange(next);
    setActiveIndex(Math.max(0, Math.min(activeIndex, next.length - 1)));
    onCalculationChange(null);
  };

  const calculate = async () => {
    setError('');

    if (points.some((point) =>
      !point.address.trim() && (point.lat === undefined || point.lon === undefined)
    )) {
      setError('Укажите адрес или выберите каждую точку на карте.');
      return;
    }

    setIsCalculating(true);
    try {
      const result = await api.routes.calculate({
        points: points.map((point, order) => ({
          address: point.address.trim() || undefined,
          lat: point.lat,
          lon: point.lon,
          order,
        })),
        fuelConsumptionLitersPer100Km: fuelConsumption,
      });

      onPointsChange(result.resolvedPoints.map((point) => ({
        address: point.address ?? '',
        lat: point.lat,
        lon: point.lon,
      })));
      onCalculationChange(result);
    } catch (routeError) {
      setError(routeError instanceof Error ? routeError.message : 'Не удалось рассчитать маршрут');
    } finally {
      setIsCalculating(false);
    }
  };

  return (
    <div className="route-planner">
      <p className="route-planner__hint">
        Введите адрес или выберите строку и поставьте точку кликом по карте.
      </p>

      <div className="route-planner__points">
        {points.map((point, index) => {
          const isEndpoint = index === 0 || index === points.length - 1;
          const label = index === 0 ? 'Старт' : index === points.length - 1 ? 'Финиш' : `Точка ${index}`;
          return (
            <div
              className={`route-planner__point ${activeIndex === index ? 'route-planner__point--active' : ''}`}
              key={`${index}-${points.length}`}
              onClick={() => setActiveIndex(index)}
            >
              <span className="route-planner__badge">{label}</span>
              <input
                className="route-planner__input"
                value={point.address}
                placeholder="Город или адрес"
                onChange={(event) => updatePoint(index, {
                  address: event.target.value,
                  lat: undefined,
                  lon: undefined,
                })}
              />
              <span className="route-planner__coords">
                {point.lat !== undefined ? `${point.lat.toFixed(5)}, ${point.lon!.toFixed(5)}` : 'Выберите на карте'}
              </span>
              {!isEndpoint && (
                <button type="button" className="route-planner__remove" onClick={() => removeIntermediate(index)}>
                  Удалить
                </button>
              )}
            </div>
          );
        })}
      </div>

      <div className="route-planner__actions">
        <button type="button" onClick={addIntermediate} disabled={points.length >= 10}>
          + Промежуточная точка ({Math.max(0, points.length - 2)}/8)
        </button>
        <button type="button" className="route-planner__calculate" onClick={calculate} disabled={isCalculating}>
          {isCalculating ? 'Расчёт...' : 'Рассчитать маршрут'}
        </button>
      </div>

      {error && <div className="route-planner__error">{error}</div>}

      <MapContainer center={defaultCenter} zoom={5} className="route-planner__map">
        <TileLayer
          attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>'
          url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
        />
        <MapClick
          onSelect={(lat, lon) => updatePoint(activeIndex, { address: '', lat, lon })}
        />
        <FitRoute points={displayedPoints} />
        {displayedPoints.map((point, index) =>
          point.lat !== undefined && point.lon !== undefined ? (
            <CircleMarker
              key={`${index}-${point.lat}-${point.lon}`}
              center={[point.lat, point.lon]}
              radius={index === 0 || index === displayedPoints.length - 1 ? 9 : 7}
              pathOptions={{ color: index === 0 ? '#4caf50' : index === displayedPoints.length - 1 ? '#f5576c' : '#ffb74d' }}
            >
              <Tooltip permanent direction="top" offset={[0, -8]}>
                {index === 0 ? 'Старт' : index === displayedPoints.length - 1 ? 'Финиш' : index}
              </Tooltip>
            </CircleMarker>
          ) : null
        )}
        {geoJson && <GeoJSON key={JSON.stringify(geoJson)} data={geoJson as GeoJsonObject} style={{ color: '#6c8cff', weight: 5 }} />}
      </MapContainer>

      {calculation && (
        <div className="route-planner__summary">
          <strong>{calculation.distanceKm.toLocaleString('ru-RU')} км</strong>
          <span>{Math.round(calculation.durationMinutes / 60)} ч</span>
          <span>{calculation.fuelConsumptionLiters.toLocaleString('ru-RU')} л топлива</span>
        </div>
      )}
    </div>
  );
};
