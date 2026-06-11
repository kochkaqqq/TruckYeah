import { useEffect, useState } from 'react';
import './Toast.css';

interface ToastProps {
  message: string;
  type: 'success' | 'error' | 'info';
  onClose: () => void;
  duration?: number;
}

export const Toast = ({ message, type, onClose, duration = 3000 }: ToastProps) => {
  // Начальное состояние сразу true
  const [visible, setVisible] = useState(true);

  useEffect(() => {
    const timer = setTimeout(() => {
      setVisible(false);
      setTimeout(onClose, 300);
    }, duration);

    return () => clearTimeout(timer);
  }, [onClose, duration]);

  return (
    <div className={`toast toast--${type} ${visible ? 'toast--visible' : ''}`}>
      <span className="toast__icon">
        {type === 'success' ? '✓' : type === 'error' ? '✕' : 'ℹ'}
      </span>
      <span className="toast__message">{message}</span>
      <button className="toast__close" onClick={onClose}>×</button>
    </div>
  );
};