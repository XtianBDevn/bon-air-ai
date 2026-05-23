"use client";

import { useQuery } from "@tanstack/react-query";
import { AppShell } from "@/components/layout/app-shell";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { api } from "@/lib/api";
import { useSessionToken } from "@/hooks/use-session-token";
import { formatNumber } from "@/lib/utils";

export default function AdminPage() {
  const { token } = useSessionToken();
  const { data } = useQuery({
    queryKey: ["admin-stats", token],
    queryFn: () => api.getAdminStats(token!),
    enabled: !!token,
  });

  const stats = [
    { label: "Users", value: data?.totalUsers ?? 0 },
    { label: "Organizations", value: data?.totalOrganizations ?? 0 },
    { label: "Campaigns", value: data?.totalCampaigns ?? 0 },
    { label: "Total AI tokens", value: formatNumber(data?.totalTokens ?? 0) },
  ];

  return (
    <AppShell>
      <div className="max-w-4xl mx-auto space-y-6">
        <h1 className="text-3xl font-bold">Admin panel</h1>
        <p className="text-zinc-400">Platform usage and subscription overview</p>
        <div className="grid sm:grid-cols-2 lg:grid-cols-4 gap-4">
          {stats.map((s) => (
            <Card key={s.label}>
              <CardHeader className="pb-2">
                <CardTitle className="text-sm text-zinc-400 font-normal">{s.label}</CardTitle>
              </CardHeader>
              <CardContent>
                <p className="text-3xl font-bold">{s.value}</p>
              </CardContent>
            </Card>
          ))}
        </div>
      </div>
    </AppShell>
  );
}
