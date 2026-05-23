"use client";

import Link from "next/link";
import { motion } from "framer-motion";
import { ArrowRight, Sparkles, Zap, Shield, BarChart3, Layers } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";

const logos = ["Meridian Law", "Harbor Bistro", "Peak Agency", "Cedar Studio", "Vault Legal"];
const features = [
  { icon: Sparkles, title: "AI Campaign Engine", desc: "Generate Facebook, Google, email, SEO, and landing pages in one flow." },
  { icon: Layers, title: "Multi-Agent Orchestration", desc: "SEO, ad copy, email, and landing page agents work in parallel." },
  { icon: Zap, title: "Realtime Generation", desc: "Watch campaigns build live with SignalR progress updates." },
  { icon: Shield, title: "Agency White-Label", desc: "Custom branding, domains, and client-ready exports." },
  { icon: BarChart3, title: "Usage Analytics", desc: "Token tracking, campaign limits, and performance insights." },
];

const pricing = [
  { name: "Starter", price: "$49", campaigns: "10 campaigns/mo", features: ["All channels", "PDF export", "Email support"] },
  { name: "Growth", price: "$149", campaigns: "100 campaigns/mo", popular: true, features: ["Priority AI", "Team seats", "Markdown/HTML export"] },
  { name: "Agency", price: "$399", campaigns: "Unlimited", features: ["White label", "Custom domain", "Dedicated support"] },
];

const faqs = [
  { q: "Who is Bon Air Media Campaigns for?", a: "Agencies, law firms, restaurants, creators, and local businesses who need production-ready campaigns fast." },
  { q: "How long does generation take?", a: "Most full campaigns complete in under 2 minutes with our orchestrated agent pipeline." },
  { q: "Can I export campaigns?", a: "Yes — export to PDF, Markdown, or HTML with one click." },
];

