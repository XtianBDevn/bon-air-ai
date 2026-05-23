export const API_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:8080";

export type Campaign = {
  id: string;
  name: string;
  status: string;
  businessType: string;
  offer: string;
  targetAudience: string;
  location: string;
  tone: string;
  budget?: number;
  channels: string[];
  headline?: string;
  cta?: string;
  tokensUsed: number;
  progressPercent: number;
  createdAt: string;
  completedAt?: string;
  generated?: Record<string, unknown>;
};

export type Dashboard = {
  totalCampaigns: number;
  activeCampaigns: number;
  completedCampaigns: number;
  tokensUsedThisMonth: number;
  campaignsRemaining: number;
  plan: string;
  recentCampaigns: Campaign[];
  recentAssets: { id: string; assetType: string; title: string; content: string; createdAt: string }[];
  usageChart: { date: string; tokens: number }[];
};

export type Me = {
  id: string;
  email: string;
  fullName?: string;
  role: string;
  isPlatformAdmin: boolean;
  organization: {
    id: string;
    name: string;
    slug: string;
    plan: string;
    campaignLimit: number;
    campaignsUsedThisMonth: number;
    subscriptionStatus: string;
    whiteLabelEnabled: boolean;
  };
};

async function request<T>(path: string, token?: string, init?: RequestInit): Promise<T> {
  const headers: Record<string, string> = {
    "Content-Type": "application/json",
    ...(init?.headers as Record<string, string>),
  };
  if (token) headers.Authorization = `Bearer ${token}`;

  const res = await fetch(`${API_URL}${path}`, { ...init, headers, cache: "no-store" });
  if (!res.ok) {
    const err = await res.text();
    throw new Error(err || res.statusText);
  }
  return res.json() as Promise<T>;
}

export const api = {
  getDashboard: (token: string) => request<Dashboard>("/api/dashboard", token),
  getCampaigns: (token: string) => request<Campaign[]>("/api/campaigns", token),
  getCampaign: (token: string, id: string) => request<Campaign>(`/api/campaigns/${id}`, token),
  createCampaign: (token: string, body: unknown) =>
    request<Campaign>("/api/campaigns", token, { method: "POST", body: JSON.stringify(body) }),
  getMe: (token: string) => request<Me>("/api/auth/me", token),
  getBrandKit: (token: string) => request<Record<string, string>>("/api/settings/brand-kit", token),
  updateBrandKit: (token: string, body: unknown) =>
    request<Record<string, string>>("/api/settings/brand-kit", token, { method: "PUT", body: JSON.stringify(body) }),
  exportCampaign: (token: string, id: string, format: string) =>
    request<{ id: string; fileUrl: string }>(`/api/exports/campaigns/${id}?format=${format}`, token, { method: "POST" }),
  checkout: (token: string, plan: string) =>
    request<{ url: string }>("/api/billing/checkout", token, {
      method: "POST",
      body: JSON.stringify({
        plan,
        successUrl: `${typeof window !== "undefined" ? window.location.origin : ""}/dashboard?billing=success`,
        cancelUrl: `${typeof window !== "undefined" ? window.location.origin : ""}/pricing`,
      }),
    }),
  getPlans: () => request<{ id: string; name: string; price: number; campaigns: number; features: string[] }[]>("/api/billing/plans"),
  getAdminStats: (token: string) => request<{ totalUsers: number; totalOrganizations: number; totalCampaigns: number; totalTokens: number }>("/api/admin/stats", token),
};
