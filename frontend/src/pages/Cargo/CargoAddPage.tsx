import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { api, LoadingType, PaymentType, ListingVisibility, RouteCalculation } from '../../api/client';
import { useAuthStore } from '../../store/authStore';
import { Toast } from '../../components/ui/Toast';
import { EditableRoutePoint, RoutePlanner } from '../../components/route/RoutePlanner';
import './CargoAddPage.css';

type ToastType = 'success' | 'error' | 'info';

interface ToastData {
  message: string;
  type: ToastType;
}

interface FormData {
  calculationMode: 'manual' | 'automatic';
  cargoName: string;
  title: string;
  notes: string;
  routeFrom: string;
  routeTo: string;
  loadDateTime: string;
  unloadDateTime: string;
  weightTons: string;
  volumeM3: string;
  bodyTypeRequired: string;
  loadingType: LoadingType;
  lengthCm: string;
  widthCm: string;
  heightCm: string;
  palletsCount: string;
  weightPerPackageKg: string;
  packagingType: string;
  startingPrice: string;
  paymentType: PaymentType;
  allowBargaining: boolean;
  prepaymentPercent: string;
  biddingEnabled: boolean;
  minBidStep: string;
  visibility: ListingVisibility;
  requiresCMR: boolean;
  requiresTIR: boolean;
  isADR: boolean;
  requiresTwoDrivers: boolean;
}

const initialFormData: FormData = {
  calculationMode: 'manual',
  cargoName: '',
  title: '',
  notes: '',
  routeFrom: '',
  routeTo: '',
  loadDateTime: '',
  unloadDateTime: '',
  weightTons: '',
  volumeM3: '',
  bodyTypeRequired: '',
  loadingType: LoadingType.Rear,
  lengthCm: '',
  widthCm: '',
  heightCm: '',
  palletsCount: '',
  weightPerPackageKg: '',
  packagingType: '',
  startingPrice: '',
  paymentType: PaymentType.Cash,
  allowBargaining: true,       // ✅ Включено по умолчанию
  prepaymentPercent: '',
  biddingEnabled: true,        // ✅ Включено по умолчанию
  minBidStep: '1000',          // ✅ Шаг по умолчанию
  visibility: ListingVisibility.Exchange,
  requiresCMR: false,
  requiresTIR: false,
  isADR: false,
  requiresTwoDrivers: false,
};

