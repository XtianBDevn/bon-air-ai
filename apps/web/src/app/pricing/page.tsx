"use client";

import { useQuery, useMutation } from "@tanstack/react-query";
import { AppShell } from "@/components/layout/app-shell";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { api } from "@/lib/api";
import { useSessionToken } from "@/hooks/use-session-token";
import { toast } from "sonner";

export default function PricingPage() {
  const { token } = useSessionToken();
  const { data: plans } = useQuery({ queryKey: ["plans"], queryFn: () => api.getPlans() });

  const checkout = useMutation({
    mutationFn: (plan: string) => api.checkout(token!, plan),
    onSuccess: (data) => {
      if (data.url) window.location.href = data.url;
    },
    onError: () => toast.error("Checkout failed — configure Stripe keys"),
  });

  return (
    <AppShell>
      <div className="max-w-4xl mx-auto">
        <h1 className="text-3xl font-bold mb-2">Billing</h1>
        <p className="text-zinc-400 mb-8">Upgrade your plan via Stripe Checkout</p>
        <div className="grid md:grid-cols-3 gap-6">
          {(plans ?? []).map((p) => (
            <Card key={p.id} className={p.id === "growth" ? "border-indigo-500/50" : ""}>
              <CardHeader>
                <CardTitle>{p.name}</CardTitle>
                <p className="text-3xl font-bold">${p.price}<span className="text-sm text-zinc-500">/mo</span></p>
              </CardHeader>
              <CardContent>
                <ul className="space-y-2 text-sm text-zinc-300 mb-6">
                  {p.features.map((f) => (
                    <li key={f}>✓ {f}</li>
                  ))}
                </ul>
                <Button
                  className="w-full"
                  onClick={() => checkout.mutate(p.id)}
                  disabled={!token || checkout.isPending}
                >
                  Upgrade to {p.name}
                </Button>
              </CardContent>
            </Card>
          ))}
        </div>
      </div>
    </AppShell>
  );
}
