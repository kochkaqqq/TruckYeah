import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { api, Chat } from '../../api/client';
import './ChatsPage.css';

const AvatarPlaceholder = ({ size = 50 }: { size?: number }) => (
  <svg 
    xmlns="http://www.w3.org/2000/svg" 
    viewBox="0 0 300 300" 
    style={{ width: size, height: size, borderRadius: '50%' }}
  >
    <circle cx="150" cy="150" r="150" fill="#d1d5db"/>
    <circle cx="150" cy="110" r="50" fill="#f3f4f6"/>
    <ellipse cx="150" cy="260" rx="90" ry="70" fill="#f3f4f6"/>
  </svg>
);

interface ChatWithUser {
  id: string;
  userId: string;
  userName: string;
  lastMessage: string;
  lastMessageTime: string;
  unreadCount: number;
  isOnline: boolean;
  route?: string;
}

export const ChatsPage = () => {
  const navigate = useNavigate();
  const [chats, setChats] = useState<ChatWithUser[]>([]);
  const [searchQuery, setSearchQuery] = useState('');
  const [showUnreadOnly, setShowUnreadOnly] = useState(false);
  const [showNewChatModal, setShowNewChatModal] = useState(false);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    void loadChats();
    const intervalId = window.setInterval(() => {
      void loadChats(false);
    }, 10_000);

    return () => window.clearInterval(intervalId);
  }, []);

  const loadChats = async (showLoader = true) => {
    try {
      if (showLoader) setIsLoading(true);
      const chatData: Chat[] = await api.chats.getAll();
      
      const transformedChats: ChatWithUser[] = await Promise.all(
        chatData.map(async (chat: Chat) => {
          const [user, context] = await Promise.all([
            api.users.getPublic(chat.otherParticipantUserId).catch(() => null),
            chat.contextType === 'Cargo'
              ? api.cargos.getById(chat.contextId).catch(() => null)
              : api.trucks.getById(chat.contextId).catch(() => null),
          ]);

          return {
            id: chat.id,
            userId: chat.otherParticipantUserId,
            userName: user?.fullName || [user?.name, user?.surname].filter(Boolean).join(' ') || user?.email || 'Пользователь',
            lastMessage: chat.lastMessageText || 'Сообщений пока нет',
            lastMessageTime: chat.lastMessageAt
              ? new Date(chat.lastMessageAt).toLocaleTimeString('ru-RU', {
                  hour: '2-digit',
                  minute: '2-digit',
                })
              : '',
            unreadCount: chat.unreadCount,
            isOnline: false,
            route: context ? `${context.routeFrom || '—'} → ${context.routeTo || '—'}` : undefined,
          };
        })
      );

      setChats(transformedChats);
    } catch (error) {
      console.error('Ошибка загрузки чатов:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const filteredChats = chats
    .filter((chat) => {
      const matchesSearch = chat.userName.toLowerCase().includes(searchQuery.toLowerCase());
      const matchesUnread = showUnreadOnly ? chat.unreadCount > 0 : true;
      return matchesSearch && matchesUnread;
    })
    .sort((a, b) => {
      if (a.unreadCount > 0 && b.unreadCount === 0) return -1;
      if (a.unreadCount === 0 && b.unreadCount > 0) return 1;
      return 0;
    });

  const handleChatClick = async (chatId: string) => {
    try {
      await api.chats.markAsRead(chatId);
      setChats((prev) =>
        prev.map((chat) =>
          chat.id === chatId ? { ...chat, unreadCount: 0 } : chat
        )
      );
    } catch (error) {
      console.error('Ошибка отметки прочитанных:', error);
    }
    
    navigate(`/chats/${chatId}`);
  };

  const handleNewChat = () => {
    setShowNewChatModal(true);
  };

  if (isLoading) {
    return (
      <div className="chats">
        <div className="chats__container">
          <div className="chats__loading">Загрузка чатов...</div>
        </div>
      </div>
    );
  }

  return (
    <div className="chats">
      <div className="chats__container">
        <div className="chats__header">
          <h1 className="chats__title">Чаты</h1>
          <button className="chats__new-btn" onClick={handleNewChat}>
            + Новый чат
          </button>
        </div>

        <div className="chats__filters">
          <div className="chats__search">
            <input
              type="text"
              className="chats__search-input"
              placeholder="Поиск по имени..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
            />
            <span className="chats__search-icon"></span>
          </div>

          <label className="chats__filter-checkbox">
            <input
              type="checkbox"
              checked={showUnreadOnly}
              onChange={(e) => setShowUnreadOnly(e.target.checked)}
            />
            <span>Только непрочитанные</span>
          </label>
        </div>

        <div className="chats__list">
          {filteredChats.length === 0 ? (
            <div className="chats__empty">
              <p>Чаты не найдены</p>
            </div>
          ) : (
            filteredChats.map((chat) => (
              <div
                key={chat.id}
                className={`chats__item ${chat.unreadCount > 0 ? 'chats__item--unread' : ''}`}
                onClick={() => handleChatClick(chat.id)}
              >
                <div className="chats__avatar-wrapper">
                  <AvatarPlaceholder size={60} />
                  {chat.isOnline && <div className="chats__online-indicator"></div>}
                </div>

                <div className="chats__content">
                  <div className="chats__top">
                    <h3 className="chats__name">{chat.userName}</h3>
                    <span className="chats__time">{chat.lastMessageTime}</span>
                  </div>
                  
                  {chat.route && (
                    <div className="chats__route">{chat.route}</div>
                  )}
                  
                  <div className="chats__bottom">
                    <p className="chats__message">{chat.lastMessage}</p>
                    {chat.unreadCount > 0 && (
                      <span className="chats__badge">{chat.unreadCount}</span>
                    )}
                  </div>
                </div>
              </div>
            ))
          )}
        </div>
      </div>

      {showNewChatModal && (
        <div 
          className="chats__modal"
          onClick={() => setShowNewChatModal(false)}
        >
          <div 
            className="chats__modal-content"
            onClick={(e) => e.stopPropagation()}
          >
            <h2 className="chats__modal-title">Новый чат</h2>
            <p className="chats__modal-text">
              Эта функция будет доступна после полной интеграции.
            </p>
            <button
              className="chats__modal-btn"
              onClick={() => setShowNewChatModal(false)}
            >
              Закрыть
            </button>
          </div>
        </div>
      )}
    </div>
  );
};
