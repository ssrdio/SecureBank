import type { Metadata } from "next";
import "./globals.css";

export const metadata: Metadata = {
  title: "SecureBank",
  description: "SecureBank is an open source project in .NET core with some security flaws, its purpose is to learn how to write good code from the bad practices found during penetration testing.",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body>{children}</body>
    </html>
  );
}
