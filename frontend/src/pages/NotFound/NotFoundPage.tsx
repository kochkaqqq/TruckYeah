import { Link } from 'react-router-dom';
import truckImage from '../../assets/icons/truck-404.png';
import './NotFoundPage.css';

export const NotFoundPage = () => {
  return (
    <div className="not-found">
      <div className="not-found__container">
        {/* Грузовик */}
        <div className="not-found__truck">
          <img src={truckImage} alt="404 - Грузовик не найден" />
        </div>
        
        
        
        {/* Кнопка */}
        <Link to="/" className="not-found__btn">
          Вернуться на главную
        </Link>
      </div>
    </div>
  );
};