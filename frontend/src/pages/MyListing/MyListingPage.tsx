import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { api, CargoResponse, TruckResponse, ListingStatus } from '../../api/client';
import { useAuthStore } from '../../store/authStore';
import { Toast } from '../../components/ui/Toast';
import './MyListingPage.css';

type ToastType = 'success' | 'error' | 'info';

interface ToastData {
  message: string;
  type: ToastType;
}

export const MyListingPage = () => {
  const navigate = useNavigate();
  const { isAuthenticated } = useAuthStore();
  const [cargos, setCargos] = useState<CargoResponse[]>([]);
  const [trucks, setTrucks] = useState<TruckResponse[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [toast, setToast] = useState<ToastData | null>(null);

  useEffect(() => {
    if (!isAuthenticated) {
      navigate('/auth');
      return;
    }
    loadMyListings();
  }, [isAuthenticated, navigate]);

  const loadMyListings = async () => {
  try {
    setIsLoading(true);

    const [myCargos, myTrucks] = await Promise.all([
      api.cargos.getMyCargos(),
      api.trucks.getMyTrucks(),
    ]);

    setCargos(myCargos ?? []);
    setTrucks(myTrucks ?? []);
  } catch (error) {
    console.error('❌ Ошибка загрузки:', error);
    showToast('Не удалось загрузить данные', 'error');
  } finally {
    setIsLoading(false);
  }
};

  const showToast = (message: string, type: ToastType) => {
    setToast({ message, type });
  };

  const handleDeleteCargo = async (id: string) => {
    if (!confirm('Вы уверены, что хотите удалить этот груз?')) return;

    try {
      await api.cargos.delete(id);
      showToast('Груз удалён', 'success');
      await loadMyListings();
    } catch (error) {
      console.error('Ошибка удаления:', error);
      showToast('Ошибка при удалении', 'error');
    }
  };

  const handleDeleteTruck = async (id: string) => {
    if (!confirm('Вы уверены, что хотите удалить эту машину?')) return;

    try {
      await api.trucks.delete(id);
      showToast('Машина удалена', 'success');
      await loadMyListings();
    } catch (error) {
      console.error('Ошибка удаления:', error);
      showToast('Ошибка при удалении', 'error');
    }
  };

  const handleArchiveCargo = async (id: string) => {
    try {
      await api.cargos.archive(id);
      showToast('Груз архивирован', 'success');
      await loadMyListings();
    } catch (error) {
      console.error('Ошибка архивации:', error);
      showToast('Ошибка при архивации', 'error');
    }
  };

  const handleArchiveTruck = async (id: string) => {
    try {
      await api.trucks.archive(id);
      showToast('Машина архивирована', 'success');
      await loadMyListings();
    } catch (error) {
      console.error('Ошибка архивации:', error);
      showToast('Ошибка при архивации', 'error');
    }
  };

  const handleCopyCargo = async (id: string) => {
    try {
      await api.cargos.copy(id);
      showToast('Груз скопирован', 'success');
      await loadMyListings();
    } catch (error) {
      console.error('Ошибка копирования:', error);
      showToast('Ошибка при копировании', 'error');
    }
  };

  const handleCopyTruck = async (id: string) => {
    try {
      await api.trucks.copy(id);
      showToast('Машина скопирована', 'success');
      await loadMyListings();
    } catch (error) {
      console.error('Ошибка копирования:', error);
      showToast('Ошибка при копировании', 'error');
    }
  };

  const getStatusLabel = (status: ListingStatus | number | string | undefined | null): string => {
    const labels: Record<string, string> = {
      'Draft': 'Черновик',
      'Published': 'Опубликован',
      'Archived': 'В архиве',
      'Completed': 'Завершён',
      '0': 'Черновик',
      '1': 'Опубликован',
      '2': 'В архиве',
      '3': 'Завершён',
    };
    return labels[String(status)] || String(status);
  };

  const getStatusKey = (status: ListingStatus | number | string | undefined | null): string => {
    const map: Record<string, string> = {
      'Draft': 'draft',
      'Published': 'published',
      'Archived': 'archived',
      'Completed': 'completed',
      '0': 'draft',
      '1': 'published',
      '2': 'archived',
      '3': 'completed',
    };
    return map[String(status)] || 'draft';
  };

  const isPublished = (status: ListingStatus | number | string | undefined | null): boolean => {
    return status === ListingStatus.Published || status === 1 || String(status) === 'Published' || String(status) === '1';
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('ru-RU');
  };

  const formatPrice = (price?: number, currency: string = 'RUB') => {
    if (!price) return '—';
    const currencySymbol = currency === 'EUR' ? '€' : '₽';
    return `${price.toLocaleString('ru-RU')} ${currencySymbol}`;
  };

  if (isLoading) {
    return (
      <div className="my-listing">
        <div className="my-listing__container">
          <div className="my-listing__loading">Загрузка...</div>
        </div>
      </div>
    );
  }

  return (
    <div className="my-listing">
      {toast && (
        <Toast message={toast.message} type={toast.type} onClose={() => setToast(null)} />
      )}

      <div className="my-listing__container">
        <div className="my-listing__header">
          <h1 className="my-listing__title">Ваши грузы и машины</h1>
          <div className="my-listing__actions">
            <button 
              className="my-listing__btn my-listing__btn--add-cargo"
              onClick={() => navigate('/cargo/add')}
            >
              + Добавить груз
            </button>
            <button 
              className="my-listing__btn my-listing__btn--add-truck"
              onClick={() => navigate('/vehicles/add')}
            >
              + Добавить машину
            </button>
          </div>
        </div>

        <section className="my-listing__section">
          <h2 className="my-listing__section-title">Грузы</h2>
          
          <div className="my-listing__table-header">
            <div className="my-listing__th">Направление</div>
            <div className="my-listing__th">Загрузка</div>
            <div className="my-listing__th">Разгрузка</div>
            <div className="my-listing__th">Вес/объём, груз</div>
            <div className="my-listing__th">Транспорт</div>
            <div className="my-listing__th">Ставка</div>
          </div>

          {cargos.length === 0 ? (
            <div className="my-listing__empty">
              <p>У вас пока нет грузов</p>
              <button 
                className="my-listing__empty-btn"
                onClick={() => navigate('/cargo/add')}
              >
                + Добавить первый груз
              </button>
            </div>
          ) : (
            <div className="my-listing__list">
              {cargos.map((cargo) => (
                <div key={cargo.id} className="my-listing__card">
                  <div className="my-listing__card-row">
                    <div className="my-listing__card-cell">
                      <div className="my-listing__country">
                        {cargo.routeFrom?.slice(0, 3).toUpperCase() || '—'}
                      </div>
                      <div className="my-listing__distance">
                        {cargo.routeFrom && cargo.routeTo ? '704 км' : '—'}
                      </div>
                    </div>

                    <div className="my-listing__card-cell">
                      <div className="my-listing__city">{cargo.routeFrom || '—'}</div>
                      <div className="my-listing__address">
                        {formatDate(cargo.loadDateTime)}
                      </div>
                    </div>

                    <div className="my-listing__card-cell">
                      <div className="my-listing__city">{cargo.routeTo || '—'}</div>
                      <div className="my-listing__address">
                        {formatDate(cargo.unloadDateTime)}
                      </div>
                    </div>

                    <div className="my-listing__card-cell">
                      <div className="my-listing__weight">
                        {cargo.weightTons}т/{cargo.volumeM3}м³
                      </div>
                      <div className="my-listing__cargo-name">
                        {cargo.cargoName || '—'}
                      </div>
                    </div>

                    <div className="my-listing__card-cell">
                      <div className="my-listing__transport-type">
                        {cargo.bodyTypeRequired || '—'}
                      </div>
                      <div className="my-listing__transport-note">
                        {cargo.loadingType || ''}
                      </div>
                    </div>

                    <div className="my-listing__card-cell">
                      <div className="my-listing__price">
                        {formatPrice(cargo.startingPrice)}
                      </div>
                      <div className="my-listing__price-note">
                        {cargo.allowBargaining ? 'Торг' : 'Без торга'}
                      </div>
                    </div>
                  </div>

                  <div className="my-listing__card-actions">
                    <button 
                      className="my-listing__action-btn my-listing__action-btn--edit"
                      onClick={() => navigate(`/cargo/${cargo.id}/edit`)}
                    >
                      Редактировать
                    </button>
                    {isPublished(cargo.status) ? (
                      <button 
                        className="my-listing__action-btn my-listing__action-btn--archive"
                        onClick={() => handleArchiveCargo(cargo.id)}
                      >
                        Снять с публикации
                      </button>
                    ) : (
                      <button 
                        className="my-listing__action-btn my-listing__action-btn--publish"
                        onClick={() => navigate(`/cargo/${cargo.id}`)}
                      >
                        Опубликовать
                      </button>
                    )}
                    <button 
                      className="my-listing__action-btn my-listing__action-btn--copy"
                      onClick={() => handleCopyCargo(cargo.id)}
                    >
                      Копировать
                    </button>
                    <button 
                      className="my-listing__action-btn my-listing__action-btn--delete"
                      onClick={() => handleDeleteCargo(cargo.id)}
                    >
                      Удалить
                    </button>
                  </div>

                  <div className="my-listing__status">
                    <span className={`my-listing__status-badge my-listing__status-badge--${getStatusKey(cargo.status)}`}>
                      {getStatusLabel(cargo.status)}
                    </span>
                  </div>
                </div>
              ))}
            </div>
          )}
        </section>

        <section className="my-listing__section">
          <h2 className="my-listing__section-title">Машины</h2>
          
          <div className="my-listing__table-header">
            <div className="my-listing__th">Направление</div>
            <div className="my-listing__th">Загрузка</div>
            <div className="my-listing__th">Разгрузка</div>
            <div className="my-listing__th">Транспорт</div>
            <div className="my-listing__th">Ставка</div>
          </div>

          {trucks.length === 0 ? (
            <div className="my-listing__empty">
              <p>У вас пока нет машин</p>
              <button 
                className="my-listing__empty-btn"
                onClick={() => navigate('/vehicles/add')}
              >
                + Добавить первую машину
              </button>
            </div>
          ) : (
            <div className="my-listing__list">
              {trucks.map((truck) => (
                <div key={truck.id} className="my-listing__card">
                  <div className="my-listing__card-row">
                    <div className="my-listing__card-cell">
                      <div className="my-listing__country">
                        {truck.routeFrom?.slice(0, 3).toUpperCase() || '—'}
                      </div>
                      <div className="my-listing__distance">
                        {truck.routeFrom && truck.routeTo ? '328 км' : '—'}
                      </div>
                    </div>

                    <div className="my-listing__card-cell">
                      <div className="my-listing__city">{truck.routeFrom || '—'}</div>
                      <div className="my-listing__address">
                        {formatDate(truck.availableFrom)}
                      </div>
                    </div>

                    <div className="my-listing__card-cell">
                      <div className="my-listing__city">{truck.routeTo || '—'}</div>
                      <div className="my-listing__address">—</div>
                    </div>

                    <div className="my-listing__card-cell">
                      <div className="my-listing__transport-type">
                        {truck.capacityTons}т/{truck.volumeM3}м³
                      </div>
                      <div className="my-listing__cargo-name">
                        {truck.bodyType || '—'}
                      </div>
                    </div>

                    <div className="my-listing__card-cell">
                      <div className="my-listing__price">
                        {formatPrice(truck.price, 'EUR')}
                      </div>
                      <div className="my-listing__price-note">
                        {truck.allowBargaining ? 'Торг' : 'Без торга'}
                      </div>
                    </div>
                  </div>

                  <div className="my-listing__card-actions">
                    <button 
                      className="my-listing__action-btn my-listing__action-btn--edit"
                      onClick={() => navigate(`/vehicles/${truck.id}/edit`)}
                    >
                      Редактировать
                    </button>
                    {isPublished(truck.status) ? (
                      <button 
                        className="my-listing__action-btn my-listing__action-btn--archive"
                        onClick={() => handleArchiveTruck(truck.id)}
                      >
                        Снять с публикации
                      </button>
                    ) : (
                      <button 
                        className="my-listing__action-btn my-listing__action-btn--publish"
                        onClick={() => navigate(`/vehicles/${truck.id}`)}
                      >
                        Опубликовать
                      </button>
                    )}
                    <button 
                      className="my-listing__action-btn my-listing__action-btn--copy"
                      onClick={() => handleCopyTruck(truck.id)}
                    >
                      Копировать
                    </button>
                    <button 
                      className="my-listing__action-btn my-listing__action-btn--delete"
                      onClick={() => handleDeleteTruck(truck.id)}
                    >
                      Удалить
                    </button>
                  </div>

                  <div className="my-listing__status">
                    <span className={`my-listing__status-badge my-listing__status-badge--${getStatusKey(truck.status)}`}>
                      {getStatusLabel(truck.status)}
                    </span>
                  </div>
                </div>
              ))}
            </div>
          )}
        </section>
      </div>
    </div>
  );
};
