# Launch Tonight Checklist

## Before launch

- [ ] Connect Stripe (live products + webhook)
- [ ] Add custom domain on Vercel + Railway
- [ ] Configure SMTP / Resend for auth emails
- [ ] Add PostHog analytics snippet
- [ ] Create waitlist form (optional: `/signup` CTA)
- [ ] Upload OG image (`public/og.png`, 1200×630)
- [ ] Configure SEO metadata in `apps/web/src/app/layout.tsx`
- [ ] Set `OPENAI_API_KEY` for production AI
- [ ] Verify Supabase Google OAuth redirect URLs
- [ ] Run `docker compose up` smoke test
- [ ] Run CI green on `main`

## Marketing assets

See [docs/MARKETING.md](docs/MARKETING.md) for Product Hunt, X, Reddit, LinkedIn, and email copy.

## Post-launch (Week 1)

- [ ] Monitor Hangfire failed jobs
- [ ] Review Stripe failed payments
- [ ] Check Sentry for API errors
- [ ] Gather first 10 user interviews

## Optional killer features (backlog)

- AI image generation
- Campaign scoring
- Competitor scraping
- SEO audits
- GBP automation
- Multi-language campaigns
