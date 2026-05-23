"use client";

import { useEffect } from "react";
import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { useQueryClient } from "@tanstack/react-query";
import { API_URL, api, type Campaign, type Dashboard } from "@/lib/api";

type CampaignUpdatePayload = {
  id?: string;
  Id?: string;
  status?: string;
  Status?: string;
  progressPercent?: number;
  ProgressPercent?: number;
  tokensUsed?: number;
  TokensUsed?: number;
  headline?: string | null;
  Headline?: string | null;
};

type CampaignUpdate = {
  id: string;
  status?: string;
  progressPercent?: number;
  tokensUsed?: number;
  headline?: string | null;
};

const terminalStatuses = new Set(["completed", "failed"]);

function normalizeCampaignUpdate(payload: CampaignUpdatePayload): CampaignUpdate | null {
  const id = payload.id ?? payload.Id;
  if (!id) return null;

  return {
    id,
    status: payload.status ?? payload.Status,
    progressPercent: payload.progressPercent ?? payload.ProgressPercent,
    tokensUsed: payload.tokensUsed ?? payload.TokensUsed,
    headline: payload.headline ?? payload.Headline,
  };
}

function mergeCampaignUpdate(campaign: Campaign, update: CampaignUpdate): Campaign {
  return {
    ...campaign,
    status: update.status ?? campaign.status,
    progressPercent: update.progressPercent ?? campaign.progressPercent,
    tokensUsed: update.tokensUsed ?? campaign.tokensUsed,
    headline: update.headline ?? campaign.headline,
  };
}

export function useCampaignUpdates(token?: string | null) {
  const queryClient = useQueryClient();

  useEffect(() => {
    if (!token) return;

    let active = true;
    const hubUrl = `${API_URL.replace(/\/$/, "")}/hubs/campaigns`;
    const connection = new HubConnectionBuilder()
      .withUrl(hubUrl, { accessTokenFactory: () => token })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Warning)
      .build();

    connection.on("CampaignUpdated", (payload: CampaignUpdatePayload) => {
      const update = normalizeCampaignUpdate(payload);
      if (!update) return;

      queryClient.setQueriesData<Campaign>({ queryKey: ["campaign", update.id] }, (campaign) =>
        campaign ? mergeCampaignUpdate(campaign, update) : campaign,
      );

      queryClient.setQueriesData<Dashboard>({ queryKey: ["dashboard"] }, (dashboard) =>
        dashboard
          ? {
              ...dashboard,
              recentCampaigns: dashboard.recentCampaigns.map((campaign) =>
                campaign.id === update.id ? mergeCampaignUpdate(campaign, update) : campaign,
              ),
            }
          : dashboard,
      );

      void queryClient.invalidateQueries({ queryKey: ["dashboard"] });

      if (update.status && terminalStatuses.has(update.status.toLowerCase())) {
        void queryClient.invalidateQueries({ queryKey: ["campaign", update.id] });
      }
    });

    const startConnection = async () => {
      try {
        const me = await queryClient.fetchQuery({
          queryKey: ["me", token],
          queryFn: () => api.getMe(token),
          staleTime: 60_000,
        });

        if (!active) return;

        await connection.start();

        if (!active) {
          await connection.stop();
          return;
        }

        await connection.invoke("JoinOrganization", me.organization.id);
      } catch (error) {
        if (process.env.NODE_ENV !== "production") {
          console.warn("Campaign realtime unavailable", error);
        }
      }
    };

    void startConnection();

    return () => {
      active = false;
      connection.off("CampaignUpdated");
      void connection.stop();
    };
  }, [queryClient, token]);
}
