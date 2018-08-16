# MissionControl
CLI for your web apps and microservices

**UNDER HEAVY DEVELOPMENT**

# Why

Often we want to add some kind of interaction with our application. For example: 

- check some background tasks
- state of objects or queues
- resources consumption
- add new user to database, update database records 
- etc

and all that without building admin HTML UI.   

MissionControl acts as a CLI UI Terminal, proxying commands to handlers built by application developer. 


# How it works

![Diagram](docs/diagram1.png "High level diagram")  

- serves CLI interface on dedicated URL (/mc by default)
- scans and invokes handlers
- proxy output back to console

## Technologies

.NET Standard 2.0

# State of development

- [ ] basic data structure and model of DTO commands and handlers
- [x] ASP.NET Core middleware (custom URL and auth callback) 
- [ ] Basic routing of web requests to internal action
- [x] Route: default HTML 
- [x] Route: static content
- [ ] Route: CLI requests
- [ ] Command/handlers scanning
- [ ] Invoke of requested commnad handler
- [ ] UI CSS standards
- [ ] JS: UI layout and structure
- [ ] JS: input parsing
- [ ] JS: ajax proxy
- [ ] JS: response rendering
- [ ] cluster epic (control many services from one console)