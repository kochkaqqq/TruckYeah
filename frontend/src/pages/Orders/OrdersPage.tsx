import { useEffect, useState } from 'react';
import { api, BidStatus, CargoResponse } from '../../api/client';
import './OrdersPage.css';

interface CarrierOrder {
  cargo: CargoResponse;
  price: number;
  acceptedAt?: string | null;
}

export const OrdersPage = () => {
  const [ownerOrders, setOwnerOrders] = useState<CargoResponse[]>([]);
  const [carrierOrders, setCarrierOrders] = useState<CarrierOrder[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const loadOrders = async () => {
      try {
        const [myCargos, myBids] = await Promise.all([
          api.cargos.getMyCargos(),
          api.cargos.getMyBids(),
        ]);

        setOwnerOrders(myCargos.filter((cargo) => Boolean(cargo.acceptedBidId)));

        const acceptedBids = myBids.filter((bid) => bid.status === BidStatus.Accepted);
        const acceptedOrders = await Promise.all(
          acceptedBids.map(async (bid) => ({
            cargo: await api.cargos.getById(bid.cargoId),
            price: bid.price,
            acceptedAt: bid.acceptedAt,
          }))
        );
        setCarrierOrders(acceptedOrders);
      } finally {
        setIsLoading(false);
      }
    };

    void loadOrders();
  }, []);

  if (isLoading) {
    return <div className="orders-page"><div className="orders-page__container">Загрузка заказов...</div></div>;
  }

  const renderCargo = (cargo: CargoResponse, price?: number) => (
    <article className="orders-page__card" key={`${cargo.id}-${price ?? 'owner'}`}>
      <h3>{cargo.title}</h3>
      <p>{cargo.routeFrom} → {cargo.routeTo}</p>
      <p>{cargo.weightTons} т · {cargo.volumeM3} м³</p>
      <strong>{(price ?? cargo.startingPrice ?? 0).toLocaleString('ru-RU')} ₽</strong>
    </article>
  );

  return (
    <div className="orders-page">
      <div className="orders-page__container">
        <h1>Ваши заказы</h1>

        <section>
          <h2>Я заказчик</h2>
          <div className="orders-page__grid">
            {ownerOrders.length ? ownerOrders.map((cargo) => renderCargo(cargo)) : <p>Принятых ставок пока нет.</p>}
          </div>
        </section>

        <section>
          <h2>Я перевозчик</h2>
          <div className="orders-page__grid">
            {carrierOrders.length
              ? carrierOrders.map(({ cargo, price }) => renderCargo(cargo, price))
              : <p>Ваши ставки пока не были приняты.</p>}
          </div>
        </section>
      </div>
    </div>
  );
};
