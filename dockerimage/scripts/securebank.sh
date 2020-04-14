#!/bin/bash

sleep 30s

cd SecureBank

export AppSettings__StoreEndpoint__ApiUrl=http://127.0.0.1:5000/api/Store/
export AppSettings__StoreEndpoint__ApiToken=

if test $ctfMode = 'false'; then
    export AppSettings__SmtpCredentials__Ip=127.0.0.1
    export AppSettings__SmtpCredentials__Port=1025
    export AppSettings__SmtpCredentials__Username=
    export AppSettings__SmtpCredentials__Password=
else
    export AppSettings__SmtpCredentials=-
fi

if [ ! -z "$FTPServerIp" ]; then
    export AppSettings__LegalURL="ftp://${FTPServerIp}/legal.html"
else
    export AppSettings__LegalURL=-
fi

export DatabaseConnections__SecureBankMSSQL__UserId=sa
export DatabaseConnections__SecureBankMSSQL__UserPass=Your_Password123
export DatabaseConnections__SecureBankMSSQL__Server=127.0.0.1
export DatabaseConnections__SecureBankMSSQL__ServerPort=1433
export DatabaseConnections__SecureBankMSSQL__Databbase=bank

./SecureBank --urls http://0.0.0.0:80