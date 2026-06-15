import { Link } from 'react-router-dom';
import { useState, useEffect, useRef } from 'react';
import './HomePage.css';

export const HomePage = () => {
  const [showScrollHint, setShowScrollHint] = useState(true);
  const contentRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const observer = new IntersectionObserver(
      (entries) => {
        entries.forEach((entry) => {
          if (entry.isIntersecting) {
            setShowScrollHint(false);
          } else {
            setShowScrollHint(true);
          }
        });
      },
      {
        threshold: 0.1,
      }
    );

    if (contentRef.current) {
      observer.observe(contentRef.current);
    }

    return () => observer.disconnect();
  }, []);

  return (
    <div className="home">
      <section className="home__hero">
        <div className="home__hero-inner">
          {/* Сначала текст */}
          <h1 className="home__title">
            Биржа грузоперевозок и крупнейшая экосистема сервисов<br />
            для транспортной логистики
          </h1>
          <p className="home__subtitle">
            Помогаем находить грузы, проверенных перевозчиков и экономить за счёт автоматизации процессов
          </p>
          
          {/* Потом грузовик */}
          <div className="home__truck-wrapper">
            <svg className="home__truck" viewBox="0 0 1200 600" xmlns="http://www.w3.org/2000/svg">
              <rect x="50" y="150" width="700" height="300" rx="15" stroke="white" strokeWidth="8" fill="none"/>
              <rect x="780" y="200" width="300" height="250" rx="15" stroke="white" strokeWidth="8" fill="none"/>
              <rect x="820" y="230" width="220" height="120" rx="8" stroke="white" strokeWidth="6" fill="none"/>
              <circle cx="200" cy="480" r="50" stroke="white" strokeWidth="8" fill="none"/>
              <circle cx="350" cy="480" r="50" stroke="white" strokeWidth="8" fill="none"/>
              <circle cx="500" cy="480" r="50" stroke="white" strokeWidth="8" fill="none"/>
              <circle cx="850" cy="480" r="50" stroke="white" strokeWidth="8" fill="none"/>
              <circle cx="1000" cy="480" r="50" stroke="white" strokeWidth="8" fill="none"/>
              <line x1="0" y1="540" x2="1200" y2="540" stroke="white" strokeWidth="6"/>
            </svg>
          </div>
        </div>

        {/* Индикатор скролла внизу */}
        {showScrollHint && (
          <div className="scroll-indicator">
            <div className="scroll-indicator__mouse">
              <div className="scroll-indicator__wheel"></div>
            </div>
          </div>
        )}
      </section>

      <div className="home__content" ref={contentRef}>
        <div className="home__container">
          <section className="home__section">
            <h2 className="home__section-title">Грузы</h2>
            
            <div className="home__cards">
              <Link to="/cargo/add" className="home__card">
                <span className="home__card-text">Добавьте новый груз</span>
                <button className="home__btn">Добавить груз</button>
              </Link>
              
              <Link to="/cargo/search" className="home__card">
                <span className="home__card-text">Поиск существующих грузов</span>
                <button className="home__btn">Перейти к поиску</button>
              </Link>
            </div>
          </section>

          <section className="home__section">
            <h2 className="home__section-title">Машины</h2>
            
            <div className="home__cards">
              <Link to="/vehicles/add" className="home__card">
                <span className="home__card-text">Добавьте новую машину</span>
                <button className="home__btn">Добавить машину</button>
              </Link>
              
              <Link to="/vehicles/search" className="home__card">
                <span className="home__card-text">Поиск существующих машин</span>
                <button className="home__btn">Перейти к поиску</button>
              </Link>
            </div>
          </section>
        </div>
      </div>

      <footer className="home__footer">
        <div className="home__footer-content">
          <span className="home__footer-logo">ati.su</span>
          <p className="home__footer-text">© 2026 Биржа грузоперевозок</p>
        </div>
      </footer>
    </div>
  );
};