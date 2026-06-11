# Docker Compose

В проекте используется один общий `docker-compose.yml`, который поднимает весь стек приложения:

- frontend;
- identity-service;
- listing-service;
- identity-db;
- listing-db.

## Запуск

Из корня проекта выполните:

```powershell
docker compose up --build
```

После запуска фронтенд будет доступен по адресу:

```text
http://localhost:3000
```

## Сервисы и порты

| Сервис | Назначение | Внешний порт |
| --- | --- | --- |
| `frontend` | React/Vite приложение, раздается через nginx | `3000` |
| `identity-service` | Backend для пользователей, авторизации и компаний | `5001` |
| `listing-service` | Backend для грузов и машин | `5002` |
| `identity-db` | PostgreSQL база для identity-сервиса | `5433` |
| `listing-db` | PostgreSQL база для listing-сервиса | `5434` |

## Как работает связь сервисов

Все контейнеры находятся в одной Docker-сети `truckyeah`.

Внутри Docker-сети сервисы обращаются друг к другу по именам:

```text
identity-service
listing-service
identity-db
listing-db
```

Backend-сервисы подключаются к базам данных через внутренние адреса Docker Compose:

```text
identity-service -> identity-db:5432
listing-service  -> listing-db:5432
```

Снаружи базы проброшены на разные порты, чтобы не конфликтовать между собой:

```text
identity-db: localhost:5433
listing-db:  localhost:5434
```

## Frontend и API

Браузер работает только с фронтендом по адресу `http://localhost:3000`.

Фронтенд раздается через nginx. Этот nginx также проксирует API-запросы к нужным backend-сервисам:

```text
/api/users/*      -> identity-service
/api/companies/*  -> identity-service
/Cargos*          -> listing-service
/Trucks*          -> listing-service
```

То есть браузеру не нужно напрямую обращаться к `identity-service` или `listing-service`. Все API-запросы идут через `localhost:3000`.

## Данные баз

Данные PostgreSQL сохраняются в Docker volumes:

```text
identity-db-data
listing-db-data
```

Поэтому данные не пропадают после обычного перезапуска контейнеров.
