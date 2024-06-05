
import { MoveRight, Github } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";

export default function Hero() {
    return (
  <div className="w-full  py-20 lg:py-40">
    <div className="container mx-auto">
      <div className="grid grid-cols-1 gap-8 items-center lg:grid-cols-2">
        <div className="flex gap-4 flex-col">
          <div>
            <Badge variant="outline">OWASP® Top Ten Security Flaws Included</Badge>
          </div>
          <div className="flex gap-4 flex-col">
                            <h1 className="text-5xl md:text-7xl max-w-lg tracking-tight text-left font-regular text-foreground">
                                SecureBank
            </h1>
                            <p className="text-xl leading-relaxed tracking-tight text-muted-foreground max-w-md text-left">
                                An educational open source project in .NET core, with some security flaws. Can you spot them?
            </p>
          </div>
          <div className="flex flex-row gap-2">
                <Button  className="gap-2 p-4">
                    Learn More
                    <MoveRight className="w-4 h-4" />
            </Button>
                <Button
                  className="gap-2 p-4" variant="outline">
                    GitHub
                    <Github className="w-4 h-4" />
            </Button>
          </div>
        </div>
            <div className="bg-muted rounded-md aspect-square">
        </div>
      </div>
    </div>
        </div>
        )
};