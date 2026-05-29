#!/bin/bash

sleep 15s

cd SecureBank

export AppSettings__BaseUrl=http://localhost:80
export AppSettings__StoreEndpoint__ApiUrl=http://127.0.0.1:5000/api/Store/
export AppSettings__StoreEndpoint__ApiToken=

export DatabaseConnections__SecureBankMSSQL__UserId=sa
export DatabaseConnections__SecureBankMSSQL__UserPass=Your_Password123
export DatabaseConnections__SecureBankMSSQL__Server=127.0.0.1
export DatabaseConnections__SecureBankMSSQL__ServerPort=5432
export DatabaseConnections__SecureBankMSSQL__Databbase=bank

export AppSettings__SmtpCredentials__Ip=127.0.0.1
export AppSettings__SmtpCredentials__Port=1025
export AppSettings__SmtpCredentials__Username=
export AppSettings__SmtpCredentials__Password=

export SeedingSettings__Seed=true
export SeedingSettings__Admin=admin@ssrd.io
export SeedingSettings__AdminPassword=admin
export SeedingSettings__UserPassword=test1

./SecureBank --urls http://0.0.0.0:80