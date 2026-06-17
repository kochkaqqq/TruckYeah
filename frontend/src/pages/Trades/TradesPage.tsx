import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { api, CargoResponse, CargoBidResponse, ContextType } from '../../api/client';
import { useAuthStore } from '../../store/authStore';
import { Toast } from '../../components/ui/Toast';
import './TradesPage.css';

type ToastType = 'success' | 'error' | 'info';

interface ToastData {
  message: string;
  type: ToastType;
}

interface CargoWithBids {
  cargo: CargoResponse;
  bids: CargoBidResponse[];
}

export const TradesPage = () => {
  const navigate = useNavigate();
  const { isAuthenticated } = useAuthStore();
  const [cargosWithBids, setCargosWithBids] = useState<CargoWithBids[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [toast, setToast] = useState<ToastData | null>(null);

  useEffect(() => {
    if (!isAuthenticated) {
      navigate('/auth');
      return;
    }
    loadMyCargosWithBids();
  }, [isAuthenticated, navigate]);

  const loadMyCargosWithBids = async () => {
    try {
      setIsLoading(true);
      const myCargos = await api.cargos.getMyCargos();
      
      const cargosWithBidsData = await Promise.all(
        myCargos
          .filter(cargo => cargo.biddingEnabled && cargo.status === 'Published')
          .map(async (cargo) => {
            const bids = await api.cargos.getBids(cargo.id);
            return { cargo, bids };
          })
      );

      setCargosWithBids(cargosWithBidsData);
    } catch (error) {
      console.error('Ошибка загрузки торгов:', error);
      showToast('Не удалось загрузить данные', 'error');
    } finally {
      setIsLoading(false);
    }
  };

  const showToast = (message: string, type: ToastType) => {
    setToast({ message, type });
  };

  const handleAcceptBid = async (cargoId: string, bidId: string) => {
    if (!confirm('Вы уверены, что хотите принять эту ставку?')) return;

    try {
      await api.cargos.acceptBid(cargoId, bidId);
      showToast('Ставка принята!', 'success');
      await loadMyCargosWithBids();
    } catch (error) {
      console.error('Ошибка принятия ставки:', error);
      showToast('Ошибка при принятии ставки', 'error');
    }
  };

  const handleChatClick = async (cargoId: string, userId: string) => {
    try {
      const chat = await api.chats.create({
        contextType: ContextType.Cargo,
        contextId: cargoId,
        recipientUserId: userId,
      });
      navigate(`/chats/${chat.id}`);
    } catch (error) {
      console.error('Ошибка создания чата:', error);
      showToast('Не удалось создать чат', 'error');
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('ru-RU', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  const formatPrice = (price: number) => {
    return `${price.toLocaleString('ru-RU')}€`;
  };

  const getUserName = (bid: CargoBidResponse) => {
    // Здесь нужно будет получать имя пользователя из API
    // Пока заглушка
    return `Пользователь ${bid.carrierUserId.slice(0, 8)}...`;
  };

  if (isLoading) {
    return (
      <div className="trades">
        <div className="trades__container">
          <div className="trades__loading">Загрузка...</div>
        </div>
      </div>
    );
  }

  return (
    <div className="trades">
      {toast && (
        <Toast message={toast.message} type={toast.type} onClose={() => setToast(null)} />
      )}

      <div className="trades__container">
        <h1 className="trades__title">Торги</h1>

        {cargosWithBids.length === 0 ? (
          <div className="trades__empty">
            <p>У вас пока нет активных торгов</p>
          </div>
        ) : (
          <div className="trades__list">
            {cargosWithBids.map(({ cargo, bids }) => (
              <div key={cargo.id} className="trades__cargo">
                {/* Информация о грузе */}
                <div className="trades__cargo-info">
                  <div className="trades__route">
                    <span className="trades__from">{cargo.routeFrom}</span>
                    <span className="trades__arrow">→</span>
                    <span className="trades__to">{cargo.routeTo}</span>
                  </div>
                  <div className="trades__meta">
                    {formatDate(cargo.loadDateTime)} • {cargo.weightTons} т • {cargo.volumeM3} м³ • {cargo.bodyTypeRequired || 'Любой кузов'}
                  </div>
                  <div className="trades__price">
                    {formatPrice(cargo.startingPrice || 0)}
                    {cargo.minBidStep && (
                      <span className="trades__step">
                        (Шаг ставки - {formatPrice(cargo.minBidStep)})
                      </span>
                    )}
                  </div>
                </div>

                {/* Ставки */}
                {bids.length === 0 ? (
                  <div className="trades__no-bids">
                    Пока нет ставок
                  </div>
                ) : (
                  <div className="trades__bids">
                    {bids
                      .filter(bid => bid.status === 'Active')
                      .sort((a, b) => b.price - a.price)
                      .map((bid) => (
                        <div key={bid.id} className="trades__bid">
                          <div className="trades__bid-header">
                            <div className="trades__bid-user">
                              {getUserName(bid)}
                            </div>
                            <div className="trades__bid-time">
                              Ставка сделана {formatDate(bid.createdAt)}
                            </div>
                          </div>
                          <div className="trades__bid-footer">
                            <div className="trades__bid-price">
                              {formatPrice(bid.price)}
                            </div>
                            <div className="trades__bid-actions">
                              <button
                                className="trades__btn trades__btn--chat"
                                onClick={() => handleChatClick(cargo.id, bid.carrierUserId)}
                              >
                                Написать в чат
                              </button>
                              <button
                                className="trades__btn trades__btn--accept"
                                onClick={() => handleAcceptBid(cargo.id, bid.id)}
                              >
                                Принять ставку
                              </button>
                            </div>
                          </div>
                        </div>
                      ))}
                  </div>
                )}
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};