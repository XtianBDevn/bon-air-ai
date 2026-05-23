"use client";

import { useParams } from "next/navigation";
import { useQuery } from "@tanstack/react-query";
import { AppShell } from "@/components/layout/app-shell";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { api } from "@/lib/api";
import { useSessionToken } from "@/hooks/use-session-token";
import { Download, RefreshCw } from "lucide-react";
import { toast } from "sonner";

export default function CampaignDetailPage() {
  const { id } = useParams<{ id: string }>();
  const { token } = useSessionToken();

  const { data: campaign, refetch } = useQuery({
    queryKey: ["campaign", id, token],
    queryFn: () => api.getCampaign(token!, id),
    enabled: !!token && !!id,
    refetchInterval: (q) => (q.state.data?.status === "generating" ? 2000 : false),
  });

  const exportFmt = async (format: string) => {
    if (!token) return;
    try {
      const res = await api.exportCampaign(token, id, format);
      window.open(`${process.env.NEXT_PUBLIC_API_URL || "http://localhost:8080"}${res.fileUrl}`, "_blank");
      toast.success(`${format.toUpperCase()} export ready`);
    } catch {
      toast.error("Export failed");
    }
  };

  return (
    <AppShell>
      <div className="max-w-4xl mx-auto space-y-6">
        <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
          <div>
            <h1 className="text-3xl font-bold">{campaign?.name ?? "Campaign"}</h1>
            <p className="text-zinc-400 capitalize">{campaign?.status} · {campaign?.tokensUsed ?? 0} tokens</p>
          </div>
          <div className="flex gap-2 flex-wrap">
            <Button variant="outline" size="sm" onClick={() => refetch()}>
              <RefreshCw className="h-4 w-4" /> Refresh
            </Button>
            <Button variant="secondary" size="sm" onClick={() => exportFmt("pdf")}>
              <Download className="h-4 w-4" /> PDF
            </Button>
            <Button variant="secondary" size="sm" onClick={() => exportFmt("markdown")}>MD</Button>
            <Button variant="secondary" size="sm" onClick={() => exportFmt("html")}>HTML</Button>
          </div>
        </div>

        {campaign?.status === "generating" && (
          <Card>
            <CardContent className="pt-6">
              <p className="text-sm text-zinc-400 mb-2">Generating assets… {campaign.progressPercent}%</p>
              <div className="h-2 bg-zinc-800 rounded-full overflow-hidden">
                <div className="h-full bg-indigo-500 transition-all" style={{ width: `${campaign.progressPercent}%` }} />
              </div>
            </CardContent>
          </Card>
        )}

        {campaign?.headline && (
          <Card>
            <CardHeader><CardTitle>Headline & CTA</CardTitle></CardHeader>
            <CardContent>
              <p className="text-xl font-semibold">{campaign.headline}</p>
              <p className="text-indigo-400 mt-2">{campaign.cta}</p>
            </CardContent>
          </Card>
        )}

        {campaign?.generated && (
          <Card>
            <CardHeader><CardTitle>Generated JSON</CardTitle></CardHeader>
            <CardContent>
              <pre className="text-xs text-zinc-400 overflow-x-auto p-4 bg-zinc-950 rounded-lg border border-zinc-800">
                {JSON.stringify(campaign.generated, null, 2)}
              </pre>
            </CardContent>
          </Card>
        )}

        <div className="grid gap-4">
          <h2 className="text-lg font-semibold">Campaign details</h2>
          <div className="grid sm:grid-cols-2 gap-4 text-sm">
            <div className="p-4 rounded-lg border border-zinc-800"><span className="text-zinc-500">Business</span><p>{campaign?.businessType}</p></div>
            <div className="p-4 rounded-lg border border-zinc-800"><span className="text-zinc-500">Location</span><p>{campaign?.location}</p></div>
            <div className="p-4 rounded-lg border border-zinc-800 sm:col-span-2"><span className="text-zinc-500">Offer</span><p>{campaign?.offer}</p></div>
            <div className="p-4 rounded-lg border border-zinc-800 sm:col-span-2"><span className="text-zinc-500">Audience</span><p>{campaign?.targetAudience}</p></div>
          </div>
        </div>
      </div>
    </AppShell>
  );
}
