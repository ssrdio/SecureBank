
import { MoveRight, Github } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import Image from "next/image";
import HeroImage from "../../public/hero-img/hero-image.jpeg";

export default function Hero() {
  return (
    <div className="w-full py-10 lg:p-10 lg:py-20 relative">
      <Image
        src={HeroImage}
        alt="Hero background"
        placeholder="blur"
        quality={50}
        className="-z-[1] object-cover m-0"
        fill={true}
        
      />
      
      <div className="container mx-auto">
        <div className="grid grid-cols-1 gap-8 items-center lg:grid-cols-1">
              <div className="flex gap-4 flex-col backdrop-brightness-75 p-10">
                  <div className="flex gap-4 flex-col">
                                    <h1 className="text-5xl md:text-7xl max-w-lg text-left font-regular text-primary-foreground">
                                        Welcome to award winning banking
                    </h1>
                        <p className="text-md leading-relaxed tracking-tight text-primary-foreground max-w-md text-left">
                          See why we're tusted by over a million customers across Europe.*
                    </p>
                  </div>
                  <div className="flex flex-row gap-2">
                        <Button  className="gap-2 p-4">
                            Create Account
                            <MoveRight className="w-4 h-4" />
                    </Button>
                        <Button
                          className="gap-2 p-4" variant="outline">
                            Sign-in
                    </Button>
                  </div>
              </div>
            </div>
        </div>
        </div>
        )
};