# Deployment Guide

## Architecture

- **Frontend:** Vercel (`apps/web`)
- **API:** Railway (`apps/api/Dockerfile`)
- **Database:** Supabase Postgres or Railway Postgres
- **Redis:** Railway Redis or Upstash
- **Auth:** Supabase
- **Storage:** Cloudflare R2 (S3-compatible)
- **Payments:** Stripe
- **Email:** Resend
- **Analytics:** PostHog
- **Errors:** Sentry

## Supabase setup

1. Create project at [supabase.com](https://supabase.com)
2. Enable Email auth + Google OAuth provider
3. Copy **Project URL**, **anon key**, and **JWT Secret** (Settings → API → JWT Settings)
4. Set `NEXT_PUBLIC_SUPABASE_URL`, `NEXT_PUBLIC_SUPABASE_ANON_KEY`, `Supabase__JwtSecret`

## Stripe setup

1. Create products: Starter ($49), Growth ($149), Agency ($399)
2. Copy Price IDs to `STRIPE_PRICE_*` env vars
3. Add webhook endpoint: `https://your-api.railway.app/api/billing/webhook`
4. Events: `checkout.session.completed`, `customer.subscription.updated`

## Railway (API)

1. New project → Deploy from GitHub
2. Set root directory / Dockerfile: `apps/api/Dockerfile`
3. Add PostgreSQL and Redis plugins (or external URLs)
4. Environment variables from `.env.example`
5. Public domain → set `Cors__Origins` to your Vercel URL

## Vercel (Web)

1. Import repo, set root to `apps/web`
2. Framework: Next.js
3. Environment:
   - `NEXT_PUBLIC_API_URL`
   - `NEXT_PUBLIC_SUPABASE_URL`
   - `NEXT_PUBLIC_SUPABASE_ANON_KEY`
   - `NEXT_PUBLIC_STRIPE_PUBLISHABLE_KEY`

## Cloudflare R2

Configure S3-compatible credentials for asset uploads (future logo storage).

## Health checks

- API: `GET /health`
- Web: Vercel deployment URL

## Production checklist

- [ ] Rotate all secrets
- [ ] Enable Stripe live mode
- [ ] Configure custom domain + SSL
- [ ] Set up Sentry DSN
- [ ] Enable PostHog
- [ ] Configure Resend for transactional email
- [ ] Upload OG images to `apps/web/public/og.png`
