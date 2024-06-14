import { ModeToggle } from "./mode-toggle";
import { buttonVariants } from "./ui/button";

interface RouteProps {
    href: string;
    label: string;
  }
  
  const routeList: RouteProps[] = [
    {
      href: "#features",
      label: "Home",
    },
    {
      href: "#about",
      label: "About",
    },
    {
      href: "#login",
      label: "Login",
    },
    {
      href: "#signup",
      label: "Signup",
    },
  ];


export function NavContent() {
  return (
      <nav className="flex gap-2 py-2 border-b w-full">
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
