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

- just a middleware for any .NET Core app
- serves CLI UI interface on dedicated URL (/mc by default)
- automatically registers all commands/handlers in the app

## Technologies

.NET Standard 2.0 / Core 2.0

# State of development

- [x] basic data structure and model of DTO commands and handlers
- [x] ASP.NET Core middleware (custom URL and auth callback) 
- [x] Basic routing of web requests to internal action
- [x] Route: default HTML 
- [x] Route: static content
- [x] Route: CLI requests
- [x] Command/handlers scanning
- [x] Invoke of requested commnad handler
- [ ] UI CSS standards
- [ ] JS: UI layout and structure
- [ ] JS: input parsing
- [ ] JS: ajax proxy
- [ ] JS: response rendering
- [ ] JS: unit tests
- [ ] cluster epic (control many services from one console)
- [ ] Refactor: argument parser (implement tokenizer)
- [ ] Refactor: better command/handler contracts for easier/streamlined development
- [ ] Included commands: ping, list-commands, command help
- [ ] DevOps: build pipeline
- [ ] DevOps: sample site deployment pipeline