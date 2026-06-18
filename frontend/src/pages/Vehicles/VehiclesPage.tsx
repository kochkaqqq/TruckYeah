import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { api, TruckResponse, ContextType } from '../../api/client';
import { useAuthStore } from '../../store/authStore';
import './VehiclesPage.css';

export const VehiclesPage = () => {
  const navigate = useNavigate();
  const { isAuthenticated } = useAuthStore();
  
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

    try {
      const owner = await api.users.getCurrent(truck.userId);
      alert(`Контакты владельца:\n\n${owner.fullName || owner.email}\nТелефон: ${owner.phone || 'Не указан'}`);
    } catch (error) {
      console.error('Ошибка получения данных владельца:', error);
      alert('Не удалось загрузить данные владельца');
    }
  };

  const handleChatClick = async (truck: TruckResponse) => {
    if (!isAuthenticated) {
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
      alert('Не удалось создать чат');
    }
  };

  const formatPrice = (price?: number) => {
    if (!price) return 'Договорная';
    return `${price.toLocaleString('ru-RU')} €`;
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('ru-RU');
  };

  return (
    <div className="vehicles-page">
      <div className="vehicles-page__container">
        <div className="vehicles-page__header">
          <h1 className="vehicles-page__title">Машины</h1>
          {isAuthenticated && (
            <button 
              className="vehicles-page__add-btn"
              onClick={() => navigate('/vehicles/add')}
            >
              + Добавить машину
            </button>
          )}
        </div>

        <form className="vehicles-page__filters" onSubmit={handleSearch}>
          <div className="vehicles-page__filter-row">
            <div className="vehicles-page__filter-group">
              <label className="vehicles-page__label">Точка отправления</label>
              <input
                type="text"
                name="RouteFrom"
                className="vehicles-page__input"
                placeholder="Введите адрес отправления"
                value={filters.RouteFrom}
                onChange={handleFilterChange}
              />
            </div>

            <div className="vehicles-page__filter-group">
              <label className="vehicles-page__label">Точка назначения</label>
              <input
                type="text"
                name="RouteTo"
                className="vehicles-page__input"
                placeholder="Введите адрес назначения"
                value={filters.RouteTo}
                onChange={handleFilterChange}
              />
            </div>
          </div>

          <div className="vehicles-page__filter-row">
            <div className="vehicles-page__filter-group">
              <label className="vehicles-page__label">Грузоподъёмность от (т)</label>
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
              <label className="vehicles-page__label">Грузоподъёмность до (т)</label>
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
              <label className="vehicles-page__label">Объём от (м³)</label>
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
              <label className="vehicles-page__label">Объём до (м³)</label>
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
              <label className="vehicles-page__label">Дата доступности</label>
              <input
                type="date"
                name="AvailableDate"
                className="vehicles-page__input"
                value={filters.AvailableDate}
                onChange={handleFilterChange}
              />
            </div>

            <div className="vehicles-page__filter-group">
              <label className="vehicles-page__label">Тип кузова</label>
              <input
                type="text"
                name="BodyType"
                className="vehicles-page__input"
                placeholder="Любой"
                value={filters.BodyType}
                onChange={handleFilterChange}
              />
            </div>

            <div className="vehicles-page__filter-group">
              <label className="vehicles-page__label">Тип загрузки</label>
              <select
                name="LoadingType"
                className="vehicles-page__input vehicles-page__select"
                value={filters.LoadingType}
                onChange={handleFilterChange}
              >
                <option value="">Любой</option>
                <option value="Rear">Задняя</option>
                <option value="Side">Боковая</option>
                <option value="Top">Верхняя</option>
                <option value="FullAccess">Полный доступ</option>
              </select>
            </div>
          </div>

          <div className="vehicles-page__filter-actions">
            <button type="submit" className="vehicles-page__btn vehicles-page__btn--primary">
              🔍 Поиск
            </button>
            <button 
              type="button" 
              className="vehicles-page__btn vehicles-page__btn--secondary"
              onClick={handleClearFilters}
            >
              Сбросить
            </button>
          </div>
        </form>

        <div className="vehicles-page__results">
          <div className="vehicles-page__filter-row">
            <h2 className="vehicles-page__results-title">Результаты поиска</h2>
            <select className="vehicles-page__input" value={sortBy} onChange={(event) => setSortBy(event.target.value)}>
              <option value="createdAt">По дате</option>
              <option value="price">По цене</option>
              <option value="capacity">По грузоподъёмности</option>
              <option value="volume">По объёму</option>
            </select>
            <select className="vehicles-page__input" value={sortDirection} onChange={(event) => setSortDirection(event.target.value)}>
              <option value="desc">По убыванию</option>
              <option value="asc">По возрастанию</option>
            </select>
            <button className="vehicles-page__btn vehicles-page__btn--secondary" onClick={() => void loadTrucks(1)}>
              Применить
            </button>
          </div>

          {isLoading ? (
            <div className="vehicles-page__loading">Загрузка...</div>
          ) : trucks.length === 0 ? (
            <div className="vehicles-page__empty">
              <p>Машины не найдены</p>
            </div>
          ) : (
            <div className="vehicles-page__list">
              {trucks.map((truck) => (
                <div key={truck.id} className="truck-card">
                  <div className="truck-card__header">
                    <div className="truck-card__route">
                      <span className="truck-card__from">{truck.routeFrom || 'Не указан'}</span>
                      <span className="truck-card__arrow">→</span>
                      <span className="truck-card__to">{truck.routeTo || 'Не указан'}</span>
                    </div>
                    <div className="truck-card__date">
                      📅 {formatDate(truck.availableFrom)}
                    </div>
                  </div>

                  <div className="truck-card__details">
                    <div className="truck-card__detail">
                      <span className="truck-card__detail-label">Грузоподъёмность:</span>
                      <span className="truck-card__detail-value">{truck.capacityTons} т</span>
                    </div>
                    <div className="truck-card__detail">
                      <span className="truck-card__detail-label">Объём:</span>
                      <span className="truck-card__detail-value">{truck.volumeM3} м³</span>
                    </div>
                    <div className="truck-card__detail">
                      <span className="truck-card__detail-label">Кузов:</span>
                      <span className="truck-card__detail-value">{truck.bodyType || 'Любой'}</span>
                    </div>
                  </div>

                  <div className="truck-card__price">
                    {formatPrice(truck.price)}
                  </div>

                  <div className="truck-card__actions">
                    <button 
                      className="truck-card__btn truck-card__btn--contacts"
                      onClick={() => handleContactClick(truck)}
                    >
                      Контакты
                    </button>
                    <button 
                      className="truck-card__btn truck-card__btn--chat"
                      onClick={() => handleChatClick(truck)}
                    >
                      Написать в чат
                    </button>
                  </div>
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
              Назад
            </button>
            <span>Страница {page}</span>
            <button
              className="vehicles-page__btn vehicles-page__btn--secondary"
              disabled={trucks.length < pageSize || isLoading}
              onClick={() => {
                const nextPage = page + 1;
                setPage(nextPage);
                void loadTrucks(nextPage);
              }}
            >
              Далее
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};
