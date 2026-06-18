import type { ReactNode } from 'react';
import { ListingStatus, type CargoResponse, type TruckResponse } from '../../api/client';

export const AdminPageHeader = ({
  title,
  subtitle,
  action,
}: {
  title: string;
  subtitle: string;
  action?: ReactNode;
}) => (
  <div className="admin-page-header">
    <div>
      <h1>{title}</h1>
      <p>{subtitle}</p>
    </div>
    {action}
  </div>
);

export const AdminNotice = ({
  type,
  text,
  onClose,
}: {
  type: 'success' | 'error';
  text: string;
  onClose: () => void;
}) => (
  <div className={`admin-alert admin-alert--${type}`}>
    <span>{text}</span>
    <button onClick={onClose} aria-label="Закрыть">×</button>
  </div>
);

export const AdminSkeleton = () => (
  <div className="admin-skeleton" aria-label="Загрузка">
    <i /><i /><i />
  </div>
);

export const StatusBadge = ({ status }: { status: ListingStatus }) => (
  <span className={`admin-status admin-status--${status.toLowerCase()}`}>
    {statusLabels[status]}
  </span>
);

export const ListingCard = ({
  listing,
  kind,
  busy,
  onApprove,
  onReject,
}: {
  listing: CargoResponse | TruckResponse;
  kind: 'cargo' | 'truck';
  busy: boolean;
  onApprove: () => void;
  onReject: () => void;
}) => {
  const isCargo = kind === 'cargo';
  const cargo = isCargo ? (listing as CargoResponse) : null;
  const truck = !isCargo ? (listing as TruckResponse) : null;
  const pending = listing.status === ListingStatus.PendingModeration;

  return (
    <article className="admin-listing-card">
      <div className="admin-listing-card__head">
        <div>
          <p className="admin-eyebrow">{isCargo ? 'Груз' : 'Машина'} · {shortId(listing.id)}</p>
          <h2>{listing.title}</h2>
        </div>
        <StatusBadge status={listing.status} />
      </div>

      <div className="admin-route">
        <strong>{listing.routeFrom}</strong>
        <span>→</span>
        <strong>{listing.routeTo}</strong>
      </div>

      <dl className="admin-listing-stats">
        {cargo && (
          <>
            <div><dt>Груз</dt><dd>{cargo.cargoName}</dd></div>
            <div><dt>Вес / объём</dt><dd>{cargo.weightTons} т · {cargo.volumeM3} м³</dd></div>
            <div><dt>Погрузка</dt><dd>{formatDateTime(cargo.loadDateTime)}</dd></div>
            <div><dt>Кузов</dt><dd>{cargo.bodyTypeRequired}</dd></div>
            <div><dt>Цена</dt><dd>{formatMoney(cargo.startingPrice)}</dd></div>
          </>
        )}
        {truck && (
          <>
            <div><dt>Грузоподъёмность</dt><dd>{truck.capacityTons} т</dd></div>
            <div><dt>Объём</dt><dd>{truck.volumeM3} м³</dd></div>
            <div><dt>Готовность</dt><dd>{formatDateTime(truck.availableFrom)}</dd></div>
            <div><dt>Кузов</dt><dd>{truck.bodyType}</dd></div>
            <div><dt>Ставка</dt><dd>{formatMoney(truck.price)}</dd></div>
          </>
        )}
      </dl>

      {listing.rejectionReason && (
        <div className="admin-rejection">
          <strong>Причина отклонения</strong>
          <span>{listing.rejectionReason}</span>
        </div>
      )}

      <div className="admin-card-actions">
        <span className="admin-muted">
          Создано {formatDateTime(listing.createdAt)}
        </span>
        {pending && (
          <div>
            <button className="admin-button admin-button--danger-ghost" onClick={onReject} disabled={busy}>
              Отклонить
            </button>
            <button className="admin-button admin-button--primary" onClick={onApprove} disabled={busy}>
              {busy ? 'Сохраняем…' : 'Одобрить'}
            </button>
          </div>
        )}
      </div>
    </article>
  );
};

const statusLabels: Record<ListingStatus, string> = {
  [ListingStatus.Draft]: 'Черновик',
  [ListingStatus.PendingModeration]: 'На проверке',
  [ListingStatus.Published]: 'Опубликовано',
  [ListingStatus.Rejected]: 'Отклонено',
  [ListingStatus.Archived]: 'В архиве',
  [ListingStatus.Completed]: 'Завершено',
};

function formatDateTime(value: string) {
  return new Intl.DateTimeFormat('ru-RU', {
    dateStyle: 'medium',
    timeStyle: 'short',
  }).format(new Date(value));
}

function formatMoney(value?: number | null) {
  return value
    ? new Intl.NumberFormat('ru-RU', { style: 'currency', currency: 'RUB', maximumFractionDigits: 0 }).format(value)
    : 'Договорная';
}

function shortId(id: string) {
  return id.slice(0, 8).toUpperCase();
}
