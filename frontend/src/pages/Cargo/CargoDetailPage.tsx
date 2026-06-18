import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { api, CargoResponse, ListingStatus, ContextType } from '../../api/client';
import { useAuthStore } from '../../store/authStore';
import { Toast } from '../../components/ui/Toast';
import { RouteMapDisplay } from '../../components/route/RouteMapDisplay';
import './CargoDetailPage.css';

type ToastType = 'success' | 'error' | 'info';

interface ToastData {
  message: string;
  type: ToastType;
}

export const CargoDetailPage = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { isAuthenticated, currentUser } = useAuthStore();
  const [cargo, setCargo] = useState<CargoResponse | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [toast, setToast] = useState<ToastData | null>(null);

  useEffect(() => {
    if (id) {
      loadCargo(id);
    }
  }, [id]);

  const loadCargo = async (cargoId: string) => {
    try {
      setIsLoading(true);
      const data = await api.cargos.getById(cargoId);
      setCargo(data);
    } catch (error) {
      console.error('Ошибка загрузки груза:', error);
      showToast('Не удалось загрузить данные груза', 'error');
    } finally {
      setIsLoading(false);
    }
  };

  const showToast = (message: string, type: ToastType) => {
    setToast({ message, type });
  };

  const getStatusLabel = (status: ListingStatus) => {
    const labels: Record<ListingStatus, string> = {
      [ListingStatus.Draft]: 'Черновик',
      [ListingStatus.Published]: 'Опубликован',
      [ListingStatus.Archived]: 'В архиве',
      [ListingStatus.Completed]: 'Завершён',
    };
    return labels[status] || status;
  };

  const getStatusClass = (status: ListingStatus) => {
    const classes: Record<ListingStatus, string> = {
      [ListingStatus.Draft]: 'cargo-detail__status--draft',
      [ListingStatus.Published]: 'cargo-detail__status--published',
      [ListingStatus.Archived]: 'cargo-detail__status--archived',
      [ListingStatus.Completed]: 'cargo-detail__status--completed',
    };
    return classes[status] || '';
  };

  const handleChatClick = async () => {
    if (!isAuthenticated || !cargo) {
      navigate('/auth');
      return;
    }

    try {
    const chat = await api.chats.create({
      contextType: ContextType.Cargo,
      contextId: cargo.id,
      recipientUserId: cargo.userId,
    });
      navigate(`/chats/${chat.id}`);
    } catch (error) {
      console.error('Ошибка создания чата:', error);
      showToast('Не удалось создать чат', 'error');
    }
  };

  const handleArchive = async () => {
    if (!cargo) return;
    if (!confirm('Вы уверены, что хотите архивировать этот груз?')) return;

    try {
      await api.cargos.archive(cargo.id);
      showToast('Груз архивирован', 'success');
      await loadCargo(cargo.id);
    } catch (error) {
      console.error('Ошибка архивации:', error);
      showToast('Ошибка при архивации', 'error');
    }
  };

  const handlePublish = async () => {
    if (!cargo) return;

    try {
      await api.cargos.publish(cargo.id);
      showToast('Груз опубликован', 'success');
      await loadCargo(cargo.id);
    } catch (error) {
      console.error('Ошибка публикации:', error);
      showToast('Ошибка при публикации', 'error');
    }
  };

  const handleDelete = async () => {
    if (!cargo) return;
    if (!confirm('Вы уверены, что хотите удалить этот груз? Это действие нельзя отменить.')) return;

    try {
      await api.cargos.delete(cargo.id);
      showToast('Груз удалён', 'success');
      setTimeout(() => navigate('/cargo'), 1000);
    } catch (error) {
      console.error('Ошибка удаления:', error);
      showToast('Ошибка при удалении', 'error');
    }
  };

  const isOwner = currentUser?.id === cargo?.userId;

  const formatDate = (dateString?: string | null) => {
    if (!dateString) return '—';
    return new Date(dateString).toLocaleString('ru-RU', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  if (isLoading) {
    return (
      <div className="cargo-detail">
        <div className="cargo-detail__container">
          <div className="cargo-detail__loading">Загрузка...</div>
        </div>
      </div>
    );
  }

  if (!cargo) {
    return (
      <div className="cargo-detail">
        <div className="cargo-detail__container">
          <div className="cargo-detail__empty">
            <h2>Груз не найден</h2>
            <button className="cargo-detail__btn cargo-detail__btn--primary" onClick={() => navigate('/cargo')}>
              ← Вернуться к списку
            </button>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="cargo-detail">
      {toast && (
        <Toast message={toast.message} type={toast.type} onClose={() => setToast(null)} />
      )}

      <div className="cargo-detail__container">
        <div className="cargo-detail__header">
          <button className="cargo-detail__back-btn" onClick={() => navigate('/cargo')}>
            ← Назад к списку
          </button>
          <div className="cargo-detail__title-row">
            <h1 className="cargo-detail__title">{cargo.cargoName || 'Без названия'}</h1>
            <span className={`cargo-detail__status ${getStatusClass(cargo.status)}`}>
              {getStatusLabel(cargo.status)}
            </span>
          </div>
          {cargo.title && <p className="cargo-detail__subtitle">{cargo.title}</p>}
        </div>

        {/* Основная информация */}
        <section className="cargo-detail__section">
          <h2 className="cargo-detail__section-title">Маршрут</h2>
          <div className="cargo-detail__route">
            <div className="cargo-detail__route-point">
              <span className="cargo-detail__route-label">Откуда</span>
              <span className="cargo-detail__route-value">{cargo.routeFrom || '—'}</span>
            </div>
            <div className="cargo-detail__route-arrow">→</div>
            <div className="cargo-detail__route-point">
              <span className="cargo-detail__route-label">Куда</span>
              <span className="cargo-detail__route-value">{cargo.routeTo || '—'}</span>
            </div>
          </div>
          <div className="cargo-detail__dates">
            <div className="cargo-detail__date-item">
              <span className="cargo-detail__date-label">Погрузка</span>
              <span className="cargo-detail__date-value">{formatDate(cargo.loadDateTime)}</span>
            </div>
            <div className="cargo-detail__date-item">
              <span className="cargo-detail__date-label">Выгрузка</span>
              <span className="cargo-detail__date-value">{formatDate(cargo.unloadDateTime)}</span>
            </div>
          </div>
          <RouteMapDisplay
            points={cargo.routePoints}
            geometry={cargo.routeGeometryGeoJson}
            distanceKm={cargo.routeDistanceKm}
            durationMinutes={cargo.routeDurationMinutes}
            fuelLiters={cargo.routeFuelLiters}
          />
        </section>

        {/* Параметры */}
        <section className="cargo-detail__section">
          <h2 className="cargo-detail__section-title">Параметры груза</h2>
          <div className="cargo-detail__params">
            <div className="cargo-detail__param">
              <span className="cargo-detail__param-label">Вес</span>
              <span className="cargo-detail__param-value">{cargo.weightTons} т</span>
            </div>
            <div className="cargo-detail__param">
              <span className="cargo-detail__param-label">Объём</span>
              <span className="cargo-detail__param-value">{cargo.volumeM3} м³</span>
            </div>
            <div className="cargo-detail__param">
              <span className="cargo-detail__param-label">Тип кузова</span>
              <span className="cargo-detail__param-value">{cargo.bodyTypeRequired || 'Любой'}</span>
            </div>
            <div className="cargo-detail__param">
              <span className="cargo-detail__param-label">Тип загрузки</span>
              <span className="cargo-detail__param-value">{cargo.loadingType}</span>
            </div>
            {(cargo.lengthCm || cargo.widthCm || cargo.heightCm) && (
              <>
                {cargo.lengthCm && (
                  <div className="cargo-detail__param">
                    <span className="cargo-detail__param-label">Длина</span>
                    <span className="cargo-detail__param-value">{cargo.lengthCm} см</span>
                  </div>
                )}
                {cargo.widthCm && (
                  <div className="cargo-detail__param">
                    <span className="cargo-detail__param-label">Ширина</span>
                    <span className="cargo-detail__param-value">{cargo.widthCm} см</span>
                  </div>
                )}
                {cargo.heightCm && (
                  <div className="cargo-detail__param">
                    <span className="cargo-detail__param-label">Высота</span>
                    <span className="cargo-detail__param-value">{cargo.heightCm} см</span>
                  </div>
                )}
              </>
            )}
            {cargo.palletsCount && (
              <div className="cargo-detail__param">
                <span className="cargo-detail__param-label">Паллет</span>
                <span className="cargo-detail__param-value">{cargo.palletsCount} шт</span>
              </div>
            )}
            {cargo.packagingType && (
              <div className="cargo-detail__param">
                <span className="cargo-detail__param-label">Упаковка</span>
                <span className="cargo-detail__param-value">{cargo.packagingType}</span>
              </div>
            )}
          </div>
        </section>

        {/* Финансы */}
        <section className="cargo-detail__section">
          <h2 className="cargo-detail__section-title">Финансы</h2>
          <div className="cargo-detail__finance">
            <div className="cargo-detail__price">
              <span className="cargo-detail__price-label">Цена</span>
              <span className="cargo-detail__price-value">
                {cargo.startingPrice ? `${cargo.startingPrice.toLocaleString('ru-RU')} ₽` : 'Договорная'}
              </span>
            </div>
            <div className="cargo-detail__param">
              <span className="cargo-detail__param-label">Тип оплаты</span>
              <span className="cargo-detail__param-value">
                {cargo.paymentType === 'Cash' && 'Наличные'}
                {cargo.paymentType === 'WithVAT' && 'С НДС'}
                {cargo.paymentType === 'WithoutVAT' && 'Без НДС'}
              </span>
            </div>
            {cargo.prepaymentPercent !== undefined && cargo.prepaymentPercent !== null && (
              <div className="cargo-detail__param">
                <span className="cargo-detail__param-label">Предоплата</span>
                <span className="cargo-detail__param-value">{cargo.prepaymentPercent}%</span>
              </div>
            )}
            {cargo.allowBargaining && (
              <div className="cargo-detail__param">
                <span className="cargo-detail__param-label">Торг</span>
                <span className="cargo-detail__param-value cargo-detail__param-value--yes">Разрешён</span>
              </div>
            )}
            {cargo.biddingEnabled && (
              <div className="cargo-detail__param">
                <span className="cargo-detail__param-label">Ставки</span>
                <span className="cargo-detail__param-value cargo-detail__param-value--yes">
                  Включены{cargo.minBidStep ? ` (шаг ${cargo.minBidStep} ₽)` : ''}
                </span>
              </div>
            )}
          </div>
        </section>

        {/* Дополнительно */}
        {(cargo.requiresCMR || cargo.requiresTIR || cargo.isADR || cargo.requiresTwoDrivers || cargo.notes) && (
          <section className="cargo-detail__section">
            <h2 className="cargo-detail__section-title">Дополнительно</h2>
            <div className="cargo-detail__extras">
              {cargo.requiresCMR && <span className="cargo-detail__tag">CMR</span>}
              {cargo.requiresTIR && <span className="cargo-detail__tag">TIR</span>}
              {cargo.isADR && <span className="cargo-detail__tag cargo-detail__tag--danger">ADR (опасный)</span>}
              {cargo.requiresTwoDrivers && <span className="cargo-detail__tag">2 водителя</span>}
              {cargo.notes && (
                <div className="cargo-detail__notes">
                  <strong>Заметки:</strong>
                  <p>{cargo.notes}</p>
                </div>
              )}
            </div>
          </section>
        )}

        {/* Мета-информация */}
        <section className="cargo-detail__section">
          <h2 className="cargo-detail__section-title">Информация</h2>
          <div className="cargo-detail__meta">
            <div className="cargo-detail__meta-item">
              <span className="cargo-detail__meta-label">ID</span>
              <span className="cargo-detail__meta-value">{cargo.id}</span>
            </div>
            <div className="cargo-detail__meta-item">
              <span className="cargo-detail__meta-label">Создан</span>
              <span className="cargo-detail__meta-value">{formatDate(cargo.createdAt)}</span>
            </div>
            {cargo.publishedAt && (
              <div className="cargo-detail__meta-item">
                <span className="cargo-detail__meta-label">Опубликован</span>
                <span className="cargo-detail__meta-value">{formatDate(cargo.publishedAt)}</span>
              </div>
            )}
          </div>
        </section>

        {/* Действия */}
        <div className="cargo-detail__actions">
          {!isOwner && (
            <button
              className="cargo-detail__btn cargo-detail__btn--chat"
              onClick={handleChatClick}
            >
              Написать в чат
            </button>
          )}

          {isOwner && cargo.status === ListingStatus.Draft && (
            <button
              className="cargo-detail__btn cargo-detail__btn--primary"
              onClick={handlePublish}
            >
              Опубликовать
            </button>
          )}

          {isOwner && cargo.status === ListingStatus.Published && (
            <button
              className="cargo-detail__btn cargo-detail__btn--secondary"
              onClick={handleArchive}
            >
              Архивировать
            </button>
          )}

          {isOwner && (
            <button
              className="cargo-detail__btn cargo-detail__btn--danger"
              onClick={handleDelete}
            >
              Удалить
            </button>
          )}
        </div>
      </div>
    </div>
  );
};