export default function LandingPage() {
  return (
    <div className="min-h-screen gradient-mesh">
      <header className="fixed top-0 inset-x-0 z-50 border-b border-zinc-800/50 glass">
        <div className="max-w-6xl mx-auto flex items-center justify-between px-6 h-16">
          <Link href="/" className="flex items-center gap-2 font-semibold">
            <div className="h-8 w-8 rounded-lg bg-indigo-600 flex items-center justify-center text-xs">BA</div>
            Bon Air Campaigns
          </Link>
          <nav className="hidden md:flex items-center gap-8 text-sm text-zinc-400">
            <a href="#features" className="hover:text-white">Features</a>
            <a href="#pricing" className="hover:text-white">Pricing</a>
            <a href="#faq" className="hover:text-white">FAQ</a>
          </nav>
          <div className="flex items-center gap-3">
            <Link href="/login"><Button variant="ghost" size="sm">Log in</Button></Link>
            <Link href="/signup"><Button size="sm">Start free <ArrowRight className="h-4 w-4" /></Button></Link>
          </div>
        </div>
      </header>

      <section className="pt-32 pb-20 px-6 grid-pattern">
        <div className="max-w-4xl mx-auto text-center">
          <motion.div initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }}>
            <p className="text-indigo-400 text-sm font-medium mb-4">AI Campaign Operating System</p>
            <h1 className="text-5xl md:text-7xl font-bold tracking-tight text-gradient leading-[1.1]">
              Launch campaigns that feel funded.
            </h1>
            <p className="mt-6 text-lg text-zinc-400 max-w-2xl mx-auto">
              Verticalized AI for agencies and local businesses. One wizard. Every channel. Production-ready copy tonight.
            </p>
            <div className="mt-10 flex flex-col sm:flex-row gap-4 justify-center">
              <Link href="/signup"><Button size="lg">Generate your first campaign</Button></Link>
              <Link href="/login"><Button size="lg" variant="outline">View demo dashboard</Button></Link>
            </div>
          </motion.div>
          <motion.div
            initial={{ opacity: 0, y: 40 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.2 }}
            className="mt-16 rounded-2xl border border-zinc-800 glass p-1 shadow-2xl shadow-indigo-500/10"
          >
            <div className="rounded-xl bg-zinc-950 p-6 text-left">
              <div className="flex gap-2 mb-4">
                <div className="h-3 w-3 rounded-full bg-red-500/80" />
                <div className="h-3 w-3 rounded-full bg-yellow-500/80" />
                <div className="h-3 w-3 rounded-full bg-green-500/80" />
              </div>
              <pre className="text-xs text-zinc-400 overflow-x-auto">
{`{
  "headline": "Protect Your Business — Free Legal Consult",
  "facebookAds": [...],
  "emails": [...],
  "seo": { "title": "Business Lawyer Richmond VA" }
}`}
              </pre>
            </div>
          </motion.div>
        </div>
      </section>

      <section className="py-12 border-y border-zinc-800/50">
        <p className="text-center text-xs text-zinc-500 uppercase tracking-widest mb-8">Trusted by teams like</p>
        <div className="flex flex-wrap justify-center gap-8 px-6 opacity-60">
          {logos.map((l) => (
            <span key={l} className="text-sm font-medium text-zinc-400">{l}</span>
          ))}
        </div>
      </section>

      <section id="features" className="py-24 px-6 max-w-6xl mx-auto">
        <h2 className="text-3xl font-bold text-center mb-4">Everything in one campaign OS</h2>
        <p className="text-zinc-400 text-center mb-16 max-w-xl mx-auto">Not another generic AI writer. Built for agency workflows and verticalized offers.</p>
        <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-6">
          {features.map((f, i) => (
            <motion.div key={f.title} initial={{ opacity: 0, y: 20 }} whileInView={{ opacity: 1, y: 0 }} transition={{ delay: i * 0.05 }} viewport={{ once: true }}>
              <Card className="h-full hover:border-indigo-500/30 transition-colors">
                <CardContent className="pt-6">
                  <f.icon className="h-8 w-8 text-indigo-400 mb-4" />
                  <h3 className="font-semibold text-lg mb-2">{f.title}</h3>
                  <p className="text-sm text-zinc-400">{f.desc}</p>
                </CardContent>
              </Card>
            </motion.div>
          ))}
        </div>
      </section>

      <section id="pricing" className="py-24 px-6 border-t border-zinc-800/50">
        <h2 className="text-3xl font-bold text-center mb-16">Simple, scalable pricing</h2>
        <div className="grid md:grid-cols-3 gap-6 max-w-5xl mx-auto">
          {pricing.map((p) => (
            <Card key={p.name} className={p.popular ? "border-indigo-500/50 ring-1 ring-indigo-500/20" : ""}>
              <CardContent className="pt-8">
                {p.popular && <span className="text-xs text-indigo-400 font-medium">Most popular</span>}
                <h3 className="text-xl font-bold mt-2">{p.name}</h3>
                <p className="text-4xl font-bold mt-4">{p.price}<span className="text-sm text-zinc-500 font-normal">/mo</span></p>
                <p className="text-sm text-zinc-400 mt-2">{p.campaigns}</p>
                <ul className="mt-6 space-y-2 text-sm text-zinc-300">
                  {p.features.map((f) => (
                    <li key={f}>✓ {f}</li>
                  ))}
                </ul>
                <Link href="/signup" className="block mt-8">
                  <Button className="w-full" variant={p.popular ? "default" : "secondary"}>Get started</Button>
                </Link>
              </CardContent>
            </Card>
          ))}
        </div>
      </section>

      <section id="faq" className="py-24 px-6 max-w-2xl mx-auto">
        <h2 className="text-3xl font-bold text-center mb-12">FAQ</h2>
        <div className="space-y-6">
          {faqs.map((f) => (
            <div key={f.q} className="border-b border-zinc-800 pb-6">
              <h3 className="font-medium mb-2">{f.q}</h3>
              <p className="text-sm text-zinc-400">{f.a}</p>
            </div>
          ))}
        </div>
      </section>

      <section className="py-24 px-6 text-center">
        <h2 className="text-3xl font-bold mb-6">Ready to launch tonight?</h2>
        <Link href="/signup"><Button size="lg">Start building campaigns <ArrowRight className="h-4 w-4" /></Button></Link>
      </section>

      <footer className="border-t border-zinc-800 py-8 text-center text-sm text-zinc-500">
        © {new Date().getFullYear()} Bon Air Media. All rights reserved.
      </footer>
    </div>
  );
}
