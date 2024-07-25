"use client"

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
import React, { useEffect, useState } from 'react'

type Transaction = {
  senderId: string,
  receiverId: string,
  dateTime: string,
  amount: number,
  reason: string,
  id: string,
}

type Props = {}

export default function Transactions({ }: Props) {
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchTransactions = async () => {
      try {
        const response = await fetch('http://localhost:1337/api/Transaction/GetTransactions');
        if (!response.ok) throw new Error('Failed to fetch transactions');
        const data = await response.json();
        setTransactions(data);
      } catch (error : any) {
        setError(error.message);
      } finally {
        setLoading(false);
      }
    };

    fetchTransactions();
  }, []);

  if (loading) return <p>Loading...</p>;
  if (error) return <p>Error: {error}</p>;
  
    return (
          <div className="p-8">
    <Card>
      <CardHeader className="px-7">
        <CardTitle>Transactions</CardTitle>
        <CardDescription>Recent transactions</CardDescription>
      </CardHeader>
      <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Sender</TableHead>
                <TableHead className="hidden sm:table-cell">Receiver</TableHead>
                <TableHead className="hidden sm:table-cell">Reason</TableHead>
                <TableHead className="hidden md:table-cell">Transaction Date</TableHead>
                <TableHead className="text-right">Amount</TableHead>
                <TableHead className="text-right">View</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {transactions.map(transaction => (
                <TableRow key={transaction.id}>
                  <TableCell>
                    <div className="font-medium">{transaction.senderId}</div>
                  </TableCell>
                  <TableCell className="hidden sm:table-cell">{transaction.receiverId}</TableCell>
                  <TableCell className="hidden sm:table-cell">{transaction.reason}</TableCell>
                  <TableCell className="hidden md:table-cell">{transaction.dateTime}</TableCell>
                  <TableCell className="text-right">{transaction.amount.toFixed(2)}€</TableCell>
                  <TableCell className="text-right">
                    <a href={`/Transaction/Details/${transaction.id}`}>
                      <button className="btn btn-primary btn-sm">Details</button>
                    </a>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
                    </Card>
                </div> 
  )
}