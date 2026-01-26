# Docker Setup - Quick Start Guide

## Prerequisites

- Docker Desktop installed
- Docker Compose installed

## Quick Start

### 1. Start the application

```bash
docker-compose up -d
```

### 2. View logs

```bash
docker-compose logs -f api
```

### 3. Verify services are running

```bash
docker ps
```

Expected output:

- `messaging-platform-api` (port 5000)
- `messaging-platform-postgres` (port 5432)

### 4. Access the application

- API: http://localhost:5000
- Swagger: http://localhost:5000/swagger
- API Endpoints: http://localhost:5000/api/conversations

### 5. Stop the application

```bash
docker-compose down
```

### 6. Stop and remove all data

```bash
docker-compose down -v
```

## Development Workflow

### Rebuild after code changes

```bash
docker-compose up --build
```

### View database logs

```bash
docker-compose logs postgres
```

### Connect to database

```bash
docker exec -it messaging-platform-postgres psql -U postgres -d MessagingPlatform
```

### Execute SQL query

```bash
docker exec -it messaging-platform-postgres psql -U postgres -d MessagingPlatform -c "SELECT * FROM conversations;"
```

## Troubleshooting

### Check API is responding

```bash
curl http://localhost:5000/api/conversations
```

### View API logs

```bash
docker logs messaging-platform-api
```

### Restart specific service

```bash
docker-compose restart api
```

### Clean rebuild

```bash
docker-compose down -v
docker-compose build --no-cache
docker-compose up -d
```

## Environment Variables

Copy `.env.example` to `.env` and customize:

```bash
cp .env.example .env
```

Default values:

- `POSTGRES_PASSWORD=postgres`
- `POSTGRES_USER=postgres`
- `POSTGRES_DB=MessagingPlatform`

## Production Build

Build production image:

```bash
docker build -t messaging-platform:latest .
```

Run production container:

```bash
docker run -d -p 5000:8080 --name messaging-api messaging-platform:latest
```

## Network Information

- Network name: `messaging-network`
- API container name: `messaging-platform-api`
- Database container name: `messaging-platform-postgres`
- Database host (from API): `postgres`

## Volume Information

- Volume name: `postgres-data`
- Mount point: `/var/lib/postgresql/data`
- Data persists across container restarts

## Verification Checklist

- [ ] Containers start successfully
- [ ] Database migrations run automatically
- [ ] API responds at http://localhost:5000/swagger
- [ ] Swagger UI accessible at http://localhost:5000/swagger
- [ ] Database accepts connections on port 5432
- [ ] Logs show no errors
- [ ] Sample data seeded (Development mode)
