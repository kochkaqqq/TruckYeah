import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { api, LoadingType, PaymentType, ListingVisibility, RouteCalculation } from '../../api/client';
import { useAuthStore } from '../../store/authStore';
import { Toast } from '../../components/ui/Toast';
import { EditableRoutePoint, RoutePlanner } from '../../components/route/RoutePlanner';
import './VehiclesAddPage.css';

type ToastType = 'success' | 'error' | 'info';

interface ToastData {
  message: string;
  type: ToastType;
}

interface FormData {
  title: string;
  description: string;
  routeFrom: string;
  routeTo: string;
  capacityTons: string;
  volumeM3: string;
  bodyType: string;
  loadingType: LoadingType;
  crewDriversCount: string;
  additionalEquipment: string;
  availableFrom: string;
  price: string;
  paymentType: PaymentType;
  allowBargaining: boolean;
  prepaymentPercent: string;
  visibility: ListingVisibility;
}

const initialFormData: FormData = {
  title: '',
  description: '',
  routeFrom: '',
  routeTo: '',
  capacityTons: '',
  volumeM3: '',
  bodyType: '',
  loadingType: LoadingType.Rear,
  crewDriversCount: '1',
  additionalEquipment: '',
  availableFrom: '',
  price: '',
  paymentType: PaymentType.Cash,
  allowBargaining: false,
  prepaymentPercent: '',
  visibility: ListingVisibility.Exchange,
};

