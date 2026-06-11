import { Link } from 'react-router-dom';
import './Header.css';

export const Header = () => {
  return (
    <header className="header">
      <div className="header__container">
        <Link to="/" className="header__logo">
          ati.su
        </Link>
        
        <nav className="header__nav">
          <Link to="/cargo" className="header__link">Грузы</Link>
          <Link to="/vehicles" className="header__link">Машины</Link>
          <Link to="/my-orders" className="header__link">Ваши заказы</Link>
          <Link to="/chats" className="header__link">Чаты</Link>
          <Link to="/profile" className="header__link">Профиль</Link>
        </nav>
        
        <div className="header__actions">
          {/* Здесь потом будет аватар или кнопка выхода */}
        </div>
      </div>
    </header>
  );
};