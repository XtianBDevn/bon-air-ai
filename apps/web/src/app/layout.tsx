import type { Metadata } from "next";
import { Inter } from "next/font/google";
import "./globals.css";
import { Providers } from "@/components/providers";

const inter = Inter({
  variable: "--font-inter",
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: {
    default: "Bon Air Media Campaigns — AI Campaign OS",
    template: "%s | Bon Air Media Campaigns",
  },
  description:
    "AI-powered campaign operating system for agencies, law firms, restaurants, and local businesses. Generate ads, emails, SEO, and landing pages in minutes.",
  openGraph: {
    title: "Bon Air Media Campaigns",
    description: "The AI campaign operating system for modern agencies.",
    type: "website",
  },
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="en" className="dark">
      <body className={`${inter.variable} antialiased min-h-screen`}>
        <Providers>{children}</Providers>
      </body>
    </html>
  );
}
