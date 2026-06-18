import { useNavigate } from 'react-router-dom';
import truckImage from '../../assets/icons/logo.png';
import './NotFoundPage.css';

export const NotFoundPage = () => {
  const navigate = useNavigate();

  const handleClick = () => {
    navigate('/');
  };

  return (
    <div className="not-found" onClick={handleClick}>
      <div className="not-found__container">
        <div className="not-found__truck">
          <img src={truckImage} alt="404" />
        </div>
      </div>
    </div>
  );
};