export const VehiclesAddPage = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const isEditMode = Boolean(id);
  const { isAuthenticated } = useAuthStore();
  const [formData, setFormData] = useState<FormData>(initialFormData);
  const [isLoading, setIsLoading] = useState(isEditMode);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [toast, setToast] = useState<ToastData | null>(null);
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [routePoints, setRoutePoints] = useState<EditableRoutePoint[]>([
    { address: '' },
    { address: '' },
  ]);
  const [routeCalculation, setRouteCalculation] = useState<RouteCalculation | null>(null);

  useEffect(() => {
    if (!id) return;

    const loadTruck = async () => {
      try {
        const truck = await api.trucks.getById(id);
        setFormData({
          title: truck.title ?? '',
          description: truck.description ?? '',
          routeFrom: truck.routeFrom ?? '',
          routeTo: truck.routeTo ?? '',
          capacityTons: truck.capacityTons?.toString() ?? '',
          volumeM3: truck.volumeM3?.toString() ?? '',
          bodyType: truck.bodyType ?? '',
          loadingType: truck.loadingType,
          crewDriversCount: truck.crewDriversCount?.toString() ?? '1',
          additionalEquipment: truck.additionalEquipment ?? '',
          availableFrom: new Date(truck.availableFrom).toISOString().slice(0, 16),
          price: truck.price?.toString() ?? '',
          paymentType: truck.paymentType,
          allowBargaining: truck.allowBargaining ?? false,
          prepaymentPercent: truck.prepaymentPercent?.toString() ?? '',
          visibility: truck.visibility ?? ListingVisibility.Exchange,
        });
        setRoutePoints(
          truck.routePoints?.length
            ? truck.routePoints.map((point) => ({ address: point.address ?? '', lat: point.lat, lon: point.lon }))
            : [{ address: truck.routeFrom ?? '' }, { address: truck.routeTo ?? '' }]
        );
        if (truck.routeGeometryGeoJson) {
          setRouteCalculation({
            distanceKm: truck.routeDistanceKm ?? 0,
            durationMinutes: truck.routeDurationMinutes ?? 0,
            fuelConsumptionLiters: truck.routeFuelLiters ?? 0,
            resolvedPoints: truck.routePoints ?? [],
            geometry: { geoJson: JSON.parse(truck.routeGeometryGeoJson) },
            warnings: [],
          });
        }
      } catch (error) {
        setToast({
          message: error instanceof Error ? error.message : 'Не удалось загрузить машину',
          type: 'error',
        });
      } finally {
        setIsLoading(false);
      }
    };

    void loadTruck();
  }, [id]);

  if (!isAuthenticated) {
    navigate('/auth');
    return null;
  }

  if (isLoading) {
    return <div className="vehicle-add"><div className="vehicle-add__container">Загрузка объявления...</div></div>;
  }

  const showToast = (message: string, type: ToastType) => {
    setToast({ message, type });
  };

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>
  ) => {
    const { name, value, type } = e.target;
    const checked = (e.target as HTMLInputElement).checked;

    setFormData((prev) => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : value,
    }));

    if (errors[name]) {
      setErrors((prev) => {
        const newErrors = { ...prev };
        delete newErrors[name];
        return newErrors;
      });
    }
  };

  const validate = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (!formData.routeFrom.trim()) {
      newErrors.routeFrom = 'Укажите точку отправления';
    }

    if (!formData.routeTo.trim()) {
      newErrors.routeTo = 'Укажите точку назначения';
    }
    if (!routeCalculation?.geometry.geoJson) {
      newErrors.route = 'Рассчитайте маршрут перед сохранением';
    }

    const capacity = parseFloat(formData.capacityTons);
    if (!formData.capacityTons || isNaN(capacity) || capacity <= 0) {
      newErrors.capacityTons = 'Укажите грузоподъёмность больше 0';
    }

    const volume = parseFloat(formData.volumeM3);
    if (!formData.volumeM3 || isNaN(volume) || volume <= 0) {
      newErrors.volumeM3 = 'Укажите объём больше 0';
    }

    if (!formData.bodyType.trim()) {
      newErrors.bodyType = 'Укажите тип кузова';
    }

    if (!formData.availableFrom) {
      newErrors.availableFrom = 'Укажите дату доступности';
    }

    const price = parseFloat(formData.price);
    if (!formData.price || isNaN(price) || price < 0) {
      newErrors.price = 'Укажите корректную цену';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const buildPayload = (visibility: ListingVisibility) => {
    const loadingTypeMap: Record<LoadingType, number> = {
      [LoadingType.Rear]: 0,
      [LoadingType.Side]: 1,
      [LoadingType.Top]: 2,
      [LoadingType.FullAccess]: 3,
    };

    const paymentTypeMap: Record<PaymentType, number> = {
      [PaymentType.Cash]: 0,
      [PaymentType.WithVAT]: 1,
      [PaymentType.WithoutVAT]: 2,
    };

    const visibilityMap: Record<ListingVisibility, number> = {
      [ListingVisibility.Exchange]: 0,
      [ListingVisibility.Private]: 1,
    };

    const payload: any = {
      routeFrom: formData.routeFrom.trim(),
      routeTo: formData.routeTo.trim(),
      routePoints: routePoints.map((point, order) => ({
        address: point.address,
        lat: point.lat,
        lon: point.lon,
        order,
      })),
      routeDistanceKm: routeCalculation?.distanceKm,
      routeDurationMinutes: routeCalculation?.durationMinutes,
      routeFuelLiters: routeCalculation?.fuelConsumptionLiters,
      routeGeometryGeoJson: routeCalculation?.geometry.geoJson
        ? JSON.stringify(routeCalculation.geometry.geoJson)
        : undefined,
      routeCalculatedAt: routeCalculation ? new Date().toISOString() : undefined,
      capacityTons: parseFloat(formData.capacityTons),
      volumeM3: parseFloat(formData.volumeM3),
      bodyType: formData.bodyType.trim(),
      loadingType: loadingTypeMap[formData.loadingType] ?? 0,
      availableFrom: new Date(formData.availableFrom).toISOString(),
      price: parseFloat(formData.price),
      paymentType: paymentTypeMap[formData.paymentType] ?? 0,
      visibility: visibilityMap[visibility] ?? 0,
      crewDriversCount: parseInt(formData.crewDriversCount) || 1,
      allowBargaining: formData.allowBargaining,
    };

    if (formData.title.trim()) payload.title = formData.title.trim();
    if (formData.description.trim()) payload.description = formData.description.trim();
    if (formData.additionalEquipment.trim()) {
      payload.additionalEquipment = formData.additionalEquipment.trim();
    }
    if (formData.prepaymentPercent) {
      payload.prepaymentPercent = parseFloat(formData.prepaymentPercent);
    }

    return payload;
  };

  const handleSubmit = async (asDraft: boolean) => {
    if (!validate()) {
      showToast('Проверьте правильность заполнения полей', 'error');
      const firstError = document.querySelector('.vehicle-add__input--error');
      if (firstError) {
        firstError.scrollIntoView({ behavior: 'smooth', block: 'center' });
      }
      return;
    }

    setIsSubmitting(true);
    try {
      const payload = buildPayload(
        asDraft ? ListingVisibility.Private : formData.visibility
      );

      console.log('🚛 Отправляемый payload:', payload);

      const truckId = id
        ? await api.trucks.update(id, payload)
        : await api.trucks.create(payload);
      console.log('✅ Машина создана, ID:', truckId);

      if (!asDraft) {
        try {
          await api.trucks.publish(truckId);
          console.log('✅ Машина опубликована');
        } catch (publishError) {
          console.error('⚠️ Ошибка публикации:', publishError);
          showToast('Машина создана, но не удалось опубликовать.', 'error');
          setTimeout(() => {
            navigate(`/vehicles/${truckId}`);
          }, 1500);
          return;
        }
      }

      showToast(
        isEditMode
          ? 'Изменения сохранены'
          : asDraft
            ? 'Черновик сохранён'
            : 'Машина опубликована',
        'success'
      );

      setTimeout(() => {
        navigate(`/vehicles/${truckId}`);
      }, 1000);
    } catch (error: any) {
      console.error('❌ Ошибка создания машины:', error);
      showToast(error.message || 'Ошибка при создании машины', 'error');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleCancel = () => {
    navigate(isEditMode ? '/my-listing' : '/vehicles');
  };

  const getInputClass = (fieldName: string) => {
    return `vehicle-add__input ${errors[fieldName] ? 'vehicle-add__input--error' : ''}`;
  };

  return (
    <div className="vehicle-add">
      {toast && (
        <Toast message={toast.message} type={toast.type} onClose={() => setToast(null)} />
      )}

      <div className="vehicle-add__container">
        <div className="vehicle-add__header">
          <h1 className="vehicle-add__title">
            {isEditMode ? 'Редактировать машину' : 'Добавить новую машину'}
          </h1>
          <button className="vehicle-add__back-btn" onClick={handleCancel}>
            ← Назад к списку
          </button>
        </div>

        <form className="vehicle-add__form" onSubmit={(e) => e.preventDefault()}>
          <section className="vehicle-add__section">
            <h2 className="vehicle-add__section-title">Основная информация</h2>
            <div className="vehicle-add__grid">
              <div className="vehicle-add__field vehicle-add__field--full">
                <label className="vehicle-add__label">Заголовок объявления (опционально)</label>
                <input
                  type="text"
                  name="title"
                  className="vehicle-add__input"
                  placeholder="Краткое описание для привлечения внимания"
                  value={formData.title}
                  onChange={handleChange}
                />
              </div>

              <div className="vehicle-add__field vehicle-add__field--full">
                <label className="vehicle-add__label">Описание (опционально)</label>
                <textarea
                  name="description"
                  className="vehicle-add__input vehicle-add__textarea"
                  placeholder="Дополнительная информация о машине"
                  value={formData.description}
                  onChange={handleChange}
                  rows={3}
                />
              </div>
            </div>
          </section>

          <section className="vehicle-add__section">
            <h2 className="vehicle-add__section-title">Маршрут</h2>
            <RoutePlanner
              points={routePoints}
              calculation={routeCalculation}
              onPointsChange={(points) => {
                setRoutePoints(points);
                setFormData((current) => ({
                  ...current,
                  routeFrom: points[0]?.address ?? '',
                  routeTo: points[points.length - 1]?.address ?? '',
                }));
              }}
              onCalculationChange={setRouteCalculation}
            />
            {errors.route && <span className="vehicle-add__error">{errors.route}</span>}
            <div className="vehicle-add__grid vehicle-add__grid--2">
              <div className="vehicle-add__field">
                <label className="vehicle-add__label">
                  Дата доступности <span className="vehicle-add__required">*</span>
                </label>
                <input
                  type="datetime-local"
                  name="availableFrom"
                  className={getInputClass('availableFrom')}
                  value={formData.availableFrom}
                  onChange={handleChange}
                />
                {errors.availableFrom && <span className="vehicle-add__error">{errors.availableFrom}</span>}
              </div>
            </div>
          </section>

          <section className="vehicle-add__section">
            <h2 className="vehicle-add__section-title">Параметры машины</h2>
            <div className="vehicle-add__grid vehicle-add__grid--4">
              <div className="vehicle-add__field">
                <label className="vehicle-add__label">
                  Грузоподъёмность (т) <span className="vehicle-add__required">*</span>
                </label>
                <input
                  type="number"
                  name="capacityTons"
                  className={getInputClass('capacityTons')}
                  placeholder="0.0"
                  step="0.1"
                  min="0"
                  value={formData.capacityTons}
                  onChange={handleChange}
                />
                {errors.capacityTons && <span className="vehicle-add__error">{errors.capacityTons}</span>}
              </div>

              <div className="vehicle-add__field">
                <label className="vehicle-add__label">
                  Объём кузова (м³) <span className="vehicle-add__required">*</span>
                </label>
                <input
                  type="number"
                  name="volumeM3"
                  className={getInputClass('volumeM3')}
                  placeholder="0.0"
                  step="0.1"
                  min="0"
                  value={formData.volumeM3}
                  onChange={handleChange}
                />
                {errors.volumeM3 && <span className="vehicle-add__error">{errors.volumeM3}</span>}
              </div>

              <div className="vehicle-add__field">
                <label className="vehicle-add__label">
                  Тип кузова <span className="vehicle-add__required">*</span>
                </label>
                <input
                  type="text"
                  name="bodyType"
                  className={getInputClass('bodyType')}
                  placeholder="Тент, рефрижератор, изотерм..."
                  value={formData.bodyType}
                  onChange={handleChange}
                />
                {errors.bodyType && <span className="vehicle-add__error">{errors.bodyType}</span>}
              </div>

              <div className="vehicle-add__field">
                <label className="vehicle-add__label">
                  Тип загрузки <span className="vehicle-add__required">*</span>
                </label>
                <select
                  name="loadingType"
                  className="vehicle-add__input vehicle-add__select"
                  value={formData.loadingType}
                  onChange={handleChange}
                >
                  <option value="Rear">Задняя</option>
                  <option value="Side">Боковая</option>
                  <option value="Top">Верхняя</option>
                  <option value="FullAccess">Полный доступ</option>
                </select>
              </div>

              <div className="vehicle-add__field">
                <label className="vehicle-add__label">Количество водителей</label>
                <input
                  type="number"
                  name="crewDriversCount"
                  className="vehicle-add__input"
                  placeholder="1"
                  min="1"
                  max="3"
                  value={formData.crewDriversCount}
                  onChange={handleChange}
                />
              </div>

              <div className="vehicle-add__field vehicle-add__field--full">
                <label className="vehicle-add__label">Дополнительное оборудование</label>
                <input
                  type="text"
                  name="additionalEquipment"
                  className="vehicle-add__input"
                  placeholder="GPS, кондиционер, гидроборт..."
                  value={formData.additionalEquipment}
                  onChange={handleChange}
                />
              </div>
            </div>
          </section>

          <section className="vehicle-add__section">
            <h2 className="vehicle-add__section-title">Финансы</h2>
            <div className="vehicle-add__grid vehicle-add__grid--2">
              <div className="vehicle-add__field">
                <label className="vehicle-add__label">
                  Цена (€) <span className="vehicle-add__required">*</span>
                </label>
                <input
                  type="number"
                  name="price"
                  className={getInputClass('price')}
                  placeholder="0"
                  step="100"
                  min="0"
                  value={formData.price}
                  onChange={handleChange}
                />
                {errors.price && <span className="vehicle-add__error">{errors.price}</span>}
              </div>

              <div className="vehicle-add__field">
                <label className="vehicle-add__label">
                  Тип оплаты <span className="vehicle-add__required">*</span>
                </label>
                <select
                  name="paymentType"
                  className="vehicle-add__input vehicle-add__select"
                  value={formData.paymentType}
                  onChange={handleChange}
                >
                  <option value="Cash">Наличные</option>
                  <option value="WithVAT">С НДС</option>
                  <option value="WithoutVAT">Без НДС</option>
                </select>
              </div>

              <div className="vehicle-add__field">
                <label className="vehicle-add__label">Предоплата (%)</label>
                <input
                  type="number"
                  name="prepaymentPercent"
                  className="vehicle-add__input"
                  placeholder="0-100"
                  min="0"
                  max="100"
                  value={formData.prepaymentPercent}
                  onChange={handleChange}
                />
              </div>
            </div>

            <div className="vehicle-add__checkboxes">
              <label className="vehicle-add__checkbox">
                <input
                  type="checkbox"
                  name="allowBargaining"
                  checked={formData.allowBargaining}
                  onChange={handleChange}
                />
                <span>Разрешить торг</span>
              </label>
            </div>
          </section>

          <section className="vehicle-add__section">
            <h2 className="vehicle-add__section-title">Настройки</h2>
            <div className="vehicle-add__grid vehicle-add__grid--2">
              <div className="vehicle-add__field">
                <label className="vehicle-add__label">Видимость</label>
                <select
                  name="visibility"
                  className="vehicle-add__input vehicle-add__select"
                  value={formData.visibility}
                  onChange={handleChange}
                >
                  <option value="Exchange">Биржа (публично)</option>
                  <option value="Private">Приватно</option>
                </select>
              </div>
            </div>
          </section>

          <div className="vehicle-add__actions">
            <button
              type="button"
              className="vehicle-add__btn vehicle-add__btn--secondary"
              onClick={handleCancel}
              disabled={isSubmitting}
            >
              Отмена
            </button>
            <button
              type="button"
              className="vehicle-add__btn vehicle-add__btn--draft"
              onClick={() => handleSubmit(true)}
              disabled={isSubmitting}
            >
              {isSubmitting ? 'Сохранение...' : isEditMode ? 'Сохранить' : 'Сохранить черновик'}
            </button>
            <button
              type="button"
              className="vehicle-add__btn vehicle-add__btn--primary"
              onClick={() => handleSubmit(false)}
              disabled={isSubmitting}
            >
              {isSubmitting ? 'Публикация...' : isEditMode ? 'Сохранить и опубликовать' : 'Опубликовать'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};
