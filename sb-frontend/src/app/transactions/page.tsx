import { Badge } from "@/components/ui/badge"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import React from 'react'

type Props = {}

export default function Transactions({}: Props) {
    return (
          <div className="p-8">
    <Card>
      <CardHeader className="px-7">
        <CardTitle>Transactions</CardTitle>
        <CardDescription>Recent transactions</CardDescription>
      </CardHeader>
      <CardContent>
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Sender</TableHead>
              <TableHead className="hidden sm:table-cell">Receiver</TableHead>
              <TableHead className="hidden sm:table-cell">Status</TableHead>
              <TableHead className="hidden md:table-cell">Transaction Date</TableHead>
              <TableHead className="text-right">Amount</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            <TableRow className="bg-accent">
              <TableCell>
                <div className="font-medium">Admin</div>
                <div className="hidden text-sm text-muted-foreground md:inline">
                  admin@ssrd.io
                </div>
              </TableCell>
              <TableCell className="hidden sm:table-cell">Bank of America</TableCell>
              <TableCell className="hidden sm:table-cell">
                <Badge className="text-xs" variant="secondary">
                  Fulfilled
                </Badge>
              </TableCell>
              <TableCell className="hidden md:table-cell">2023-06-23</TableCell>
              <TableCell className="text-right">$250.00</TableCell>
            </TableRow>
            <TableRow>
              <TableCell>
                <div className="font-medium">Admin</div>
                <div className="hidden text-sm text-muted-foreground md:inline">
                  admin@ssrd.io
                </div>
              </TableCell>
              <TableCell className="hidden sm:table-cell">HSBC</TableCell>
              <TableCell className="hidden sm:table-cell">
                <Badge className="text-xs" variant="outline">
                  Declined
                </Badge>
              </TableCell>
              <TableCell className="hidden md:table-cell">2023-06-24</TableCell>
              <TableCell className="text-right">$150.00</TableCell>
            </TableRow>
            <TableRow>
                          <TableCell>
                            <div className="font-medium">Liam Johnson</div>
                            <div className="hidden text-sm text-muted-foreground md:inline">
                              liam@example.com
                            </div>
                          </TableCell>
                          <TableCell className="hidden sm:table-cell">
                            admin@ssrd.io
                          </TableCell>
                          <TableCell className="hidden sm:table-cell">
                            <Badge className="text-xs" variant="secondary">
                              Fulfilled
                            </Badge>
                          </TableCell>
                          <TableCell className="hidden md:table-cell">
                            2023-06-23
                          </TableCell>
                          <TableCell className="text-right">$250.00</TableCell>
                        </TableRow>

          </TableBody>
        </Table>
      </CardContent>
                    </Card>
                </div> 
  )
}