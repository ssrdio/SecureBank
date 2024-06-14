
import { MoveRight, Github } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import Image from "next/image";
import HeroImage from "../../public/hero-img/hero-image.jpeg";

export default function Hero() {
  return (
    <div className="w-full py-20 lg:p-5 lg:py-5">
                {/* <Image
                  src={HeroImage}
                  alt="Hero background"
                  layout="fill"
                  objectFit="cover"
                  quality={100}
                  className="flex relative w-full h-full -z-[1]"
        /> */}
      
      <div className="container mx-auto">
        <div className="grid grid-cols-1 gap-8 items-center lg:grid-cols-2">
              <div className="flex gap-4 flex-col">
                  <div>
                    <Badge variant="outline">OWASP® Top Ten Security Flaws Included</Badge>
                  </div>
                  <div className="flex gap-4 flex-col">
                                    <h1 className="text-5xl md:text-7xl max-w-lg tracking-tight text-left font-regular text-foreground">
                                        Welcome to award-winning banking
                    </h1>
                        <p className="text-xl leading-relaxed tracking-tight text-muted-foreground max-w-md text-left">
                          See why we're tusted by over a million customers across Europe.
                    </p>
                  </div>
                  <div className="flex flex-row gap-2">
                        <Button  className="gap-2 p-4">
                            Open an Account
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