# TruckYeah

TruckYeah — учебная микросервисная платформа для публикации грузов и транспорта, торгов, расчёта маршрутов и общения участников перевозки.

## Архитектура

- `IdentityService` — регистрация, вход, JWT/refresh-токены, профиль, компании и страны.
- `ListingService` — грузы, машины, поиск, сортировка, черновики, публикация и ставки.
- `ChatService` — чаты по объявлению, история сообщений и счётчики непрочитанного.
- `RouteService` — геокодирование и расчёт маршрута через openrouteservice.
- `frontend` — React + TypeScript + Vite; nginx проксирует запросы к сервисам.
- PostgreSQL — отдельная база для каждого сервиса.

## Запуск через Docker

1. Скопируйте `.env.example` в `.env`.
2. Задайте безопасные `POSTGRES_PASSWORD`, `JWT_KEY` (не менее 32 символов) и `OPENROUTESERVICE_API_KEY`.
3. Запустите:

```powershell
docker compose up --build
```

Интерфейс доступен на `http://localhost:3000`. Health endpoints сервисов: `/health`.

## Локальная разработка

Backend:

```powershell
dotnet restore TruckYeah.sln
dotnet build TruckYeah.sln
dotnet test TruckYeah.sln
```

Frontend:

```powershell
cd frontend
npm ci
npm run lint
npm run build
npm run dev
```

Vite проксирует `/api`, `/Cargos`, `/Trucks`, `/Chats` и `/Routes` к локальным сервисам.

## Основные пользовательские сценарии

- Регистрация и вход с восстановлением сессии через `GET /api/users/me`.
- Редактирование профиля через `PUT /api/users/me`.
- Создание, редактирование, копирование, публикация и архивирование грузов и машин.
- Ручной либо автоматический расчёт массы и объёма груза.
- Поиск, фильтрация, сортировка и постраничный просмотр объявлений.
- Ставки на груз и принятие ставки владельцем.
- Чат в контексте груза или машины с polling новых сообщений.
- Расчёт маршрута через `POST /Routes/calculate`.

## API и Swagger

При запуске сервисов в Development:

- Identity: `http://localhost:5001/swagger`
- Listing: `http://localhost:5002/swagger`
- Chat: `http://localhost:5003/swagger`
- Route: `http://localhost:5004/swagger`

Публичные списки ListingService возвращают только объявления `Published` с видимостью `Exchange`. Изменение объявлений и работа с чатами требуют Bearer JWT.

## Безопасность и конфигурация

Секреты не должны попадать в Git. `.env` игнорируется, а `.env.example` содержит только шаблон. API не возвращает хеш пароля. Ошибки уровня 500 выдаются без stack trace. Все сервисы используют строковую сериализацию enum и health checks.

## Проверки

GitHub Actions выполняет backend build/test, frontend audit/lint/build и сборку Docker-образов. Перед отправкой изменений локально рекомендуется выполнить те же команды.
