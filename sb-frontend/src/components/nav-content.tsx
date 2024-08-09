import { ModeToggle } from "./mode-toggle";
import { buttonVariants } from "./ui/button";
import Logo from "../../public/logo/cover-2.png"
import Image from "next/image";

interface RouteProps {
    href: string;
    label: string;
  }
  
  const routeList: RouteProps[] = [
    {
      href: "/",
      label: "Home",
    },
    {
      href: "/#about",
      label: "About",
    },
    {
      href: "/login",
      label: "Login",
    },
    {
      href: "/signup",
      label: "Signup",
    },
  ];


export function NavContent() {
  return (
    <nav className="flex gap-2 py-2 border-b w-full justify-between">
      <div className="flex items-center gap-8 max-w-48">
      <Image
        src={Logo}
        alt="logo"
        sizes="100vw"
        style={{
          width: 'auto',
          height: 'auto',
        }}
      />
      </div>
    {routeList.map((route: RouteProps, i) => (
      <a
        rel="noreferrer noopener"
        href={route.href}
        key={i}
        className={`text-[17px] ${buttonVariants({
          variant: "ghost",
        })}`}
      >
        {route.label}
      </a>
    ))}
          
        <ModeToggle />
        
  </nav>
  )
}
