# SecureBank

SecureBank is FinTech application which contains all [OWASP TOP 10](https://owasp.org/www-project-top-ten/) security vulnerabilities alog with some other security flaws found in real-world applications.

## Environment

- `AppSettings:Ctf:Enabled` - If you want to run in ctf mode set it to `true` else set it to `false`. (Default `false`)
- `AppSettings:Ctf:Seed` - Seed used to generate ctf challenge flags. (Default `icEYG435oN`)
- `SecureBank:Seed` - Set it `true` if you want to seed SecureBank database with users and transactions.
- `SecureBank:Seed:Password`- With which password should users be seeded. (Default `Password1!`).
- `SecureBank:Seed:Admin` - Which username should be used for seeding admin.
- `SecureBank:Seed:AdminPassword` - Which password should be seeded for admin password.
- `StoreAPI:Seed` - Set it to `true` if you want to seed storeapi database with.
- `FTPServerIp` - Set it to host ip

## CTF - mode 

If you run in `CTF` mode zip file with challenges and flags will be generated when you start your container (in folder `/SecureBank/Ctf`). This file can be used to import challenges into [CTFd](https://github.com/CTFd/CTFd). Don't forget to remove this file otherwise, users might be able to get it (`Path traversal` challenge).

## Running docker image
It takes around a minute to start up all services inside the docker container.

With docker-compose:
```
version: '3'

services:
    securebank:
        image: ssrd/securebank
        environment: 
            - AppSettings:Ctf:Enabled=false
            - AppSettings:Ctf:Seed=example
            - FTPServerIp=Your server ip
        ports: 
            - 80:80
            - 5000:5000
            - 1080:1080
            - 20-21:20-21
            - 50000-50100:50000-50100
        volumes: 
            - ./ctf:/SecureBank/Ctf
            - .data:/var/opt/mssql/data
```
To run in docker: 

`docker run -p 80:80 -p 5000:5000 -p 1080:1080 ssrd/securebank`

To run in docker CTF mode:

`docker run -p 80:80 -p 5000:5000 -e 'AppSettings:Ctf:Enabled=true' -e 'AppSettings:Ctf:Seed=example'  ssrd/securebank`

## Email
E-mail server will automatically start and will listen on port 1080. To check registration/password recovery emails can navigate to `http://{server}:1080`.

Note: Running in CTF mode will not run the e-mail server. All accounts will be automatically confirmed. 

## FTP
If you want to include FTP challenge you nedd to set `FTPServerIp` to your server ip, and expose ports `20-21`, `50000-50100`.