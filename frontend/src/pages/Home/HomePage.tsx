import { Link } from 'react-router-dom';
import { useState, useEffect, useRef } from 'react';
import TruckIcon from '../../assets/icons/truck.svg?react';
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
          <h1 className="home__title">
            Биржа грузоперевозок и крупнейшая экосистема сервисов<br />
            для транспортной логистики
          </h1>
          <p className="home__subtitle">
            Помогаем находить грузы, проверенных перевозчиков и экономить за счёт автоматизации процессов
          </p>
          
          <div className="home__truck-wrapper">
            <TruckIcon className="home__truck" />
          </div>
        </div>

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