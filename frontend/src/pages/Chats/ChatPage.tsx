import { useState, useEffect, useRef } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { api, Message as ApiMessage } from '../../api/client';
import { useAuthStore } from '../../store/authStore';
import './ChatPage.css';

const AvatarPlaceholder = ({ size = 40 }: { size?: number }) => (
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

interface DisplayMessage {
  id: string;
  text: string;
  timestamp: string;
  isMine: boolean;
}

export const ChatPage = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { currentUser, isAuthenticated } = useAuthStore();
  const [messages, setMessages] = useState<DisplayMessage[]>([]);
  const [newMessage, setNewMessage] = useState('');
  const [isLoading, setIsLoading] = useState(true);
  const messagesEndRef = useRef<HTMLDivElement>(null);

  const chatName = 'Пользователь';
  const isOnline = false;

  useEffect(() => {
    console.log(' currentUser из authStore:', currentUser);
    console.log('🔑 currentUser.id:', currentUser?.id);
    console.log('🔐 isAuthenticated:', isAuthenticated);
  }, [currentUser, isAuthenticated]);

  useEffect(() => {
    if (id) {
      void loadMessages(id);
      const intervalId = window.setInterval(() => {
        void loadMessages(id, false);
      }, 5_000);

      return () => window.clearInterval(intervalId);
    }
  }, [id]);

  useEffect(() => {
    scrollToBottom();
  }, [messages]);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  };

  const loadMessages = async (chatId: string, showLoader = true) => {
    try {
      if (showLoader) setIsLoading(true);
      const messagesData: ApiMessage[] = await api.chats.getMessages(chatId, 1, 100);
      
      // ✅ Берём ID из authStore
      const currentUserId = currentUser?.id || '';
      
      console.log('📨 Всего сообщений:', messagesData.length);
      console.log('🆔 currentUserId:', currentUserId);
      
      const transformedMessages: DisplayMessage[] = messagesData
        .filter((msg: ApiMessage) => !msg.isDeleted)
        .map((msg: ApiMessage) => {
          const isMine = msg.senderUserId === currentUserId;
          console.log(`   Сообщение ${msg.id}: sender=${msg.senderUserId}, isMine=${isMine}`);
          return {
            id: msg.id,
            text: msg.text,
            timestamp: new Date(msg.createdAt).toLocaleTimeString('ru-RU', { 
              hour: '2-digit', 
              minute: '2-digit' 
            }),
            isMine,
          };
        });

      console.log('✅ Обработанные сообщения:', transformedMessages);
      setMessages(transformedMessages);
    } catch (error) {
      console.error('Ошибка загрузки сообщений:', error);
    } finally {
      if (showLoader) setIsLoading(false);
    }
  };

  const handleSend = async () => {
    if (!newMessage.trim() || !id) return;

    try {
      await api.chats.sendMessage(id, { text: newMessage.trim() });
      
      const message: DisplayMessage = {
        id: Date.now().toString(),
        text: newMessage.trim(),
        timestamp: new Date().toLocaleTimeString('ru-RU', { hour: '2-digit', minute: '2-digit' }),
        isMine: true,
      };

      setMessages((prev) => [...prev, message]);
      setNewMessage('');
      
      if (id) {
        await loadMessages(id);
      }
    } catch (error) {
      console.error('Ошибка отправки сообщения:', error);
    }
  };

  const handleKeyPress = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      handleSend();
    }
  };

  if (isLoading) {
    return (
      <div className="chat">
        <div className="chat__container">
          <div className="chat__loading">Загрузка сообщений...</div>
        </div>
      </div>
    );
  }

  return (
    <div className="chat">
      <div className="chat__container">
        <div className="chat__header">
          <button 
            className="chat__back-btn"
            onClick={() => navigate('/chats')}
          >
            ← Назад
          </button>
          
          <div className="chat__user-info">
            <div className="chat__avatar-wrapper">
              <AvatarPlaceholder size={45} />
              {isOnline && <div className="chat__online-indicator"></div>}
            </div>
            <div className="chat__user-details">
              <h2 className="chat__user-name">{chatName}</h2>
              <span className="chat__user-status">
                {isOnline ? '🟢 В сети' : '⚫ Не в сети'}
              </span>
            </div>
          </div>
        </div>

        <div className="chat__messages">
          {messages.map((message) => (
            <div
              key={message.id}
              className={`chat__message ${message.isMine ? 'chat__message--mine' : 'chat__message--other'}`}
            >
              <div className="chat__message-bubble">
                <p className="chat__message-text">{message.text}</p>
                <span className="chat__message-time">{message.timestamp}</span>
              </div>
            </div>
          ))}
          <div ref={messagesEndRef} />
        </div>

        <div className="chat__input-area">
          <textarea
            className="chat__input"
            placeholder="Введите сообщение..."
            value={newMessage}
            onChange={(e) => setNewMessage(e.target.value)}
            onKeyPress={handleKeyPress}
            rows={1}
          />
          <button 
            className="chat__send-btn"
            onClick={handleSend}
            disabled={!newMessage.trim()}
          >
            Отправить
          </button>
        </div>
      </div>
    </div>
  );
};
