import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { api, CargoResponse, ContextType } from '../../api/client';
import { useAuthStore } from '../../store/authStore';
import './CargoPage.css';



export const CargoPage = () => {
  const navigate = useNavigate();
  const { isAuthenticated } = useAuthStore();
  
  const [cargos, setCargos] = useState<CargoResponse[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  
  const [filters, setFilters] = useState({
    RouteFrom: '',
    RouteTo: '',
    CargoName: '',
    WeightFrom: '',
    WeightTo: '',
    VolumeFrom: '',
    VolumeTo: '',
    BodyType: '',
    LoadingType: '',
    LoadDate: '',
  });

  const [showBidModal, setShowBidModal] = useState(false);
  const [selectedCargo, setSelectedCargo] = useState<CargoResponse | null>(null);
  const [bidPrice, setBidPrice] = useState('');

  useEffect(() => {
    loadCargos();
  }, []);

  const loadCargos = async () => {
    try {
      setIsLoading(true);
      const params: any = {
        Page: 1,
        PageSize: 50,
      };

      Object.entries(filters).forEach(([key, value]) => {
        if (value !== '' && value !== null && value !== undefined) {
          params[key] = value;
        }
      });

      console.log('🔍 Параметры поиска:', params);
      const data = await api.cargos.search(params);
      console.log('📦 Получено грузов:', data.length);
      setCargos(data);
    } catch (error) {
      console.error('Ошибка загрузки грузов:', error);
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
    loadCargos();
  };

  const handleClearFilters = () => {
    setFilters({
      RouteFrom: '',
      RouteTo: '',
      CargoName: '',
      WeightFrom: '',
      WeightTo: '',
      VolumeFrom: '',
      VolumeTo: '',
      BodyType: '',
      LoadingType: '',
      LoadDate: '',
    });
    loadCargos();
  };

  const handleBidClick = (cargo: CargoResponse) => {
    if (!isAuthenticated) {
      navigate('/auth');
      return;
    }
    setSelectedCargo(cargo);
    setBidPrice(cargo.startingPrice?.toString() || '');
    setShowBidModal(true);
  };

  const handleSubmitBid = async () => {
    if (!selectedCargo || !bidPrice) return;

    const price = parseFloat(bidPrice);
    if (isNaN(price) || price < 0) {
      alert('Введите корректную цену');
      return;
    }

    try {
      await api.cargos.createBid(selectedCargo.id, { price });
      alert('✅ Ставка успешно создана!');
      setShowBidModal(false);
      setBidPrice('');
    } catch (error: any) {
      console.error('Ошибка создания ставки:', error);
      
      let errorMessage = 'Ошибка при создании ставки';
      if (error.message?.includes('lower than')) {
        errorMessage = '💡 Цена ставки должна быть минимум на 1 ₽ меньше текущей лучшей цены';
      } else if (error.message?.includes('own cargo')) {
        errorMessage = '❌ Нельзя делать ставку на собственный груз';
      } else if (error.message) {
        errorMessage = error.message;
      }
      
      alert(errorMessage);
    }
  };

  const handleContactClick = async (cargo: CargoResponse) => {
    if (!isAuthenticated) {
      navigate('/auth');
      return;
    }

    try {
      const owner = await api.users.getCurrent(cargo.userId);
      alert(`Контакты владельца:\n\n${owner.fullName || owner.email}\nТелефон: ${owner.phone || 'Не указан'}`);
    } catch (error) {
      console.error('Ошибка получения данных владельца:', error);
      alert('Не удалось загрузить данные владельца');
    }
  };

  const handleChatClick = async (cargo: CargoResponse) => {
    if (!isAuthenticated) {
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
      alert('Не удалось создать чат');
    }
  };

  const formatPrice = (price?: number) => {
    if (!price) return 'Договорная';
    return `${price.toLocaleString('ru-RU')} ₽`;
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('ru-RU');
  };

  return (
    <div className="cargo-page">
      <div className="cargo-page__container">
        <div className="cargo-page__header">
          <h1 className="cargo-page__title">Грузы</h1>
          {isAuthenticated && (
            <button 
              className="cargo-page__add-btn"
              onClick={() => navigate('/cargo/add')}
            >
              + Добавить груз
            </button>
          )}
        </div>

        <form className="cargo-page__filters" onSubmit={handleSearch}>
          <div className="cargo-page__filter-row">
            <div className="cargo-page__filter-group">
              <label className="cargo-page__label">Точка отправления</label>
              <input
                type="text"
                name="RouteFrom"
                className="cargo-page__input"
                placeholder="Введите адрес отправления"
                value={filters.RouteFrom}
                onChange={handleFilterChange}
              />
            </div>

            <div className="cargo-page__filter-group">
              <label className="cargo-page__label">Точка назначения</label>
              <input
                type="text"
                name="RouteTo"
                className="cargo-page__input"
                placeholder="Введите адрес назначения"
                value={filters.RouteTo}
                onChange={handleFilterChange}
              />
            </div>
          </div>

          <div className="cargo-page__filter-row">
            <div className="cargo-page__filter-group">
              <label className="cargo-page__label">Наименование груза</label>
              <input
                type="text"
                name="CargoName"
                className="cargo-page__input"
                placeholder="Введите наименование груза"
                value={filters.CargoName}
                onChange={handleFilterChange}
              />
            </div>

            <div className="cargo-page__filter-group">
              <label className="cargo-page__label">Дата погрузки</label>
              <input
                type="date"
                name="LoadDate"
                className="cargo-page__input"
                value={filters.LoadDate}
                onChange={handleFilterChange}
              />
            </div>
          </div>

          <div className="cargo-page__filter-row">
            <div className="cargo-page__filter-group">
              <label className="cargo-page__label">Вес от (т)</label>
              <input
                type="number"
                name="WeightFrom"
                className="cargo-page__input"
                placeholder="0"
                step="0.1"
                value={filters.WeightFrom}
                onChange={handleFilterChange}
              />
            </div>

            <div className="cargo-page__filter-group">
              <label className="cargo-page__label">Вес до (т)</label>
              <input
                type="number"
                name="WeightTo"
                className="cargo-page__input"
                placeholder="100"
                step="0.1"
                value={filters.WeightTo}
                onChange={handleFilterChange}
              />
            </div>

            <div className="cargo-page__filter-group">
              <label className="cargo-page__label">Объём от (м³)</label>
              <input
                type="number"
                name="VolumeFrom"
                className="cargo-page__input"
                placeholder="0"
                step="0.1"
                value={filters.VolumeFrom}
                onChange={handleFilterChange}
              />
            </div>

            <div className="cargo-page__filter-group">
              <label className="cargo-page__label">Объём до (м³)</label>
              <input
                type="number"
                name="VolumeTo"
                className="cargo-page__input"
                placeholder="1000"
                step="0.1"
                value={filters.VolumeTo}
                onChange={handleFilterChange}
              />
            </div>
          </div>

          <div className="cargo-page__filter-row">
            <div className="cargo-page__filter-group">
              <label className="cargo-page__label">Тип кузова</label>
              <input
                type="text"
                name="BodyType"
                className="cargo-page__input"
                placeholder="Любой"
                value={filters.BodyType}
                onChange={handleFilterChange}
              />
            </div>

            <div className="cargo-page__filter-group">
              <label className="cargo-page__label">Тип загрузки</label>
              <select
                name="LoadingType"
                className="cargo-page__input"
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

          <div className="cargo-page__filter-actions">
            <button type="submit" className="cargo-page__btn cargo-page__btn--primary">
              🔍 Поиск
            </button>
            <button 
              type="button" 
              className="cargo-page__btn cargo-page__btn--secondary"
              onClick={handleClearFilters}
            >
              Сбросить
            </button>
          </div>
        </form>

        <div className="cargo-page__results">
          <h2 className="cargo-page__results-title">Результаты поиска</h2>

          {isLoading ? (
            <div className="cargo-page__loading">Загрузка...</div>
          ) : cargos.length === 0 ? (
            <div className="cargo-page__empty">
              <p>Грузы не найдены</p>
            </div>
          ) : (
            <div className="cargo-page__list">
              {cargos.map((cargo) => (
                <div key={cargo.id} className="cargo-card">
                  <div className="cargo-card__header">
                    <div className="cargo-card__route">
                      <span className="cargo-card__from">{cargo.routeFrom || 'Не указан'}</span>
                      <span className="cargo-card__arrow">→</span>
                      <span className="cargo-card__to">{cargo.routeTo || 'Не указан'}</span>
                    </div>
                    <div className="cargo-card__date">
                      📅 {formatDate(cargo.loadDateTime)}
                    </div>
                  </div>

                  <div className="cargo-card__details">
                    <div className="cargo-card__detail">
                      <span className="cargo-card__detail-label">Вес:</span>
                      <span className="cargo-card__detail-value">{cargo.weightTons} т</span>
                    </div>
                    <div className="cargo-card__detail">
                      <span className="cargo-card__detail-label">Объём:</span>
                      <span className="cargo-card__detail-value">{cargo.volumeM3} м³</span>
                    </div>
                    <div className="cargo-card__detail">
                      <span className="cargo-card__detail-label">Кузов:</span>
                      <span className="cargo-card__detail-value">{cargo.bodyTypeRequired || 'Любой'}</span>
                    </div>
                    {cargo.cargoName && (
                      <div className="cargo-card__detail">
                        <span className="cargo-card__detail-label">Груз:</span>
                        <span className="cargo-card__detail-value">{cargo.cargoName}</span>
                      </div>
                    )}
                  </div>

                  <div className="cargo-card__price">
                    {formatPrice(cargo.startingPrice)}
                  </div>

                  <div className="cargo-card__actions">
                    <button 
                      className="cargo-card__btn cargo-card__btn--contacts"
                      onClick={() => handleContactClick(cargo)}
                    >
                      Контакты
                    </button>
                    <button 
                      className="cargo-card__btn cargo-card__btn--bid"
                      onClick={() => handleBidClick(cargo)}
                    >
                      Сделать ставку
                    </button>
                    <button 
                      className="cargo-card__btn cargo-card__btn--chat"
                      onClick={() => handleChatClick(cargo)}
                    >
                      Написать в чат
                    </button>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>

      {showBidModal && selectedCargo && (
        <div 
          className="cargo-page__modal"
          onClick={() => setShowBidModal(false)}
        >
          <div 
            className="cargo-page__modal-content"
            onClick={(e) => e.stopPropagation()}
          >
            <button 
              className="cargo-page__modal-close"
              onClick={() => setShowBidModal(false)}
            >
              ✕
            </button>
            
            <h2 className="cargo-page__modal-title">Сделать ставку</h2>
            
            <div className="cargo-page__modal-info">
              <p><strong>Маршрут:</strong> {selectedCargo.routeFrom} → {selectedCargo.routeTo}</p>
              <p><strong>Вес:</strong> {selectedCargo.weightTons} т</p>
              <p><strong>Объём:</strong> {selectedCargo.volumeM3} м³</p>
              {selectedCargo.startingPrice && (
                <p><strong>Начальная цена:</strong> {formatPrice(selectedCargo.startingPrice)}</p>
              )}
              <p className="cargo-page__modal-hint">
                💡 Ваша ставка должна быть минимум на 1 ₽ меньше текущей лучшей цены
              </p>
            </div>

            <div className="cargo-page__modal-field">
              <label className="cargo-page__label">Ваша ставка (₽)</label>
              <input
                type="number"
                className="cargo-page__input"
                value={bidPrice}
                onChange={(e) => setBidPrice(e.target.value)}
                placeholder="Введите сумму"
                step="1"
                min="0"
              />
            </div>

            <div className="cargo-page__modal-actions">
              <button 
                className="cargo-page__btn cargo-page__btn--secondary"
                onClick={() => setShowBidModal(false)}
              >
                Отмена
              </button>
              <button 
                className="cargo-page__btn cargo-page__btn--primary"
                onClick={handleSubmitBid}
                disabled={!bidPrice}
              >
                Отправить ставку
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};