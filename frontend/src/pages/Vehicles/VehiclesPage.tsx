import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { api, TruckResponse, ContextType } from '../../api/client';
import { useAuthStore } from '../../store/authStore';
import './VehiclesPage.css';

export const VehiclesPage = () => {
  const navigate = useNavigate();
  const { isAuthenticated, currentUser } = useAuthStore();
  const { t } = useTranslation(); // ← Добавили перевод
  
  const [trucks, setTrucks] = useState<TruckResponse[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [page, setPage] = useState(1);
  const [sortBy, setSortBy] = useState('createdAt');
  const [sortDirection, setSortDirection] = useState('desc');
  const pageSize = 12;
  
  const [filters, setFilters] = useState({
    RouteFrom: '',
    RouteTo: '',
    CapacityFrom: '',
    CapacityTo: '',
    VolumeFrom: '',
    VolumeTo: '',
    BodyType: '',
    LoadingType: '',
    AvailableDate: '',
  });

  useEffect(() => {
    loadTrucks();
  }, []);

  const loadTrucks = async (requestedPage = page) => {
    try {
      setIsLoading(true);
      const params: any = {
        Page: requestedPage,
        PageSize: pageSize,
        SortBy: sortBy,
        SortDirection: sortDirection,
      };

      Object.entries(filters).forEach(([key, value]) => {
        if (value !== '' && value !== null && value !== undefined) {
          params[key] = value;
        }
      });

      console.log('🔍 Параметры поиска машин:', params);
      const data = await api.trucks.search(params);
      console.log('🚛 Получено машин:', data.length);
      setTrucks(data);
    } catch (error) {
      console.error('Ошибка загрузки машин:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleFilterChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFilters((prev) => ({ ...prev, [name]: value }));
  };

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    setPage(1);
    void loadTrucks(1);
  };

  const handleClearFilters = () => {
    setFilters({
      RouteFrom: '',
      RouteTo: '',
      CapacityFrom: '',
      CapacityTo: '',
      VolumeFrom: '',
      VolumeTo: '',
      BodyType: '',
      LoadingType: '',
      AvailableDate: '',
    });
    loadTrucks();
  };

  const handleContactClick = async (truck: TruckResponse) => {
    if (!isAuthenticated) {
      navigate('/auth');
      return;
    }
    if (currentUser?.id === truck.userId) return;

    try {
      const owner = await api.users.getCurrent(truck.userId);
      alert(`${t('vehicle.ownerContacts')}\n\n${owner.fullName || owner.email}\n${t('vehicle.phone')}: ${owner.phone || t('vehicle.notSpecified')}`);
    } catch (error) {
      console.error('Ошибка получения данных владельца:', error);
      alert(t('vehicle.failedToLoadOwner'));
    }
  };

  const handleChatClick = async (truck: TruckResponse) => {
    if (!isAuthenticated) {
      navigate('/auth');
      return;
    }
    if (currentUser?.id === truck.userId) return;

    try {
      const chat = await api.chats.create({
        contextType: ContextType.Truck,
        contextId: truck.id,
        recipientUserId: truck.userId,
      });

      navigate(`/chats/${chat.id}`);
    } catch (error) {
      console.error('Ошибка создания чата:', error);
      alert(t('vehicle.failedToCreateChat'));
    }
  };

  const formatPrice = (price?: number) => {
    if (!price) return t('vehicle.negotiable');
    return `${price.toLocaleString('ru-RU')} €`;
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('ru-RU');
  };

  return (
    <div className="vehicles-page">
      <div className="vehicles-page__container">
        <div className="vehicles-page__header">
          <h1 className="vehicles-page__title">{t('vehicle.title')}</h1>
          {isAuthenticated && (
            <button 
              className="vehicles-page__add-btn"
              onClick={() => navigate('/vehicles/add')}
            >
              + {t('vehicle.add')}
            </button>
          )}
        </div>

        <form className="vehicles-page__filters" onSubmit={handleSearch}>
          <div className="vehicles-page__filter-row">
            <div className="vehicles-page__filter-group">
              <label className="vehicles-page__label">{t('vehicle.routeFrom')}</label>
              <input
                type="text"
                name="RouteFrom"
                className="vehicles-page__input"
                placeholder={t('vehicle.routeFromPlaceholder')}
                value={filters.RouteFrom}
                onChange={handleFilterChange}
              />
            </div>

            <div className="vehicles-page__filter-group">
              <label className="vehicles-page__label">{t('vehicle.routeTo')}</label>
              <input
                type="text"
                name="RouteTo"
                className="vehicles-page__input"
                placeholder={t('vehicle.routeToPlaceholder')}
                value={filters.RouteTo}
                onChange={handleFilterChange}
              />
            </div>
          </div>

          <div className="vehicles-page__filter-row">
            <div className="vehicles-page__filter-group">
              <label className="vehicles-page__label">{t('vehicle.capacityFrom')}</label>
              <input
                type="number"
                name="CapacityFrom"
                className="vehicles-page__input"
                placeholder="0"
                step="0.1"
                value={filters.CapacityFrom}
                onChange={handleFilterChange}
              />
            </div>

            <div className="vehicles-page__filter-group">
              <label className="vehicles-page__label">{t('vehicle.capacityTo')}</label>
              <input
                type="number"
                name="CapacityTo"
                className="vehicles-page__input"
                placeholder="100"
                step="0.1"
                value={filters.CapacityTo}
                onChange={handleFilterChange}
              />
            </div>

            <div className="vehicles-page__filter-group">
              <label className="vehicles-page__label">{t('vehicle.volumeFrom')}</label>
              <input
                type="number"
                name="VolumeFrom"
                className="vehicles-page__input"
                placeholder="0"
                step="0.1"
                value={filters.VolumeFrom}
                onChange={handleFilterChange}
              />
            </div>

            <div className="vehicles-page__filter-group">
              <label className="vehicles-page__label">{t('vehicle.volumeTo')}</label>
              <input
                type="number"
                name="VolumeTo"
                className="vehicles-page__input"
                placeholder="1000"
                step="0.1"
                value={filters.VolumeTo}
                onChange={handleFilterChange}
              />
            </div>
          </div>

          <div className="vehicles-page__filter-row">
            <div className="vehicles-page__filter-group">
              <label className="vehicles-page__label">{t('vehicle.availableDate')}</label>
              <input
                type="date"
                name="AvailableDate"
                className="vehicles-page__input"
                value={filters.AvailableDate}
                onChange={handleFilterChange}
              />
            </div>

            <div className="vehicles-page__filter-group">
              <label className="vehicles-page__label">{t('vehicle.bodyType')}</label>
              <input
                type="text"
                name="BodyType"
                className="vehicles-page__input"
                placeholder={t('vehicle.any')}
                value={filters.BodyType}
                onChange={handleFilterChange}
              />
            </div>

            <div className="vehicles-page__filter-group">
              <label className="vehicles-page__label">{t('vehicle.loadingType')}</label>
              <select
                name="LoadingType"
                className="vehicles-page__input vehicles-page__select"
                value={filters.LoadingType}
                onChange={handleFilterChange}
              >
                <option value="">{t('vehicle.any')}</option>
                <option value="Rear">{t('vehicle.loadingRear')}</option>
                <option value="Side">{t('vehicle.loadingSide')}</option>
                <option value="Top">{t('vehicle.loadingTop')}</option>
                <option value="FullAccess">{t('vehicle.loadingFullAccess')}</option>
              </select>
            </div>
          </div>

          <div className="vehicles-page__filter-actions">
            <button type="submit" className="vehicles-page__btn vehicles-page__btn--primary">
              🔍 {t('common.search')}
            </button>
            <button 
              type="button" 
              className="vehicles-page__btn vehicles-page__btn--secondary"
              onClick={handleClearFilters}
            >
              {t('common.reset')}
            </button>
          </div>
        </form>

        <div className="vehicles-page__results">
          <div className="vehicles-page__filter-row">
            <h2 className="vehicles-page__results-title">{t('vehicle.searchResults')}</h2>
            <select className="vehicles-page__input" value={sortBy} onChange={(event) => setSortBy(event.target.value)}>
              <option value="createdAt">{t('vehicle.sortByDate')}</option>
              <option value="price">{t('vehicle.sortByPrice')}</option>
              <option value="capacity">{t('vehicle.sortByCapacity')}</option>
              <option value="volume">{t('vehicle.sortByVolume')}</option>
            </select>
            <select className="vehicles-page__input" value={sortDirection} onChange={(event) => setSortDirection(event.target.value)}>
              <option value="desc">{t('vehicle.sortDescending')}</option>
              <option value="asc">{t('vehicle.sortAscending')}</option>
            </select>
            <button className="vehicles-page__btn vehicles-page__btn--secondary" onClick={() => void loadTrucks(1)}>
              {t('common.apply')}
            </button>
          </div>

          {isLoading ? (
            <div className="vehicles-page__loading">{t('common.loading')}</div>
          ) : trucks.length === 0 ? (
            <div className="vehicles-page__empty">
              <p>{t('vehicle.noVehicles')}</p>
            </div>
          ) : (
            <div className="vehicles-page__list">
              {trucks.map((truck) => (
                <div key={truck.id} className="truck-card">
                  <div className="truck-card__header">
                    <div className="truck-card__route">
                      <span className="truck-card__from">{truck.routeFrom || t('vehicle.notSpecified')}</span>
                      <span className="truck-card__arrow">→</span>
                      <span className="truck-card__to">{truck.routeTo || t('vehicle.notSpecified')}</span>
                    </div>
                    <div className="truck-card__date">
                      📅 {formatDate(truck.availableFrom)}
                    </div>
                  </div>

                  <div className="truck-card__details">
                    <div className="truck-card__detail">
                      <span className="truck-card__detail-label">{t('vehicle.capacity')}:</span>
                      <span className="truck-card__detail-value">{truck.capacityTons} т</span>
                    </div>
                    <div className="truck-card__detail">
                      <span className="truck-card__detail-label">{t('vehicle.volume')}:</span>
                      <span className="truck-card__detail-value">{truck.volumeM3} м³</span>
                    </div>
                    <div className="truck-card__detail">
                      <span className="truck-card__detail-label">{t('vehicle.bodyType')}:</span>
                      <span className="truck-card__detail-value">{truck.bodyType || t('vehicle.any')}</span>
                    </div>
                  </div>

                  <div className="truck-card__price">
                    {formatPrice(truck.price)}
                  </div>

                  {currentUser?.id !== truck.userId && (
                    <div className="truck-card__actions">
                      <button
                        className="truck-card__btn truck-card__btn--contacts"
                        onClick={() => handleContactClick(truck)}
                      >
                        {t('vehicle.contacts')}
                      </button>
                      <button
                        className="truck-card__btn truck-card__btn--chat"
                        onClick={() => handleChatClick(truck)}
                      >
                        {t('vehicle.writeToChat')}
                      </button>
                    </div>
                  )}
                </div>
              ))}
            </div>
          )}
          <div className="vehicles-page__filter-actions">
            <button
              className="vehicles-page__btn vehicles-page__btn--secondary"
              disabled={page === 1 || isLoading}
              onClick={() => {
                const nextPage = page - 1;
                setPage(nextPage);
                void loadTrucks(nextPage);
              }}
            >
              {t('common.back')}
            </button>
            <span>{t('vehicle.page')} {page}</span>
            <button
              className="vehicles-page__btn vehicles-page__btn--secondary"
              disabled={trucks.length < pageSize || isLoading}
              onClick={() => {
                const nextPage = page + 1;
                setPage(nextPage);
                void loadTrucks(nextPage);
              }}
            >
              {t('common.next')}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};
