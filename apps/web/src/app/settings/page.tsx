"use client";

import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { AppShell } from "@/components/layout/app-shell";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input, Label } from "@/components/ui/input";
import { api } from "@/lib/api";
import { useSessionToken } from "@/hooks/use-session-token";
import { useState, useEffect } from "react";
import { toast } from "sonner";

export default function SettingsPage() {
  const { token } = useSessionToken();
  const qc = useQueryClient();
  const { data } = useQuery({
    queryKey: ["brand-kit", token],
    queryFn: () => api.getBrandKit(token!),
    enabled: !!token,
  });

  const [form, setForm] = useState({
    logoUrl: "",
    primaryColor: "#6366f1",
    secondaryColor: "#8b5cf6",
    accentColor: "#06b6d4",
    fontFamily: "Inter",
    tone: "professional",
    tagline: "",
  });

  useEffect(() => {
    if (data) setForm({
      logoUrl: data.logoUrl ?? "",
      primaryColor: data.primaryColor,
      secondaryColor: data.secondaryColor,
      accentColor: data.accentColor,
      fontFamily: data.fontFamily,
      tone: data.tone,
      tagline: data.tagline ?? "",
    });
  }, [data]);

  const save = useMutation({
    mutationFn: () => api.updateBrandKit(token!, form),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ["brand-kit"] });
      toast.success("Brand kit saved");
    },
    onError: () => toast.error("Failed to save"),
  });

  return (
    <AppShell>
      <div className="max-w-2xl mx-auto space-y-6">
        <h1 className="text-3xl font-bold">Settings</h1>

        <Card>
          <CardHeader>
            <CardTitle>Brand kit</CardTitle>
            <CardDescription>Colors, tone, and white-label assets</CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <div>
              <Label>Logo URL</Label>
              <Input className="mt-1" value={form.logoUrl} onChange={(e) => setForm({ ...form, logoUrl: e.target.value })} />
            </div>
            <div className="grid grid-cols-3 gap-4">
              {(["primaryColor", "secondaryColor", "accentColor"] as const).map((k) => (
                <div key={k}>
                  <Label>{k.replace("Color", " color")}</Label>
                  <div className="flex gap-2 mt-1">
                    <input type="color" value={form[k]} onChange={(e) => setForm({ ...form, [k]: e.target.value })} className="h-10 w-12 rounded cursor-pointer" />
                    <Input value={form[k]} onChange={(e) => setForm({ ...form, [k]: e.target.value })} />
                  </div>
                </div>
              ))}
            </div>
            <div>
              <Label>Tagline</Label>
              <Input className="mt-1" value={form.tagline} onChange={(e) => setForm({ ...form, tagline: e.target.value })} />
            </div>
            <div>
              <Label>Tone</Label>
              <Input className="mt-1" value={form.tone} onChange={(e) => setForm({ ...form, tone: e.target.value })} />
            </div>
            <Button onClick={() => save.mutate()} disabled={save.isPending}>
              Save brand kit
            </Button>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>API keys</CardTitle>
            <CardDescription>Bring your own OpenAI key (stored encrypted server-side)</CardDescription>
          </CardHeader>
          <CardContent>
            <Input type="password" placeholder="sk-…" disabled className="opacity-50" />
            <p className="text-xs text-zinc-500 mt-2">Configure via environment or contact support for BYOK enablement.</p>
          </CardContent>
        </Card>
      </div>
    </AppShell>
  );
}
