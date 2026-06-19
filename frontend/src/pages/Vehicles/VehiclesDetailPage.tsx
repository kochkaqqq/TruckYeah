import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
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
  const { t } = useTranslation();
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
      showToast(t('vehicle.failedToLoad'), 'error');
    } finally {
      setIsLoading(false);
    }
  };

  const showToast = (message: string, type: ToastType) => {
    setToast({ message, type });
  };

  const getStatusLabel = (status: ListingStatus) => {
    const labels: Record<ListingStatus, string> = {
      [ListingStatus.Draft]: t('status.draft'),
      [ListingStatus.PendingModeration]: t('status.pendingModeration'),
      [ListingStatus.Published]: t('status.published'),
      [ListingStatus.Rejected]: t('status.rejected'),
      [ListingStatus.Archived]: t('status.archived'),
      [ListingStatus.Completed]: t('status.completed'),
    };
    return labels[status] || status;
  };

  const getStatusClass = (status: ListingStatus) => {
    const classes: Record<ListingStatus, string> = {
      [ListingStatus.Draft]: 'vehicle-detail__status--draft',
      [ListingStatus.PendingModeration]: 'vehicle-detail__status--draft',
      [ListingStatus.Published]: 'vehicle-detail__status--published',
      [ListingStatus.Rejected]: 'vehicle-detail__status--archived',
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
      showToast(t('vehicle.failedToCreateChat'), 'error');
    }
  };

  const handleArchive = async () => {
    if (!truck) return;
    if (!confirm(t('vehicle.confirmArchive'))) return;

    try {
      await api.trucks.archive(truck.id);
      showToast(t('vehicle.archived'), 'success');
      await loadTruck(truck.id);
    } catch (error) {
      console.error('Ошибка архивации:', error);
      showToast(t('vehicle.archiveError'), 'error');
    }
  };

  const handlePublish = async () => {
    if (!truck) return;

    try {
      await api.trucks.publish(truck.id);
      showToast(t('vehicle.published'), 'success');
      await loadTruck(truck.id);
    } catch (error) {
      console.error('Ошибка публикации:', error);
      showToast(t('vehicle.publishError'), 'error');
    }
  };

  const handleDelete = async () => {
    if (!truck) return;
    if (!confirm(t('vehicle.confirmDelete'))) return;

    try {
      await api.trucks.delete(truck.id);
      showToast(t('vehicle.deleted'), 'success');
      setTimeout(() => navigate('/vehicles'), 1000);
    } catch (error) {
      console.error('Ошибка удаления:', error);
      showToast(t('vehicle.deleteError'), 'error');
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
          <div className="vehicle-detail__loading">{t('common.loading')}</div>
        </div>
      </div>
    );
  }

  if (!truck) {
    return (
      <div className="vehicle-detail">
        <div className="vehicle-detail__container">
          <div className="vehicle-detail__empty">
            <h2>{t('vehicle.notFound')}</h2>
            <button className="vehicle-detail__btn vehicle-detail__btn--primary" onClick={() => navigate('/vehicles')}>
              ← {t('common.backToList')}
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
            ← {t('common.backToList')}
          </button>
          <div className="vehicle-detail__title-row">
            <h1 className="vehicle-detail__title">{truck.title || truck.bodyType || t('common.noTitle')}</h1>
            <span className={`vehicle-detail__status ${getStatusClass(truck.status)}`}>
              {getStatusLabel(truck.status)}
            </span>
          </div>
          {truck.description && <p className="vehicle-detail__subtitle">{truck.description}</p>}
        </div>

        <section className="vehicle-detail__section">
          <h2 className="vehicle-detail__section-title">{t('vehicle.route')}</h2>
          <div className="vehicle-detail__route">
            <div className="vehicle-detail__route-point">
              <span className="vehicle-detail__route-label">{t('vehicle.routeFrom')}</span>
              <span className="vehicle-detail__route-value">{truck.routeFrom || '—'}</span>
            </div>
            <div className="vehicle-detail__route-arrow">→</div>
            <div className="vehicle-detail__route-point">
              <span className="vehicle-detail__route-label">{t('vehicle.routeTo')}</span>
              <span className="vehicle-detail__route-value">{truck.routeTo || '—'}</span>
            </div>
          </div>
          <div className="vehicle-detail__dates">
            <div className="vehicle-detail__date-item">
              <span className="vehicle-detail__date-label">{t('vehicle.availableFrom')}</span>
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
          <h2 className="vehicle-detail__section-title">{t('vehicle.parameters')}</h2>
          <div className="vehicle-detail__params">
            <div className="vehicle-detail__param">
              <span className="vehicle-detail__param-label">{t('vehicle.capacity')}</span>
              <span className="vehicle-detail__param-value">{truck.capacityTons} т</span>
            </div>
            <div className="vehicle-detail__param">
              <span className="vehicle-detail__param-label">{t('vehicle.bodyVolume')}</span>
              <span className="vehicle-detail__param-value">{truck.volumeM3} м³</span>
            </div>
            <div className="vehicle-detail__param">
              <span className="vehicle-detail__param-label">{t('vehicle.bodyType')}</span>
              <span className="vehicle-detail__param-value">{truck.bodyType || t('vehicle.any')}</span>
            </div>
            <div className="vehicle-detail__param">
              <span className="vehicle-detail__param-label">{t('vehicle.loadingType')}</span>
              <span className="vehicle-detail__param-value">{truck.loadingType}</span>
            </div>
            {truck.crewDriversCount && (
              <div className="vehicle-detail__param">
                <span className="vehicle-detail__param-label">{t('vehicle.drivers')}</span>
                <span className="vehicle-detail__param-value">{truck.crewDriversCount}</span>
              </div>
            )}
            {truck.additionalEquipment && (
              <div className="vehicle-detail__param">
                <span className="vehicle-detail__param-label">{t('vehicle.equipment')}</span>
                <span className="vehicle-detail__param-value">{truck.additionalEquipment}</span>
              </div>
            )}
          </div>
        </section>

        <section className="vehicle-detail__section">
          <h2 className="vehicle-detail__section-title">{t('vehicle.finance')}</h2>
          <div className="vehicle-detail__finance">
            <div className="vehicle-detail__price">
              <span className="vehicle-detail__price-label">{t('vehicle.price')}</span>
              <span className="vehicle-detail__price-value">
                {truck.price ? `${truck.price.toLocaleString('ru-RU')} €` : t('vehicle.negotiable')}
              </span>
            </div>
            <div className="vehicle-detail__param">
              <span className="vehicle-detail__param-label">{t('vehicle.paymentType')}</span>
              <span className="vehicle-detail__param-value">
                {truck.paymentType === 'Cash' && t('vehicle.paymentCash')}
                {truck.paymentType === 'WithVAT' && t('vehicle.paymentWithVAT')}
                {truck.paymentType === 'WithoutVAT' && t('vehicle.paymentWithoutVAT')}
              </span>
            </div>
            {truck.prepaymentPercent !== undefined && truck.prepaymentPercent !== null && (
              <div className="vehicle-detail__param">
                <span className="vehicle-detail__param-label">{t('vehicle.prepayment')}</span>
                <span className="vehicle-detail__param-value">{truck.prepaymentPercent}%</span>
              </div>
            )}
            {truck.allowBargaining && (
              <div className="vehicle-detail__param">
                <span className="vehicle-detail__param-label">{t('vehicle.bargaining')}</span>
                <span className="vehicle-detail__param-value vehicle-detail__param-value--yes">{t('vehicle.allowed')}</span>
              </div>
            )}
          </div>
        </section>

        <section className="vehicle-detail__section">
          <h2 className="vehicle-detail__section-title">{t('vehicle.information')}</h2>
          <div className="vehicle-detail__meta">
            <div className="vehicle-detail__meta-item">
              <span className="vehicle-detail__meta-label">ID</span>
              <span className="vehicle-detail__meta-value">{truck.id}</span>
            </div>
            <div className="vehicle-detail__meta-item">
              <span className="vehicle-detail__meta-label">{t('vehicle.createdAt')}</span>
              <span className="vehicle-detail__meta-value">{formatDate(truck.createdAt)}</span>
            </div>
            {truck.publishedAt && (
              <div className="vehicle-detail__meta-item">
                <span className="vehicle-detail__meta-label">{t('vehicle.publishedAt')}</span>
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
              {t('vehicle.writeToChat')}
            </button>
          )}

          {isOwner && truck.status === ListingStatus.Draft && (
            <button
              className="vehicle-detail__btn vehicle-detail__btn--primary"
              onClick={handlePublish}
            >
              {t('vehicle.publish')}
            </button>
          )}

          {isOwner && truck.status === ListingStatus.Published && (
            <button
              className="vehicle-detail__btn vehicle-detail__btn--secondary"
              onClick={handleArchive}
            >
              {t('vehicle.archive')}
            </button>
          )}

          {isOwner && (
            <button
              className="vehicle-detail__btn vehicle-detail__btn--danger"
              onClick={handleDelete}
            >
              {t('common.delete')}
            </button>
          )}
        </div>
      </div>
    </div>
  );
};