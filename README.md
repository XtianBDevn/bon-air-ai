# Bon Air Media Campaigns

AI-powered campaign operating system for agencies, law firms, restaurants, creators, and local businesses.

## Stack

| Layer | Technology |
|-------|------------|
| Frontend | Next.js 15, TypeScript, Tailwind, shadcn-style UI, Framer Motion, Zustand, React Query |
| Backend | .NET 9 Web API, PostgreSQL, Redis, Hangfire, SignalR |
| Auth | Supabase (JWT) |
| Billing | Stripe |
| Infra | Docker, GitHub Actions, Railway (API), Vercel (Web) |

## Quick start

### 1. Clone & configure

```bash
cp .env.example .env
cp apps/web/.env.example apps/web/.env.local
```

Fill in Supabase, OpenAI, and Stripe keys (see [docs/DEPLOYMENT.md](docs/DEPLOYMENT.md)).

### 2. Run with Docker

```bash
docker compose up --build
```

- Web: http://localhost:3000  
- API: http://localhost:8080  
- Swagger: http://localhost:8080/swagger  
- Hangfire: http://localhost:8080/hangfire  

### 3. Local development

**Database & Redis:**

```bash
docker compose up postgres redis -d
```

**API (.NET 9 SDK required):**

```bash
cd apps/api/src/BonAirCampaigns.Api
dotnet run
```

**Web:**

```bash
cd apps/web
npm install
npm run dev
```

## Features

- Supabase auth (email + Google OAuth)
- Multi-tenant organizations with roles
- AI campaign wizard (business → audience → offer → channels → generate)
- Orchestrated agents (SEO, ads, email, landing page, analytics)
- Structured JSON outputs + demo fallback without OpenAI key
- Hangfire background jobs + SignalR realtime updates
- PDF / Markdown / HTML exports
- Stripe Starter / Growth / Agency plans
- Brand kit & settings
- Admin usage dashboard
- Rate limiting, secure headers, OpenAPI/Swagger

## Project structure

```
apps/
  web/          # Next.js frontend
  api/          # .NET 9 API
docs/           # Deployment & launch guides
scripts/        # Seed helpers
```

## Tests

```bash
dotnet test BonAirCampaigns.sln
cd apps/web && npm run build
```

## Deployment

See [docs/DEPLOYMENT.md](docs/DEPLOYMENT.md) for Railway, Vercel, Supabase, Stripe, and R2 setup.

## Launch checklist

See [docs/LAUNCH_CHECKLIST.md](docs/LAUNCH_CHECKLIST.md).

## License

Proprietary — Bon Air Media.
