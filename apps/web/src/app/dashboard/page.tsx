"use client";

import { useQuery } from "@tanstack/react-query";
import Link from "next/link";
import { motion } from "framer-motion";
import { Sparkles, TrendingUp, Zap, FileText, ArrowRight } from "lucide-react";
import { AppShell } from "@/components/layout/app-shell";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { api } from "@/lib/api";
import { useSessionToken } from "@/hooks/use-session-token";
import { formatNumber } from "@/lib/utils";
import {
  AreaChart,
  Area,
  XAxis,
  YAxis,
  Tooltip,
  ResponsiveContainer,
} from "recharts";

export default function DashboardPage() {
  const { token, loading: authLoading } = useSessionToken();

  const { data: me } = useQuery({
    queryKey: ["me", token],
    queryFn: () => api.getMe(token!),
    enabled: !!token,
  });

  const { data, isLoading } = useQuery({
    queryKey: ["dashboard", token],
    queryFn: () => api.getDashboard(token!),
    enabled: !!token,
    refetchInterval: 5000,
  });

  if (authLoading || isLoading) {
    return (
      <AppShell>
        <div className="animate-pulse space-y-4">
          <div className="h-8 w-64 bg-zinc-800 rounded" />
          <div className="grid grid-cols-4 gap-4">
            {[1, 2, 3, 4].map((i) => (
              <div key={i} className="h-28 bg-zinc-800/50 rounded-xl" />
            ))}
          </div>
        </div>
      </AppShell>
    );
  }

  const stats = [
    { label: "Total campaigns", value: data?.totalCampaigns ?? 0, icon: Sparkles },
    { label: "Active", value: data?.activeCampaigns ?? 0, icon: Zap },
    { label: "Tokens this month", value: formatNumber(data?.tokensUsedThisMonth ?? 0), icon: TrendingUp },
    { label: "Remaining", value: data?.campaignsRemaining ?? 0, icon: FileText },
  ];

  return (
    <AppShell orgName={me?.organization.name}>
      <div className="max-w-6xl mx-auto space-y-8">
        <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
          <div>
            <motion.h1 initial={{ opacity: 0 }} animate={{ opacity: 1 }} className="text-3xl font-bold">
              Campaign command center
            </motion.h1>
            <p className="text-zinc-400 mt-1">
              {me?.organization.plan ?? "starter"} plan · {data?.campaignsRemaining ?? 0} campaigns left
            </p>
          </div>
          <Link href="/campaigns/new">
            <Button size="lg" className="gap-2">
              <Sparkles className="h-4 w-4" />
              Generate campaign
            </Button>
          </Link>
        </div>

        <div className="grid sm:grid-cols-2 lg:grid-cols-4 gap-4">
          {stats.map((s, i) => (
            <motion.div key={s.label} initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: i * 0.05 }}>
              <Card>
                <CardContent className="pt-6">
                  <div className="flex items-center justify-between">
                    <p className="text-sm text-zinc-400">{s.label}</p>
                    <s.icon className="h-4 w-4 text-indigo-400" />
                  </div>
                  <p className="text-3xl font-bold mt-2">{s.value}</p>
                </CardContent>
              </Card>
            </motion.div>
          ))}
        </div>

        <div className="grid lg:grid-cols-3 gap-6">
          <Card className="lg:col-span-2">
            <CardHeader>
              <CardTitle>Token usage</CardTitle>
              <CardDescription>Last 14 days</CardDescription>
            </CardHeader>
            <CardContent className="h-64">
              <ResponsiveContainer width="100%" height="100%">
                <AreaChart data={data?.usageChart ?? []}>
                  <defs>
                    <linearGradient id="colorTokens" x1="0" y1="0" x2="0" y2="1">
                      <stop offset="5%" stopColor="#6366f1" stopOpacity={0.4} />
                      <stop offset="95%" stopColor="#6366f1" stopOpacity={0} />
                    </linearGradient>
                  </defs>
                  <XAxis dataKey="date" stroke="#71717a" fontSize={11} tickFormatter={(v) => v.slice(5)} />
                  <YAxis stroke="#71717a" fontSize={11} />
                  <Tooltip contentStyle={{ background: "#18181b", border: "1px solid #3f3f46" }} />
                  <Area type="monotone" dataKey="tokens" stroke="#6366f1" fill="url(#colorTokens)" />
                </AreaChart>
              </ResponsiveContainer>
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle>AI campaign feed</CardTitle>
              <CardDescription>Recent activity</CardDescription>
            </CardHeader>
            <CardContent className="space-y-3 max-h-64 overflow-y-auto">
              {(data?.recentCampaigns ?? []).map((c) => (
                <Link
                  key={c.id}
                  href={`/campaigns/${c.id}`}
                  className="block p-3 rounded-lg border border-zinc-800 hover:border-indigo-500/30 transition-colors"
                >
                  <div className="flex justify-between items-start">
                    <p className="font-medium text-sm truncate">{c.name}</p>
                    <span className={`text-xs px-2 py-0.5 rounded-full ${
                      c.status === "completed" ? "bg-green-500/20 text-green-400" :
                      c.status === "generating" ? "bg-amber-500/20 text-amber-400" :
                      "bg-zinc-700 text-zinc-300"
                    }`}>
                      {c.status}
                    </span>
                  </div>
                  {c.status === "generating" && (
                    <div className="mt-2 h-1 bg-zinc-800 rounded-full overflow-hidden">
                      <div className="h-full bg-indigo-500 transition-all" style={{ width: `${c.progressPercent}%` }} />
                    </div>
                  )}
                </Link>
              ))}
            </CardContent>
          </Card>
        </div>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between">
            <div>
              <CardTitle>Generated assets</CardTitle>
              <CardDescription>Latest from your campaigns</CardDescription>
            </div>
            <Link href="/campaigns/new"><Button variant="ghost" size="sm">New <ArrowRight className="h-4 w-4" /></Button></Link>
          </CardHeader>
          <CardContent>
            <div className="grid md:grid-cols-2 gap-4">
              {(data?.recentAssets ?? []).map((a) => (
                <div key={a.id} className="p-4 rounded-lg border border-zinc-800 bg-zinc-950/30">
                  <p className="text-xs text-indigo-400 uppercase tracking-wide">{a.assetType.replace("_", " ")}</p>
                  <p className="font-medium mt-1">{a.title}</p>
                  <p className="text-sm text-zinc-500 mt-2 line-clamp-2">{a.content}</p>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      </div>
    </AppShell>
  );
}
