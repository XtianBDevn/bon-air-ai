"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { motion, AnimatePresence } from "framer-motion";
import { AppShell } from "@/components/layout/app-shell";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input, Label, Textarea } from "@/components/ui/input";
import { api } from "@/lib/api";
import { useSessionToken } from "@/hooks/use-session-token";
import { toast } from "sonner";
import { ChevronLeft, ChevronRight, Sparkles } from "lucide-react";

const businessTypes = [
  { id: "law_firm", label: "Law Firm" },
  { id: "restaurant", label: "Restaurant" },
  { id: "agency", label: "Agency" },
  { id: "creator", label: "Creator" },
  { id: "local_business", label: "Local Business" },
];

const tones = ["professional", "bold", "friendly", "trustworthy", "playful"];
const channels = [
  { id: "facebook", label: "Facebook Ads" },
  { id: "google", label: "Google Ads" },
  { id: "email", label: "Email" },
  { id: "seo", label: "SEO Pages" },
  { id: "landing", label: "Landing Page" },
  { id: "gbp", label: "GBP Posts" },
  { id: "reddit", label: "Reddit" },
  { id: "discord", label: "Discord" },
];

const steps = ["Business", "Audience", "Offer", "Channels", "Generate"];

export default function NewCampaignPage() {
  const { token } = useSessionToken();
  const router = useRouter();
  const [step, setStep] = useState(0);
  const [loading, setLoading] = useState(false);
  const [form, setForm] = useState({
    name: "",
    businessType: "law_firm",
    targetAudience: "",
    location: "",
    offer: "",
    tone: "professional",
    budget: "",
    selectedChannels: ["facebook", "google", "email", "seo", "landing"] as string[],
  });

  const update = (k: string, v: string | string[]) => setForm((f) => ({ ...f, [k]: v }));

  const toggleChannel = (id: string) => {
    setForm((f) => ({
      ...f,
      selectedChannels: f.selectedChannels.includes(id)
        ? f.selectedChannels.filter((c) => c !== id)
        : [...f.selectedChannels, id],
    }));
  };

  const submit = async () => {
    if (!token) return;
    setLoading(true);
    try {
      const campaign = await api.createCampaign(token, {
        name: form.name || `${form.offer} Campaign`,
        businessType: form.businessType,
        offer: form.offer,
        targetAudience: form.targetAudience,
        location: form.location,
        tone: form.tone,
        budget: form.budget ? parseFloat(form.budget) : null,
        channels: form.selectedChannels,
      });
      toast.success("Campaign queued — generating assets…");
      router.push(`/campaigns/${campaign.id}`);
    } catch (e) {
      toast.error(e instanceof Error ? e.message : "Failed to create campaign");
    } finally {
      setLoading(false);
    }
  };

  return (
    <AppShell>
      <div className="max-w-2xl mx-auto">
        <h1 className="text-3xl font-bold mb-2">Generate campaign</h1>
        <p className="text-zinc-400 mb-8">Multi-channel AI assets in one wizard</p>

        <div className="flex gap-2 mb-8">
          {steps.map((s, i) => (
            <div
              key={s}
              className={`flex-1 h-1 rounded-full ${i <= step ? "bg-indigo-500" : "bg-zinc-800"}`}
            />
          ))}
        </div>

        <Card>
          <CardHeader>
            <CardTitle>{steps[step]}</CardTitle>
            <CardDescription>Step {step + 1} of {steps.length}</CardDescription>
          </CardHeader>
          <CardContent>
            <AnimatePresence mode="wait">
              <motion.div
                key={step}
                initial={{ opacity: 0, x: 10 }}
                animate={{ opacity: 1, x: 0 }}
                exit={{ opacity: 0, x: -10 }}
                className="space-y-4"
              >
                {step === 0 && (
                  <>
                    <div>
                      <Label>Campaign name</Label>
                      <Input className="mt-1" placeholder="Spring Promo 2025" value={form.name} onChange={(e) => update("name", e.target.value)} />
                    </div>
                    <div>
                      <Label>Business type</Label>
                      <div className="grid grid-cols-2 gap-2 mt-2">
                        {businessTypes.map((b) => (
                          <button
                            key={b.id}
                            type="button"
                            onClick={() => update("businessType", b.id)}
                            className={`p-3 rounded-lg border text-sm text-left ${
                              form.businessType === b.id ? "border-indigo-500 bg-indigo-500/10" : "border-zinc-700"
                            }`}
                          >
                            {b.label}
                          </button>
                        ))}
                      </div>
                    </div>
                    <div>
                      <Label>Location</Label>
                      <Input className="mt-1" placeholder="Richmond, VA" value={form.location} onChange={(e) => update("location", e.target.value)} />
                    </div>
                  </>
                )}
                {step === 1 && (
                  <div>
                    <Label>Target audience</Label>
                    <Textarea
                      className="mt-1"
                      placeholder="Small business owners aged 35-55 looking for legal protection…"
                      value={form.targetAudience}
                      onChange={(e) => update("targetAudience", e.target.value)}
                    />
                  </div>
                )}
                {step === 2 && (
                  <>
                    <div>
                      <Label>Your offer</Label>
                      <Textarea className="mt-1" placeholder="Free 30-minute consultation…" value={form.offer} onChange={(e) => update("offer", e.target.value)} />
                    </div>
                    <div>
                      <Label>Tone</Label>
                      <div className="flex flex-wrap gap-2 mt-2">
                        {tones.map((t) => (
                          <button
                            key={t}
                            type="button"
                            onClick={() => update("tone", t)}
                            className={`px-3 py-1.5 rounded-full text-sm border ${
                              form.tone === t ? "border-indigo-500 bg-indigo-500/10" : "border-zinc-700"
                            }`}
                          >
                            {t}
                          </button>
                        ))}
                      </div>
                    </div>
                    <div>
                      <Label>Monthly ad budget ($)</Label>
                      <Input type="number" className="mt-1" value={form.budget} onChange={(e) => update("budget", e.target.value)} />
                    </div>
                  </>
                )}
                {step === 3 && (
                  <div className="grid grid-cols-2 gap-2">
                    {channels.map((c) => (
                      <button
                        key={c.id}
                        type="button"
                        onClick={() => toggleChannel(c.id)}
                        className={`p-3 rounded-lg border text-sm ${
                          form.selectedChannels.includes(c.id) ? "border-indigo-500 bg-indigo-500/10" : "border-zinc-700"
                        }`}
                      >
                        {c.label}
                      </button>
                    ))}
                  </div>
                )}
                {step === 4 && (
                  <div className="text-center py-8">
                    <Sparkles className="h-12 w-12 text-indigo-400 mx-auto mb-4" />
                    <p className="text-lg font-medium">Ready to generate</p>
                    <p className="text-sm text-zinc-400 mt-2 max-w-sm mx-auto">
                      We&apos;ll orchestrate SEO, ad copy, email, landing page, and social agents for your campaign.
                    </p>
                  </div>
                )}
              </motion.div>
            </AnimatePresence>

            <div className="flex justify-between mt-8">
              <Button variant="ghost" onClick={() => setStep((s) => Math.max(0, s - 1))} disabled={step === 0}>
                <ChevronLeft className="h-4 w-4" /> Back
              </Button>
              {step < steps.length - 1 ? (
                <Button onClick={() => setStep((s) => s + 1)}>
                  Next <ChevronRight className="h-4 w-4" />
                </Button>
              ) : (
                <Button onClick={submit} disabled={loading || !form.offer}>
                  {loading ? "Generating…" : "Generate campaign"}
                </Button>
              )}
            </div>
          </CardContent>
        </Card>
      </div>
    </AppShell>
  );
}
