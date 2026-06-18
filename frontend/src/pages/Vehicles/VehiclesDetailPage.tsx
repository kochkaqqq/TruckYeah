import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { api, TruckResponse, ListingStatus, ContextType } from '../../api/client';
import { useAuthStore } from '../../store/authStore';
import { Toast } from '../../components/ui/Toast';
import { RouteMapDisplay } from '../../components/route/RouteMapDisplay';
import './VehiclesDetailPage.css';

type ToastType = 'success' | 'error' | 'info';

interface ToastData {
  message: string;
  type: ToastType;
}

export const VehiclesDetailPage = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { isAuthenticated, currentUser } = useAuthStore();
  const [truck, setTruck] = useState<TruckResponse | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [toast, setToast] = useState<ToastData | null>(null);

  useEffect(() => {
    if (id) {
      loadTruck(id);
    }
  }, [id]);

  const loadTruck = async (truckId: string) => {
    try {
      setIsLoading(true);
      const data = await api.trucks.getById(truckId);
      setTruck(data);
    } catch (error) {
      console.error('Ошибка загрузки машины:', error);
      showToast('Не удалось загрузить данные машины', 'error');
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
      [ListingStatus.Draft]: 'vehicle-detail__status--draft',
      [ListingStatus.Published]: 'vehicle-detail__status--published',
      [ListingStatus.Archived]: 'vehicle-detail__status--archived',
      [ListingStatus.Completed]: 'vehicle-detail__status--completed',
    };
    return classes[status] || '';
  };

  const handleChatClick = async () => {
    if (!isAuthenticated || !truck) {
      navigate('/auth');
      return;
    }

    try {
      const chat = await api.chats.create({
        contextType: ContextType.Truck,
        contextId: truck.id,
        recipientUserId: truck.userId,
      });
      navigate(`/chats/${chat.id}`);
    } catch (error) {
      console.error('Ошибка создания чата:', error);
      showToast('Не удалось создать чат', 'error');
    }
  };

  const handleArchive = async () => {
    if (!truck) return;
    if (!confirm('Вы уверены, что хотите архивировать эту машину?')) return;

    try {
      await api.trucks.archive(truck.id);
      showToast('Машина архивирована', 'success');
      await loadTruck(truck.id);
    } catch (error) {
      console.error('Ошибка архивации:', error);
      showToast('Ошибка при архивации', 'error');
    }
  };

  const handlePublish = async () => {
    if (!truck) return;

    try {
      await api.trucks.publish(truck.id);
      showToast('Машина опубликована', 'success');
      await loadTruck(truck.id);
    } catch (error) {
      console.error('Ошибка публикации:', error);
      showToast('Ошибка при публикации', 'error');
    }
  };

  const handleDelete = async () => {
    if (!truck) return;
    if (!confirm('Вы уверены, что хотите удалить эту машину? Это действие нельзя отменить.')) return;

    try {
      await api.trucks.delete(truck.id);
      showToast('Машина удалена', 'success');
      setTimeout(() => navigate('/vehicles'), 1000);
    } catch (error) {
      console.error('Ошибка удаления:', error);
      showToast('Ошибка при удалении', 'error');
    }
  };

  const isOwner = currentUser?.id === truck?.userId;

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
      <div className="vehicle-detail">
        <div className="vehicle-detail__container">
          <div className="vehicle-detail__loading">Загрузка...</div>
        </div>
      </div>
    );
  }

  if (!truck) {
    return (
      <div className="vehicle-detail">
        <div className="vehicle-detail__container">
          <div className="vehicle-detail__empty">
            <h2>Машина не найдена</h2>
            <button className="vehicle-detail__btn vehicle-detail__btn--primary" onClick={() => navigate('/vehicles')}>
              ← Вернуться к списку
            </button>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="vehicle-detail">
      {toast && (
        <Toast message={toast.message} type={toast.type} onClose={() => setToast(null)} />
      )}

      <div className="vehicle-detail__container">
        <div className="vehicle-detail__header">
          <button className="vehicle-detail__back-btn" onClick={() => navigate('/vehicles')}>
            ← Назад к списку
          </button>
          <div className="vehicle-detail__title-row">
            <h1 className="vehicle-detail__title">{truck.title || truck.bodyType || 'Без названия'}</h1>
            <span className={`vehicle-detail__status ${getStatusClass(truck.status)}`}>
              {getStatusLabel(truck.status)}
            </span>
          </div>
          {truck.description && <p className="vehicle-detail__subtitle">{truck.description}</p>}
        </div>

        <section className="vehicle-detail__section">
          <h2 className="vehicle-detail__section-title">Маршрут</h2>
          <div className="vehicle-detail__route">
            <div className="vehicle-detail__route-point">
              <span className="vehicle-detail__route-label">Откуда</span>
              <span className="vehicle-detail__route-value">{truck.routeFrom || '—'}</span>
            </div>
            <div className="vehicle-detail__route-arrow">→</div>
            <div className="vehicle-detail__route-point">
              <span className="vehicle-detail__route-label">Куда</span>
              <span className="vehicle-detail__route-value">{truck.routeTo || '—'}</span>
            </div>
          </div>
          <div className="vehicle-detail__dates">
            <div className="vehicle-detail__date-item">
              <span className="vehicle-detail__date-label">Доступна с</span>
              <span className="vehicle-detail__date-value">{formatDate(truck.availableFrom)}</span>
            </div>
          </div>
          <RouteMapDisplay
            points={truck.routePoints}
            geometry={truck.routeGeometryGeoJson}
            distanceKm={truck.routeDistanceKm}
            durationMinutes={truck.routeDurationMinutes}
            fuelLiters={truck.routeFuelLiters}
          />
        </section>

        <section className="vehicle-detail__section">
          <h2 className="vehicle-detail__section-title">Параметры машины</h2>
          <div className="vehicle-detail__params">
            <div className="vehicle-detail__param">
              <span className="vehicle-detail__param-label">Грузоподъёмность</span>
              <span className="vehicle-detail__param-value">{truck.capacityTons} т</span>
            </div>
            <div className="vehicle-detail__param">
              <span className="vehicle-detail__param-label">Объём кузова</span>
              <span className="vehicle-detail__param-value">{truck.volumeM3} м³</span>
            </div>
            <div className="vehicle-detail__param">
              <span className="vehicle-detail__param-label">Тип кузова</span>
              <span className="vehicle-detail__param-value">{truck.bodyType || 'Любой'}</span>
            </div>
            <div className="vehicle-detail__param">
              <span className="vehicle-detail__param-label">Тип загрузки</span>
              <span className="vehicle-detail__param-value">{truck.loadingType}</span>
            </div>
            {truck.crewDriversCount && (
              <div className="vehicle-detail__param">
                <span className="vehicle-detail__param-label">Водителей</span>
                <span className="vehicle-detail__param-value">{truck.crewDriversCount}</span>
              </div>
            )}
            {truck.additionalEquipment && (
              <div className="vehicle-detail__param">
                <span className="vehicle-detail__param-label">Оборудование</span>
                <span className="vehicle-detail__param-value">{truck.additionalEquipment}</span>
              </div>
            )}
          </div>
        </section>

        <section className="vehicle-detail__section">
          <h2 className="vehicle-detail__section-title">Финансы</h2>
          <div className="vehicle-detail__finance">
            <div className="vehicle-detail__price">
              <span className="vehicle-detail__price-label">Цена</span>
              <span className="vehicle-detail__price-value">
                {truck.price ? `${truck.price.toLocaleString('ru-RU')} €` : 'Договорная'}
              </span>
            </div>
            <div className="vehicle-detail__param">
              <span className="vehicle-detail__param-label">Тип оплаты</span>
              <span className="vehicle-detail__param-value">
                {truck.paymentType === 'Cash' && 'Наличные'}
                {truck.paymentType === 'WithVAT' && 'С НДС'}
                {truck.paymentType === 'WithoutVAT' && 'Без НДС'}
              </span>
            </div>
            {truck.prepaymentPercent !== undefined && truck.prepaymentPercent !== null && (
              <div className="vehicle-detail__param">
                <span className="vehicle-detail__param-label">Предоплата</span>
                <span className="vehicle-detail__param-value">{truck.prepaymentPercent}%</span>
              </div>
            )}
            {truck.allowBargaining && (
              <div className="vehicle-detail__param">
                <span className="vehicle-detail__param-label">Торг</span>
                <span className="vehicle-detail__param-value vehicle-detail__param-value--yes">Разрешён</span>
              </div>
            )}
          </div>
        </section>

        <section className="vehicle-detail__section">
          <h2 className="vehicle-detail__section-title">Информация</h2>
          <div className="vehicle-detail__meta">
            <div className="vehicle-detail__meta-item">
              <span className="vehicle-detail__meta-label">ID</span>
              <span className="vehicle-detail__meta-value">{truck.id}</span>
            </div>
            <div className="vehicle-detail__meta-item">
              <span className="vehicle-detail__meta-label">Создана</span>
              <span className="vehicle-detail__meta-value">{formatDate(truck.createdAt)}</span>
            </div>
            {truck.publishedAt && (
              <div className="vehicle-detail__meta-item">
                <span className="vehicle-detail__meta-label">Опубликована</span>
                <span className="vehicle-detail__meta-value">{formatDate(truck.publishedAt)}</span>
              </div>
            )}
          </div>
        </section>

        <div className="vehicle-detail__actions">
          {!isOwner && (
            <button
              className="vehicle-detail__btn vehicle-detail__btn--chat"
              onClick={handleChatClick}
            >
              Написать в чат
            </button>
          )}

          {isOwner && truck.status === ListingStatus.Draft && (
            <button
              className="vehicle-detail__btn vehicle-detail__btn--primary"
              onClick={handlePublish}
            >
              Опубликовать
            </button>
          )}

          {isOwner && truck.status === ListingStatus.Published && (
            <button
              className="vehicle-detail__btn vehicle-detail__btn--secondary"
              onClick={handleArchive}
            >
              Архивировать
            </button>
          )}

          {isOwner && (
            <button
              className="vehicle-detail__btn vehicle-detail__btn--danger"
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
