import { useEffect, useMemo, useState } from 'react';
import { moderatorApi, type AdminUser } from '../../api/moderatorClient';
import { AdminNotice, AdminPageHeader, AdminSkeleton } from './AdminShared';
import './Admin.css';

export const AdminUsersPage = () => {
  const [users, setUsers] = useState<AdminUser[]>([]);
  const [query, setQuery] = useState('');
  const [status, setStatus] = useState('All');
  const [loading, setLoading] = useState(true);
  const [busyId, setBusyId] = useState<string | null>(null);
  const [notice, setNotice] = useState<{ type: 'success' | 'error'; text: string } | null>(
    null,
  );

  const loadUsers = async () => {
    setLoading(true);
    try {
      setUsers(await moderatorApi.users.getAll());
    } catch (error) {
      setNotice({ type: 'error', text: error instanceof Error ? error.message : 'Ошибка' });
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    // Initial synchronization with the moderator API.
    // eslint-disable-next-line react-hooks/set-state-in-effect
    void loadUsers();
  }, []);

  const filtered = useMemo(() => {
    const normalized = query.trim().toLowerCase();
    return users.filter((user) => {
      const matchesStatus = status === 'All' || user.status === status;
      const matchesQuery =
        !normalized ||
        [user.displayName, user.email, user.phone, user.company, user.city]
          .filter(Boolean)
          .some((value) => value!.toLowerCase().includes(normalized));
      return matchesStatus && matchesQuery;
    });
  }, [query, status, users]);

  const toggleBlocked = async (user: AdminUser) => {
    setBusyId(user.id);
    setNotice(null);
    try {
      if (user.status === 'Blocked') {
        await moderatorApi.users.unblock(user.id);
        setNotice({ type: 'success', text: `${user.displayName} разблокирован.` });
      } else {
        await moderatorApi.users.block(user.id);
        setNotice({ type: 'success', text: `${user.displayName} заблокирован.` });
      }
      await loadUsers();
    } catch (error) {
      setNotice({ type: 'error', text: error instanceof Error ? error.message : 'Ошибка' });
    } finally {
      setBusyId(null);
    }
  };

  return (
    <section className="admin-page">
      <AdminPageHeader
        title="Пользователи"
        subtitle={`${users.length} аккаунтов на платформе`}
        action={<button className="admin-button" onClick={() => void loadUsers()}>Обновить</button>}
      />

      {notice && <AdminNotice {...notice} onClose={() => setNotice(null)} />}

      <div className="admin-toolbar">
        <label className="admin-search">
          <span>⌕</span>
          <input
            value={query}
            onChange={(event) => setQuery(event.target.value)}
            placeholder="Имя, email, телефон или компания"
          />
        </label>
        <select value={status} onChange={(event) => setStatus(event.target.value)}>
          <option value="All">Все статусы</option>
          <option value="Active">Активные</option>
          <option value="Blocked">Заблокированные</option>
        </select>
      </div>

      {loading ? (
        <AdminSkeleton />
      ) : (
        <div className="admin-user-grid">
          {filtered.map((user) => (
            <article className="admin-user-card" key={user.id}>
              <div className="admin-user-card__profile">
                <div className="admin-avatar">
                  {user.avatarLink ? (
                    <img src={user.avatarLink} alt="" />
                  ) : (
                    user.displayName.slice(0, 1).toUpperCase()
                  )}
                </div>
                <div>
                  <div className="admin-card-title-row">
                    <h2>{user.displayName}</h2>
                    <span className={`admin-status admin-status--${user.status.toLowerCase()}`}>
                      {user.status === 'Active' ? 'Активен' : 'Заблокирован'}
                    </span>
                  </div>
                  <p className="admin-muted">{user.company || user.email}</p>
                </div>
              </div>

              <dl className="admin-meta-grid">
                <div><dt>Email</dt><dd>{user.email}</dd></div>
                <div><dt>Телефон</dt><dd>{user.phone}</dd></div>
                <div><dt>Город</dt><dd>{user.city || 'Не указан'}</dd></div>
                <div><dt>Регистрация</dt><dd>{formatDate(user.createdAt)}</dd></div>
                <div><dt>Тип</dt><dd>{formatUserType(user.userType)}</dd></div>
                <div><dt>Рейтинг</dt><dd>★ {user.rating.toFixed(1)}</dd></div>
              </dl>

              <div className="admin-card-actions">
                <button
                  className={
                    user.status === 'Blocked'
                      ? 'admin-button admin-button--primary'
                      : 'admin-button admin-button--danger'
                  }
                  onClick={() => void toggleBlocked(user)}
                  disabled={busyId === user.id || user.role === 'Moderator'}
                >
                  {busyId === user.id
                    ? 'Сохраняем…'
                    : user.status === 'Blocked'
                      ? 'Разблокировать'
                      : 'Заблокировать'}
                </button>
              </div>
            </article>
          ))}
        </div>
      )}

      {!loading && filtered.length === 0 && (
        <div className="admin-empty">По вашему запросу пользователи не найдены.</div>
      )}
    </section>
  );
};

function formatDate(value: string) {
  return new Intl.DateTimeFormat('ru-RU', { dateStyle: 'medium' }).format(new Date(value));
}

function formatUserType(value: string | number) {
  const key = String(value);
  return { Business: 'Компания', Private: 'Частное лицо', Individual: 'ИП / физлицо' }[
    key
  ] || key;
}
