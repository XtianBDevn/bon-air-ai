"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import {
  LayoutDashboard,
  Sparkles,
  Settings,
  CreditCard,
  Shield,
  LogOut,
  Menu,
} from "lucide-react";
import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/button";
import { useState } from "react";
import { createClient } from "@/lib/supabase/client";
import { useRouter } from "next/navigation";

const nav = [
  { href: "/dashboard", label: "Dashboard", icon: LayoutDashboard },
  { href: "/campaigns/new", label: "Generate", icon: Sparkles },
  { href: "/settings", label: "Settings", icon: Settings },
  { href: "/pricing", label: "Billing", icon: CreditCard },
  { href: "/admin", label: "Admin", icon: Shield },
];

export function AppShell({ children, orgName }: { children: React.ReactNode; orgName?: string }) {
  const pathname = usePathname();
  const router = useRouter();
  const [open, setOpen] = useState(false);

  const signOut = async () => {
    const supabase = createClient();
    await supabase.auth.signOut();
    router.push("/login");
  };

  return (
    <div className="min-h-screen gradient-mesh flex">
      <aside
        className={cn(
          "fixed inset-y-0 left-0 z-40 w-64 border-r border-zinc-800/80 glass p-4 flex flex-col transition-transform lg:translate-x-0 lg:static",
          open ? "translate-x-0" : "-translate-x-full"
        )}
      >
        <Link href="/dashboard" className="flex items-center gap-2 px-2 py-3 mb-6">
          <div className="h-8 w-8 rounded-lg bg-indigo-600 flex items-center justify-center text-xs font-bold">BA</div>
          <div>
            <p className="font-semibold text-sm">Bon Air Media</p>
            <p className="text-xs text-zinc-500 truncate max-w-[140px]">{orgName ?? "Campaigns"}</p>
          </div>
        </Link>
        <nav className="flex-1 space-y-1">
          {nav.map((item) => {
            const active = pathname.startsWith(item.href);
            return (
              <Link
                key={item.href}
                href={item.href}
                onClick={() => setOpen(false)}
                className={cn(
                  "flex items-center gap-3 rounded-lg px-3 py-2.5 text-sm transition-colors",
                  active ? "bg-indigo-600/20 text-indigo-300" : "text-zinc-400 hover:text-zinc-100 hover:bg-zinc-800/50"
                )}
              >
                <item.icon className="h-4 w-4" />
                {item.label}
              </Link>
            );
          })}
        </nav>
        <Button variant="ghost" className="justify-start gap-3 text-zinc-400" onClick={signOut}>
          <LogOut className="h-4 w-4" />
          Sign out
        </Button>
      </aside>
      {open && (
        <div className="fixed inset-0 z-30 bg-black/60 lg:hidden" onClick={() => setOpen(false)} />
      )}
      <div className="flex-1 flex flex-col min-w-0">
        <header className="lg:hidden flex items-center justify-between p-4 border-b border-zinc-800">
          <Button variant="ghost" size="icon" onClick={() => setOpen(true)}>
            <Menu className="h-5 w-5" />
          </Button>
          <span className="font-semibold text-sm">Bon Air Campaigns</span>
          <div className="w-10" />
        </header>
        <main className="flex-1 p-4 lg:p-8 overflow-auto">{children}</main>
      </div>
    </div>
  );
}
