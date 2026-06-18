import { useEffect, useMemo } from 'react';
import { CircleMarker, GeoJSON, MapContainer, TileLayer, Tooltip, useMap } from 'react-leaflet';
import type { GeoJsonObject } from 'geojson';
import L from 'leaflet';
import type { RoutePointResponse } from '../../api/client';
import 'leaflet/dist/leaflet.css';
import './RoutePlanner.css';

interface RouteMapDisplayProps {
  points?: RoutePointResponse[] | null;
  geometry?: string;
  distanceKm?: number;
  durationMinutes?: number;
  fuelLiters?: number;
}

const FitGeometry = ({ geometry }: { geometry: GeoJsonObject }) => {
  const map = useMap();

  useEffect(() => {
    const bounds = L.geoJSON(geometry).getBounds();
    if (bounds.isValid()) {
      map.fitBounds(bounds, { padding: [32, 32] });
    }
  }, [geometry, map]);

  return null;
};

export const RouteMapDisplay = ({
  points = [],
  geometry,
  distanceKm,
  durationMinutes,
  fuelLiters,
}: RouteMapDisplayProps) => {
  const ordered = [...(points ?? [])].sort((a, b) => a.order - b.order);
  const parsedGeometry = useMemo(() => {
    if (!geometry) return null;

    try {
      return JSON.parse(geometry) as GeoJsonObject;
    } catch {
      return null;
    }
  }, [geometry]);

  if (!parsedGeometry || ordered.length < 2) return null;
  const center: [number, number] = [ordered[0].lat, ordered[0].lon];

  return (
    <div className="route-planner">
      <MapContainer center={center} zoom={6} className="route-planner__map" scrollWheelZoom={false}>
        <TileLayer
          attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>'
          url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
        />
        <FitGeometry geometry={parsedGeometry} />
        <GeoJSON data={parsedGeometry} style={{ color: '#6c8cff', weight: 5 }} />
        {ordered.map((point, index) => (
          <CircleMarker
            key={point.id || index}
            center={[point.lat, point.lon]}
            radius={index === 0 || index === ordered.length - 1 ? 9 : 7}
            pathOptions={{ color: index === 0 ? '#4caf50' : index === ordered.length - 1 ? '#f5576c' : '#ffb74d' }}
          >
            <Tooltip permanent direction="top" offset={[0, -8]}>
              {index === 0 ? 'Старт' : index === ordered.length - 1 ? 'Финиш' : index}
            </Tooltip>
          </CircleMarker>
        ))}
      </MapContainer>
      <div className="route-planner__summary">
        {distanceKm !== undefined && <strong>{distanceKm.toLocaleString('ru-RU')} км</strong>}
        {durationMinutes !== undefined && <span>{Math.round(durationMinutes / 60)} ч</span>}
        {fuelLiters !== undefined && <span>{fuelLiters.toLocaleString('ru-RU')} л топлива</span>}
      </div>
      {ordered.length > 2 && (
        <ol>
          {ordered.slice(1, -1).map((point) => <li key={point.id}>{point.address}</li>)}
        </ol>
      )}
    </div>
  );
};
