import { useEffect, useMemo, useState } from 'react';
import { ListingStatus, type CargoResponse, type TruckResponse } from '../../api/client';
import { moderatorApi } from '../../api/moderatorClient';
import {
  AdminNotice,
  AdminPageHeader,
  AdminSkeleton,
  ListingCard,
} from './AdminShared';
import './Admin.css';

type ListingKind = 'cargo' | 'truck';

export const AdminListingsPage = ({ kind }: { kind: ListingKind }) => {
  const [items, setItems] = useState<Array<CargoResponse | TruckResponse>>([]);
  const [query, setQuery] = useState('');
  const [status, setStatus] = useState('All');
  const [loading, setLoading] = useState(true);
  const [busyId, setBusyId] = useState<string | null>(null);
  const [rejecting, setRejecting] = useState<CargoResponse | TruckResponse | null>(null);
  const [reason, setReason] = useState('');
  const [notice, setNotice] = useState<{ type: 'success' | 'error'; text: string } | null>(
    null,
  );

  const load = async () => {
    setLoading(true);
    try {
      setItems(
        kind === 'cargo'
          ? await moderatorApi.cargos.getAll()
          : await moderatorApi.trucks.getAll(),
      );
    } catch (error) {
      setNotice({ type: 'error', text: error instanceof Error ? error.message : 'Ошибка' });
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    // Initial synchronization with the moderator API.
    // eslint-disable-next-line react-hooks/set-state-in-effect
    void load();
  }, [kind]);

  const filtered = useMemo(() => {
    const normalized = query.trim().toLowerCase();
    return items.filter((item) => {
      const matchesStatus = status === 'All' || item.status === status;
      const matchesQuery =
        !normalized ||
        [item.title, item.routeFrom, item.routeTo, item.id]
          .filter((value): value is string => Boolean(value))
          .some((value) => value.toLowerCase().includes(normalized));
      return matchesStatus && matchesQuery;
    });
  }, [items, query, status]);

  const approve = async (item: CargoResponse | TruckResponse) => {
    setBusyId(item.id);
    try {
      if (kind === 'cargo') await moderatorApi.cargos.approve(item.id);
      else await moderatorApi.trucks.approve(item.id);
      setNotice({ type: 'success', text: 'Объявление одобрено и опубликовано.' });
      await load();
    } catch (error) {
      setNotice({ type: 'error', text: error instanceof Error ? error.message : 'Ошибка' });
    } finally {
      setBusyId(null);
    }
  };

  const reject = async () => {
    if (!rejecting || !reason.trim()) return;
    setBusyId(rejecting.id);
    try {
      if (kind === 'cargo') await moderatorApi.cargos.reject(rejecting.id, reason.trim());
      else await moderatorApi.trucks.reject(rejecting.id, reason.trim());
      setNotice({ type: 'success', text: 'Объявление отклонено.' });
      setRejecting(null);
      setReason('');
      await load();
    } catch (error) {
      setNotice({ type: 'error', text: error instanceof Error ? error.message : 'Ошибка' });
    } finally {
      setBusyId(null);
    }
  };

  const pendingCount = items.filter(
    (item) => item.status === ListingStatus.PendingModeration,
  ).length;

  return (
    <section className="admin-page">
      <AdminPageHeader
        title={kind === 'cargo' ? 'Грузы' : 'Машины'}
        subtitle={`${pendingCount} ожидают решения · всего ${items.length}`}
        action={<button className="admin-button" onClick={() => void load()}>Обновить</button>}
      />

      {notice && <AdminNotice {...notice} onClose={() => setNotice(null)} />}

      <div className="admin-toolbar">
        <label className="admin-search">
          <span>⌕</span>
          <input
            value={query}
            onChange={(event) => setQuery(event.target.value)}
            placeholder="Название, маршрут или ID"
          />
        </label>
        <select value={status} onChange={(event) => setStatus(event.target.value)}>
          <option value="All">Все статусы</option>
          <option value={ListingStatus.PendingModeration}>На проверке</option>
          <option value={ListingStatus.Published}>Опубликованные</option>
          <option value={ListingStatus.Rejected}>Отклонённые</option>
          <option value={ListingStatus.Draft}>Черновики</option>
          <option value={ListingStatus.Archived}>Архив</option>
        </select>
      </div>

      {loading ? (
        <AdminSkeleton />
      ) : (
        <div className="admin-listing-grid">
          {filtered.map((item) => (
            <ListingCard
              key={item.id}
              listing={item}
              kind={kind}
              busy={busyId === item.id}
              onApprove={() => void approve(item)}
              onReject={() => {
                setRejecting(item);
                setReason('');
              }}
            />
          ))}
        </div>
      )}

      {!loading && filtered.length === 0 && (
        <div className="admin-empty">Объявления не найдены.</div>
      )}

      {rejecting && (
        <div className="admin-modal-backdrop" onMouseDown={() => setRejecting(null)}>
          <div className="admin-modal" onMouseDown={(event) => event.stopPropagation()}>
            <div>
              <p className="admin-eyebrow">Отклонение объявления</p>
              <h2>{rejecting.title}</h2>
              <p className="admin-muted">Причина будет видна владельцу объявления.</p>
            </div>
            <label className="admin-field">
              <span>Причина</span>
              <textarea
                value={reason}
                onChange={(event) => setReason(event.target.value)}
                placeholder="Опишите, что необходимо исправить"
                maxLength={1000}
                autoFocus
              />
              <small>{reason.length}/1000</small>
            </label>
            <div className="admin-modal__actions">
              <button className="admin-button" onClick={() => setRejecting(null)}>Отмена</button>
              <button
                className="admin-button admin-button--danger"
                onClick={() => void reject()}
                disabled={!reason.trim() || busyId === rejecting.id}
              >
                {busyId === rejecting.id ? 'Отклоняем…' : 'Отклонить'}
              </button>
            </div>
          </div>
        </div>
      )}
    </section>
  );
};
