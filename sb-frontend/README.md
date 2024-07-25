# SecureBank - Front End 

## Overview
SecureBank is an open source project in .NET core with some security flaws. Its purpose is to learn how to write a good code from the bad practices found during penetration testing.

**SecureBank**'s objective is to show how developers fail to protect their environment due to a lack of knowledge about the applied ecosystem. 

We will determine the docker deployment of micro service solutions and where developers misconfigured their systems. Not only security in the configuration, we still have OWASP Top 10* Web Application Security risks out in the wild. Therefore, we will also present those vulnerabilities in the SecureBank application.

## User Story 

As a security researcher or developer, I want to interact with a realistic-looking banking application, So that I can learn about common security vulnerabilities and how to mitigate them.


## Project Brief (Front End)

This project aims to redesign the frontend of SecureBank, an open-source educational tool showcasing security vulnerabilities. The redesign will leverage a modern web  development tech stack (TypeScript, React, Next.js, Tailwind CSS), to create a visually appealing and user-friendly interface that emulates a real banking website, enabling users to explore and learn from common security flaws in a seemingly safe environment. 

The new frontend will include a login/signup flow, a dashboard with account overview and recent transactions, a transaction history view, and potentially additional features like a product store and search functionality.

## Application

### Existing Architecture 
![[Pasted image 20240604165831.png]]


### Tech Stack - New Front End 

Typescript, React, Next.js 14, Tailwind CSS, Shadcn 


```
admin@ssrd.io:admin
developer@ssrd.io:test
yoda@ssrd.io:test
tester@ssrd.io:test
```


## Routes 

### Pre-authorisation 
- Landing Page
	- Top Nav 
		- Home, About, Login, Signup 
	- Project Description 
	- Hero Banner 
		- Github Repo link 
	- Footer 
- Login
	- Username 
	- Password 
	- Forgot your password 
	- Register as new user (link to signup page)
- Signup 
	- Email 
	- Password
	- Confirm Password
	- T&Cs 
	- Register button 
### Post-auth 

``` 

// working 
admin@ssrd.io:admin

// couldn't log in with the credentials below, maybe something to look into 

developer@ssrd.io:test
yoda@ssrd.io:test
tester@ssrd.io:test
```

* Transactions
	* Sender, Receiver, Transaction Date, Amount (EUR), Reason
	* Ability to upload transactions 
* Store
	* Name, Price, Description, Times bought
* Search 

## Next Steps (TODO List)

- [x] Fork main repo and create Frontend Branch
- [x] Initialise NextJs App
- [x] Build basic landing page 
- [x] Create pre-auth routes 
- [x] Build Login Page
- [x] Build Signup Page
- [x] Create post-auth routes 
- [x] Build Basic Dashboard View
- [x] Develop Transactions Overview
- [ ] Create Store Page
- [ ] Integrate with SecureBank back end endpoints 

## References

https://github.com/ssrdio/SecureBank
https://ssrd.gitbook.io/securebank

