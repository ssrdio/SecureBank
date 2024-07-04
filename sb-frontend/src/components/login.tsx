"use client"

import React, { useState } from 'react'
import Image from "next/image"
import Link from "next/link"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import LoginImage from "../../public/hero-img/hero-image-2.jpeg";

type Props = {}

export default function LoginForm({ }: Props) {
    const [email, setEmail] = useState('')
    const [password, setPassword] = useState('')
    const [error, setError] = useState('')
  
    const validateEmail = (email: string) => {
      const re = /[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,3}/
      return re.test(email)
    }
  
    const handleSubmit = async (event: { preventDefault: () => void }) => {
      event.preventDefault()
      if (!validateEmail(email)) {
        setError('Invalid email address')
        return
      }
      try {
          const response = await fetch('http://localhost:1337/api/Auth/Login', { 
                method: 'POST',
                headers: {
                  'Content-Type': 'application/json',
                },
                body: JSON.stringify({ UserName: email, Password: password }),
              })
        window.location.href = "/Transactions"
      } catch (err) {
        setError('Login failed. Please check your credentials and try again.')
      }
    }

    
    return (
    <div className="w-full lg:grid lg:min-h-[600px] lg:grid-cols-2 xl:min-h-[800px]">
      <div className="flex items-center justify-center py-12">
        <div className="mx-auto grid w-[350px] gap-6">
          <div className="grid gap-2 text-center">
            <h1 className="text-3xl font-bold">Login</h1>
            <p className="text-balance text-muted-foreground">
              Enter your email below to login to your account
            </p>
          </div>
          <form onSubmit={handleSubmit} className="grid gap-4">
            <div className="grid gap-2">
              <Label htmlFor="email">Email</Label>
              <Input
                id="email"
                type="email"
                placeholder="m@example.com"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
              />
            </div>
            <div className="grid gap-2">
              <div className="flex items-center">
                <Label htmlFor="password">Password</Label>
                <Link
                  href="/forgot-password"
                  className="ml-auto inline-block text-sm underline"
                >
                  Forgot your password?
                </Link>
              </div>
                            <Input id="password"
                                type="password"
                                value={password}
                                onChange={(e) => setPassword(e.target.value)}
                                required
                             />
            </div>
                        <Button
                            type="submit"
                            className="w-full">
              Login
            </Button>
          </form>
          <div className="mt-4 text-center text-sm">
            Don&apos;t have an account?{" "}
            <Link href="#" className="underline">
              Sign up
            </Link>
          </div>
        </div>
      </div>
      <div className="hidden bg-muted lg:block lg:mx-auto">
        <Image
          src={LoginImage}
          alt="Image"
          width="1920"
          height="1080"
          className="h-full w-full object-cover dark:brightness-[0.2] dark:grayscale"
        />
      </div>
    </div>
      )
    }