export const CargoAddPage = () => {
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

    const loadCargo = async () => {
      try {
        const cargo = await api.cargos.getById(id);
        const toLocalDateTime = (value?: string | null) =>
          value ? new Date(value).toISOString().slice(0, 16) : '';

        setFormData({
          calculationMode: cargo.useAutomaticCalculation ? 'automatic' : 'manual',
          cargoName: cargo.cargoName ?? '',
          title: cargo.title ?? '',
          notes: cargo.notes ?? '',
          routeFrom: cargo.routeFrom ?? '',
          routeTo: cargo.routeTo ?? '',
          loadDateTime: toLocalDateTime(cargo.loadDateTime),
          unloadDateTime: toLocalDateTime(cargo.unloadDateTime),
          weightTons: cargo.weightTons?.toString() ?? '',
          volumeM3: cargo.volumeM3?.toString() ?? '',
          bodyTypeRequired: cargo.bodyTypeRequired ?? '',
          loadingType: cargo.loadingType,
          lengthCm: cargo.lengthCm?.toString() ?? '',
          widthCm: cargo.widthCm?.toString() ?? '',
          heightCm: cargo.heightCm?.toString() ?? '',
          palletsCount: cargo.palletsCount?.toString() ?? '',
          weightPerPackageKg: cargo.weightPerPackageKg?.toString() ?? '',
          packagingType: cargo.packagingType ?? '',
          startingPrice: cargo.startingPrice?.toString() ?? '',
          paymentType: cargo.paymentType,
          allowBargaining: cargo.allowBargaining ?? false,
          prepaymentPercent: cargo.prepaymentPercent?.toString() ?? '',
          biddingEnabled: cargo.biddingEnabled ?? false,
          minBidStep: cargo.minBidStep?.toString() ?? '',
          visibility: cargo.visibility ?? ListingVisibility.Exchange,
          requiresCMR: cargo.requiresCMR ?? false,
          requiresTIR: cargo.requiresTIR ?? false,
          isADR: cargo.isADR ?? false,
          requiresTwoDrivers: cargo.requiresTwoDrivers ?? false,
        });
        setRoutePoints(
          cargo.routePoints?.length
            ? cargo.routePoints.map((point) => ({ address: point.address ?? '', lat: point.lat, lon: point.lon }))
            : [{ address: cargo.routeFrom ?? '' }, { address: cargo.routeTo ?? '' }]
        );
        if (cargo.routeGeometryGeoJson) {
          setRouteCalculation({
            distanceKm: cargo.routeDistanceKm ?? 0,
            durationMinutes: cargo.routeDurationMinutes ?? 0,
            fuelConsumptionLiters: cargo.routeFuelLiters ?? 0,
            resolvedPoints: cargo.routePoints ?? [],
            geometry: { geoJson: JSON.parse(cargo.routeGeometryGeoJson) },
            warnings: [],
          });
        }
      } catch (error) {
        setToast({
          message: error instanceof Error ? error.message : 'Не удалось загрузить груз',
          type: 'error',
        });
      } finally {
        setIsLoading(false);
      }
    };

    void loadCargo();
  }, [id]);

  if (!isAuthenticated) {
    navigate('/auth');
    return null;
  }

  if (isLoading) {
    return <div className="cargo-add"><div className="cargo-add__container">Загрузка объявления...</div></div>;
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

  const calculateAutomaticValues = () => {
    const length = Number(formData.lengthCm);
    const width = Number(formData.widthCm);
    const height = Number(formData.heightCm);
    const packages = Number(formData.palletsCount);
    const weightPerPackage = Number(formData.weightPerPackageKg);

    if ([length, width, height, packages, weightPerPackage].some((value) => value <= 0)) {
      return { weightTons: 0, volumeM3: 0 };
    }

    return {
      weightTons: Math.round((weightPerPackage * packages / 1000) * 1000) / 1000,
      volumeM3: Math.round((length * width * height * packages / 1_000_000) * 1000) / 1000,
    };
  };

  const automaticValues = calculateAutomaticValues();

  const validate = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (!formData.title.trim()) {
      newErrors.title = 'Укажите заголовок объявления';
    }

    if (!formData.cargoName.trim()) {
      newErrors.cargoName = 'Укажите наименование груза';
    }

    if (!formData.routeFrom.trim()) {
      newErrors.routeFrom = 'Укажите точку отправления';
    }

    if (!formData.routeTo.trim()) {
      newErrors.routeTo = 'Укажите точку назначения';
    }

    if (!formData.loadDateTime) {
      newErrors.loadDateTime = 'Укажите дату погрузки';
    }

    if (!formData.unloadDateTime) {
      newErrors.unloadDateTime = 'Укажите дату выгрузки';
    }

    if (formData.loadDateTime && formData.unloadDateTime) {
      const load = new Date(formData.loadDateTime);
      const unload = new Date(formData.unloadDateTime);
      if (unload < load) {
        newErrors.unloadDateTime = 'Дата выгрузки не может быть раньше даты погрузки';
      }
    }

    const weight = formData.calculationMode === 'automatic'
      ? automaticValues.weightTons
      : parseFloat(formData.weightTons);
    if ((formData.calculationMode === 'manual' && !formData.weightTons) || isNaN(weight) || weight <= 0) {
      newErrors.weightTons = 'Укажите вес больше 0';
    }

    const volume = formData.calculationMode === 'automatic'
      ? automaticValues.volumeM3
      : parseFloat(formData.volumeM3);
    if ((formData.calculationMode === 'manual' && !formData.volumeM3) || isNaN(volume) || volume <= 0) {
      newErrors.volumeM3 = 'Укажите объём больше 0';
    }
    if (!routeCalculation?.geometry.geoJson) {
      newErrors.route = 'Рассчитайте маршрут перед сохранением';
    }

    if (formData.calculationMode === 'automatic') {
      if (Number(formData.lengthCm) <= 0) newErrors.lengthCm = 'Укажите длину';
      if (Number(formData.widthCm) <= 0) newErrors.widthCm = 'Укажите ширину';
      if (Number(formData.heightCm) <= 0) newErrors.heightCm = 'Укажите высоту';
      if (Number(formData.palletsCount) <= 0) newErrors.palletsCount = 'Укажите количество мест';
      if (Number(formData.weightPerPackageKg) <= 0) {
        newErrors.weightPerPackageKg = 'Укажите вес одного места';
      }
    }

    const price = parseFloat(formData.startingPrice);
    if (!formData.startingPrice || isNaN(price) || price < 0) {
      newErrors.startingPrice = 'Укажите корректную цену';
    }

    if (formData.biddingEnabled && formData.minBidStep) {
      const step = parseFloat(formData.minBidStep);
      if (isNaN(step) || step <= 0) {
        newErrors.minBidStep = 'Минимальный шаг должен быть больше 0';
      }
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
      title: formData.title.trim(),
      cargoName: formData.cargoName.trim(),
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
      loadDateTime: new Date(formData.loadDateTime).toISOString(),
      unloadDateTime: new Date(formData.unloadDateTime).toISOString(),
      weightTons: parseFloat(formData.weightTons),
      volumeM3: parseFloat(formData.volumeM3),
      useAutomaticCalculation: formData.calculationMode === 'automatic',
      loadingType: loadingTypeMap[formData.loadingType] ?? 0,
      paymentType: paymentTypeMap[formData.paymentType] ?? 0,
      startingPrice: parseFloat(formData.startingPrice),
      visibility: visibilityMap[visibility] ?? 0,
      allowBargaining: formData.allowBargaining,
      biddingEnabled: formData.biddingEnabled,
      requiresCMR: formData.requiresCMR,
      requiresTIR: formData.requiresTIR,
      isADR: formData.isADR,
      requiresTwoDrivers: formData.requiresTwoDrivers,
    };

    if (formData.notes.trim()) payload.notes = formData.notes.trim();
    if (formData.bodyTypeRequired.trim()) payload.bodyTypeRequired = formData.bodyTypeRequired.trim();
    if (formData.lengthCm) payload.lengthCm = parseFloat(formData.lengthCm);
    if (formData.widthCm) payload.widthCm = parseFloat(formData.widthCm);
    if (formData.heightCm) payload.heightCm = parseFloat(formData.heightCm);
    if (formData.palletsCount) payload.palletsCount = parseInt(formData.palletsCount);
    if (formData.weightPerPackageKg) {
      payload.weightPerPackageKg = parseFloat(formData.weightPerPackageKg);
    }
    if (formData.calculationMode === 'automatic') {
      payload.weightTons = automaticValues.weightTons;
      payload.volumeM3 = automaticValues.volumeM3;
    }
    if (formData.packagingType.trim()) payload.packagingType = formData.packagingType.trim();
    if (formData.prepaymentPercent) payload.prepaymentPercent = parseFloat(formData.prepaymentPercent);
    if (formData.biddingEnabled && formData.minBidStep) {
      payload.minBidStep = parseFloat(formData.minBidStep);
    }

    return payload;
  };

  const handleSubmit = async (asDraft: boolean) => {
  if (!validate()) {
    showToast('Проверьте правильность заполнения полей', 'error');
    const firstError = document.querySelector('.cargo-add__input--error');
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

    console.log(' Отправляемый payload:', payload);

    const cargoId = id
      ? await api.cargos.update(id, payload)
      : await api.cargos.create(payload);
    console.log('✅ Груз создан, ID:', cargoId);

    // 2. Если это не черновик — публикуем
    if (!asDraft) {
      try {
        await api.cargos.publish(cargoId);
        console.log('✅ Груз опубликован');
      } catch (publishError) {
        console.error('⚠️ Ошибка публикации (груз создан как черновик):', publishError);
        showToast('Груз создан, но не удалось опубликовать. Проверьте в "Мои грузы".', 'error');
        setTimeout(() => {
          navigate(`/cargo/${cargoId}`);
        }, 1500);
        return;
      }
    }

    showToast(
      isEditMode
        ? 'Изменения сохранены'
        : asDraft
          ? 'Черновик сохранён'
          : 'Груз опубликован',
      'success'
    );

    setTimeout(() => {
      navigate(`/cargo/${cargoId}`);
    }, 1000);
  } catch (error: any) {
    console.error('❌ Ошибка создания груза:', error);
    showToast(error.message || 'Ошибка при создании груза', 'error');
  } finally {
    setIsSubmitting(false);
  }
};

  const handleCancel = () => {
    navigate(isEditMode ? '/my-listing' : '/cargo');
  };

  const getInputClass = (fieldName: string) => {
    return `cargo-add__input ${errors[fieldName] ? 'cargo-add__input--error' : ''}`;
  };

  return (
    <div className="cargo-add">
      {toast && (
        <Toast message={toast.message} type={toast.type} onClose={() => setToast(null)} />
      )}

      <div className="cargo-add__container">
        <div className="cargo-add__header">
          <h1 className="cargo-add__title">
            {isEditMode ? 'Редактировать груз' : 'Добавить новый груз'}
          </h1>
          <button className="cargo-add__back-btn" onClick={handleCancel}>
            ← Назад к списку
          </button>
        </div>

        <form className="cargo-add__form" onSubmit={(e) => e.preventDefault()}>
          {/* Секция 1: Основная информация */}
          <section className="cargo-add__section">
            <h2 className="cargo-add__section-title">Основная информация</h2>
            <div className="cargo-add__grid">
              <div className="cargo-add__field cargo-add__field--full">
                <label className="cargo-add__label">
                  Заголовок объявления <span className="cargo-add__required">*</span>
                </label>
                <input
                  type="text"
                  name="title"
                  className={getInputClass('title')}
                  placeholder="Краткое описание для привлечения внимания"
                  value={formData.title}
                  onChange={handleChange}
                />
                {errors.title && <span className="cargo-add__error">{errors.title}</span>}
              </div>

              <div className="cargo-add__field cargo-add__field--full">
                <label className="cargo-add__label">
                  Наименование груза <span className="cargo-add__required">*</span>
                </label>
                <input
                  type="text"
                  name="cargoName"
                  className={getInputClass('cargoName')}
                  placeholder="Например: Электроника, продукты питания"
                  value={formData.cargoName}
                  onChange={handleChange}
                />
                {errors.cargoName && <span className="cargo-add__error">{errors.cargoName}</span>}
              </div>

              <div className="cargo-add__field cargo-add__field--full">
                <label className="cargo-add__label">Заметки (опционально)</label>
                <textarea
                  name="notes"
                  className="cargo-add__input cargo-add__textarea"
                  placeholder="Дополнительная информация о грузе"
                  value={formData.notes}
                  onChange={handleChange}
                  rows={3}
                />
              </div>
            </div>
          </section>

          {/* Секция 2: Маршрут */}
          <section className="cargo-add__section">
            <h2 className="cargo-add__section-title">Маршрут</h2>
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
            {errors.route && <span className="cargo-add__error">{errors.route}</span>}
            <div className="cargo-add__grid cargo-add__grid--2">
              <div className="cargo-add__field">
                <label className="cargo-add__label">
                  Дата погрузки <span className="cargo-add__required">*</span>
                </label>
                <input
                  type="datetime-local"
                  name="loadDateTime"
                  className={getInputClass('loadDateTime')}
                  value={formData.loadDateTime}
                  onChange={handleChange}
                />
                {errors.loadDateTime && <span className="cargo-add__error">{errors.loadDateTime}</span>}
              </div>

              <div className="cargo-add__field">
                <label className="cargo-add__label">
                  Дата выгрузки <span className="cargo-add__required">*</span>
                </label>
                <input
                  type="datetime-local"
                  name="unloadDateTime"
                  className={getInputClass('unloadDateTime')}
                  value={formData.unloadDateTime}
                  onChange={handleChange}
                />
                {errors.unloadDateTime && <span className="cargo-add__error">{errors.unloadDateTime}</span>}
              </div>
            </div>
          </section>

          {/* Секция 3: Параметры груза */}
          <section className="cargo-add__section">
            <h2 className="cargo-add__section-title">Параметры груза</h2>
            <div className="cargo-add__field">
              <label className="cargo-add__label">Режим расчёта</label>
              <select
                name="calculationMode"
                className="cargo-add__input cargo-add__select"
                value={formData.calculationMode}
                onChange={handleChange}
              >
                <option value="manual">Вручную</option>
                <option value="automatic">Автоматически по грузовым местам</option>
              </select>
            </div>
            <div className="cargo-add__grid cargo-add__grid--4">
              <div className="cargo-add__field">
                <label className="cargo-add__label">
                  Вес (т) <span className="cargo-add__required">*</span>
                </label>
                <input
                  type="number"
                  name="weightTons"
                  className={getInputClass('weightTons')}
                  placeholder="0.0"
                  step="0.1"
                  min="0"
                  value={formData.calculationMode === 'automatic' ? automaticValues.weightTons || '' : formData.weightTons}
                  onChange={handleChange}
                  readOnly={formData.calculationMode === 'automatic'}
                />
                {errors.weightTons && <span className="cargo-add__error">{errors.weightTons}</span>}
              </div>

              <div className="cargo-add__field">
                <label className="cargo-add__label">
                  Объём (м³) <span className="cargo-add__required">*</span>
                </label>
                <input
                  type="number"
                  name="volumeM3"
                  className={getInputClass('volumeM3')}
                  placeholder="0.0"
                  step="0.1"
                  min="0"
                  value={formData.calculationMode === 'automatic' ? automaticValues.volumeM3 || '' : formData.volumeM3}
                  onChange={handleChange}
                  readOnly={formData.calculationMode === 'automatic'}
                />
                {errors.volumeM3 && <span className="cargo-add__error">{errors.volumeM3}</span>}
              </div>

              <div className="cargo-add__field">
                <label className="cargo-add__label">Тип кузова</label>
                <input
                  type="text"
                  name="bodyTypeRequired"
                  className="cargo-add__input"
                  placeholder="Тент, рефрижератор..."
                  value={formData.bodyTypeRequired}
                  onChange={handleChange}
                />
              </div>

              <div className="cargo-add__field">
                <label className="cargo-add__label">
                  Тип загрузки <span className="cargo-add__required">*</span>
                </label>
                <select
                  name="loadingType"
                  className="cargo-add__input cargo-add__select"
                  value={formData.loadingType}
                  onChange={handleChange}
                >
                  <option value="Rear">Задняя</option>
                  <option value="Side">Боковая</option>
                  <option value="Top">Верхняя</option>
                  <option value="FullAccess">Полный доступ</option>
                </select>
              </div>

              <div className="cargo-add__field">
                <label className="cargo-add__label">Длина (см)</label>
                <input
                  type="number"
                  name="lengthCm"
                  className="cargo-add__input"
                  placeholder="0"
                  value={formData.lengthCm}
                  onChange={handleChange}
                />
              </div>

              <div className="cargo-add__field">
                <label className="cargo-add__label">Ширина (см)</label>
                <input
                  type="number"
                  name="widthCm"
                  className="cargo-add__input"
                  placeholder="0"
                  value={formData.widthCm}
                  onChange={handleChange}
                />
              </div>

              <div className="cargo-add__field">
                <label className="cargo-add__label">Высота (см)</label>
                <input
                  type="number"
                  name="heightCm"
                  className="cargo-add__input"
                  placeholder="0"
                  value={formData.heightCm}
                  onChange={handleChange}
                />
              </div>

              <div className="cargo-add__field">
                <label className="cargo-add__label">Количество паллет</label>
                <input
                  type="number"
                  name="palletsCount"
                  className="cargo-add__input"
                  placeholder="0"
                  value={formData.palletsCount}
                  onChange={handleChange}
                />
                {errors.palletsCount && <span className="cargo-add__error">{errors.palletsCount}</span>}
              </div>

              <div className="cargo-add__field">
                <label className="cargo-add__label">Вес одного места (кг)</label>
                <input
                  type="number"
                  name="weightPerPackageKg"
                  className={getInputClass('weightPerPackageKg')}
                  placeholder="0"
                  step="0.1"
                  min="0"
                  value={formData.weightPerPackageKg}
                  onChange={handleChange}
                  disabled={formData.calculationMode !== 'automatic'}
                />
                {errors.weightPerPackageKg && (
                  <span className="cargo-add__error">{errors.weightPerPackageKg}</span>
                )}
              </div>

              <div className="cargo-add__field">
                <label className="cargo-add__label">Тип упаковки</label>
                <input
                  type="text"
                  name="packagingType"
                  className="cargo-add__input"
                  placeholder="Коробки, мешки..."
                  value={formData.packagingType}
                  onChange={handleChange}
                />
              </div>
            </div>
          </section>

          {/* Секция 4: Финансы */}
          <section className="cargo-add__section">
            <h2 className="cargo-add__section-title">Финансы</h2>
            <div className="cargo-add__grid cargo-add__grid--2">
              <div className="cargo-add__field">
                <label className="cargo-add__label">
                  Цена (₽) <span className="cargo-add__required">*</span>
                </label>
                <input
                  type="number"
                  name="startingPrice"
                  className={getInputClass('startingPrice')}
                  placeholder="0"
                  step="1000"
                  min="0"
                  value={formData.startingPrice}
                  onChange={handleChange}
                />
                {errors.startingPrice && <span className="cargo-add__error">{errors.startingPrice}</span>}
              </div>

              <div className="cargo-add__field">
                <label className="cargo-add__label">
                  Тип оплаты <span className="cargo-add__required">*</span>
                </label>
                <select
                  name="paymentType"
                  className="cargo-add__input cargo-add__select"
                  value={formData.paymentType}
                  onChange={handleChange}
                >
                  <option value="Cash">Наличные</option>
                  <option value="WithVAT">С НДС</option>
                  <option value="WithoutVAT">Без НДС</option>
                </select>
              </div>

              <div className="cargo-add__field">
                <label className="cargo-add__label">Предоплата (%)</label>
                <input
                  type="number"
                  name="prepaymentPercent"
                  className="cargo-add__input"
                  placeholder="0-100"
                  min="0"
                  max="100"
                  value={formData.prepaymentPercent}
                  onChange={handleChange}
                />
              </div>

              <div className="cargo-add__field">
                <label className="cargo-add__label">Мин. шаг ставки (₽)</label>
                <input
                  type="number"
                  name="minBidStep"
                  className={getInputClass('minBidStep')}
                  placeholder="1000"
                  step="100"
                  min="0"
                  value={formData.minBidStep}
                  onChange={handleChange}
                />
                {errors.minBidStep && <span className="cargo-add__error">{errors.minBidStep}</span>}
              </div>
            </div>

            <div className="cargo-add__checkboxes">
              <label className="cargo-add__checkbox">
                <input
                  type="checkbox"
                  name="allowBargaining"
                  checked={formData.allowBargaining}
                  onChange={handleChange}
                />
                <span>Разрешить торг</span>
              </label>

              <label className="cargo-add__checkbox">
                <input
                  type="checkbox"
                  name="biddingEnabled"
                  checked={formData.biddingEnabled}
                  onChange={handleChange}
                />
                <span>Включить ставки</span>
              </label>
            </div>
          </section>

          {/* Секция 5: Настройки */}
          <section className="cargo-add__section">
            <h2 className="cargo-add__section-title">Настройки</h2>
            <div className="cargo-add__grid cargo-add__grid--2">
              <div className="cargo-add__field">
                <label className="cargo-add__label">Видимость</label>
                <select
                  name="visibility"
                  className="cargo-add__input cargo-add__select"
                  value={formData.visibility}
                  onChange={handleChange}
                >
                  <option value="Exchange">Биржа (публично)</option>
                  <option value="Private">Приватно</option>
                </select>
              </div>
            </div>

            <div className="cargo-add__checkboxes">
              <label className="cargo-add__checkbox">
                <input
                  type="checkbox"
                  name="requiresCMR"
                  checked={formData.requiresCMR}
                  onChange={handleChange}
                />
                <span>Требуется CMR</span>
              </label>

              <label className="cargo-add__checkbox">
                <input
                  type="checkbox"
                  name="requiresTIR"
                  checked={formData.requiresTIR}
                  onChange={handleChange}
                />
                <span>Требуется TIR</span>
              </label>

              <label className="cargo-add__checkbox">
                <input
                  type="checkbox"
                  name="isADR"
                  checked={formData.isADR}
                  onChange={handleChange}
                />
                <span>Опасный груз (ADR)</span>
              </label>

              <label className="cargo-add__checkbox">
                <input
                  type="checkbox"
                  name="requiresTwoDrivers"
                  checked={formData.requiresTwoDrivers}
                  onChange={handleChange}
                />
                <span>Требуется два водителя</span>
              </label>
            </div>
          </section>

          {/* Кнопки действий */}
          <div className="cargo-add__actions">
            <button
              type="button"
              className="cargo-add__btn cargo-add__btn--secondary"
              onClick={handleCancel}
              disabled={isSubmitting}
            >
              Отмена
            </button>
            <button
              type="button"
              className="cargo-add__btn cargo-add__btn--draft"
              onClick={() => handleSubmit(true)}
              disabled={isSubmitting}
            >
              {isSubmitting ? 'Сохранение...' : isEditMode ? 'Сохранить' : 'Сохранить черновик'}
            </button>
            <button
              type="button"
              className="cargo-add__btn cargo-add__btn--primary"
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
