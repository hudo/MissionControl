# MissionControl

[![Nuget Package](https://badgen.net/nuget/v/missioncontrol)](https://www.nuget.org/packages/missioncontrol/)
[![Build Status](https://dev.azure.com/hhudolet/MissionControl/_apis/build/status/MC%20Sample%20Web%20App%20Pipeline)](https://dev.azure.com/hhudolet/MissionControl/_build/latest?definitionId=2)

CLI as a middleware for your web apps and microservices

Example web site with MC console: https://mc-sample.azurewebsites.net/

**UNDER DEVELOPMENT - alpha version**

# Why

Often we want to add some kind of interaction with our application. For example: 

- purge cache
- check some background tasks
- state of objects or queues
- resources consumption
- add new user to database, update database records 
- etc

MissionControl serves CLI web terminal on a dedicated URL and executes custom commands created by application developer. There are some generic commands already bundled with this library, like *list-commands*, help and diagnostics. Custom commands are easy to add by just implementing an interface or adding an attribute, and MissonControl will handle the rest. 

# How it works

![Diagram](docs/diagram1.png "High level diagram")  

- just a middleware for any .NET Core app
- serves CLI UI interface on dedicated URL (/mc by default)
- automatically registers all commands/handlers in the app

## Installation and configuration

In Startup.cs register services and add a middleware:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // registers internal dependencies, scans this assemly for commands
    services.AddMissionControl();
}

public void Configure(IApplicationBuilder app)
{
    // registers /mc endpoint
    app.UseMissingControl();
}

```

With custom configuration:  
- all assemblies that contains commands and command handlers
- Url where CLI is served
- authentication callback - add your own request authentication to prevent execution of commands in production


```csharp
public void ConfigureServices(IServiceCollection services)
{
    // assemblies where commands and handlers are located
    services.AddMissionControl(typeof(PurgeCacheCommand).Assembly, typeof(ListActiveUsersCommand).Assembly);
}

public void Configure(IApplicationBuilder app)
{
    app.UseMissingControl(opt =>
    {
        opt.Url = "/mc";
        opt.Authentication = req => true; // this allows all requests, but add authentication for production deployments! 
    });
}
```

### Basic example

Simple command and handler example: 

```csharp
// Gets exposed to CLI as a command:
// > say-hi -name=Batman -foo=123
[CliCommand("say-hi", "Say Hi!")]
public class SayHiCommand : CliCommand
{
    public string Name { get; set; }

    [CliArg(required: false, help: "Attribute is optional")]
    public int Foo { get;set; }
}

public class SayHiHandler: ICliCommandHandler<SayHiCommand>
{
    // IService is resolved with standard asp.net ioc
    public SayHiHandler(IService service) { }

    public async Task<CliResponse> Handle(SayHiCommand command)
    {
        await DoSomeAsyncWork();
        return new TextResponse($"Computer says hi to {command.Name}!");
    }
}

```

Invoked in CLI console with:

```
> say-hi -name=Hackerman
Computer says hi to Hackerman!
> _
```

Available response types:
- TextResponse
- ErrorResponse
- MultipleResponses
- TableResponse

### Multiple responses

For multiple responses we can yield strings:
```csharp
[CliCommand("multi-resp", "Multiple responses example")]
public class MultipleResponsesCommand : CliCommand {  }

public class MultipleResponsesHandler : ICliCommandHandler<MultipleResponsesCommand>
{
    public async Task<CliResponse> Handle(MultipleResponsesCommand command)
    {
        await service.DoSomething();
        
        // here we can "stream" partial responses back to the CLI terminal
        // use async callback in MultipleResponses constructor:
        return new MultipleResponses(async yield =>
        {
            await yield.ReturnAsync("Response 1");
            await Task.Delay(2000);
            await yield.ReturnAsync("Response 2");
            await Task.Delay(2000);
            await yield.ReturnAsync("Response 3 - done.");
        });
    }
}

```

CLI output, with 2sec delay between each response line:

```
> multi-resp
Response 1
Response 2
Response 3 - done.
> _
```

## Included commands

Standard commands bundled with MC:

- list-commands: displayes a list of registered commands
- server-info: system information

every command can be invoked with **-help** argument. Handler will not be executed, but list of command arguments and its descriptions will be displayed in the console. 

## Technologies

.NET Standard 2.0 / Core 2.0   
Typescript

## Nuget

https://www.nuget.org/packages/MissionControl

[Release Notes](RELEASE-NOTES.md)

## Contributors

[Ivana Dukic](https://github.com/idukic)  

Thanks [David Guerin](https://github.com/dguerin) for name idea!  

# State of development

- [x] basic data structure and model of DTO commands and handlers
- [x] ASP.NET Core middleware (custom URL and auth callback) 
- [x] Basic routing of web requests to internal action
- [x] Route: default HTML 
- [x] Route: static content
- [x] Route: CLI requests
- [x] Command/handlers scanning
- [x] Invoke of requested commnad handler
- [x] Handler pre/post pipeline behavior
- [x] Args: help (done), skip (done), required (done)
- [ ] Authentication (username/pass, JWT bearer)
- [ ] UI CSS standards
- [ ] proper WebPack + watch front-end dev configuration 
- [ ] JS: UI layout and structure
- [x] JS: input parsing
- [ ] JS: ajax proxy
- [ ] JS: response rendering (text, warnings, errors)
- [ ] JS: get previous command (history)
- [ ] JS: unit tests
- [ ] control specific service instance in a cluster (epic)
- [x] workflows/sagas/muliple response objects (epic)
- [ ] Refactor: argument parser (implement tokenizer)
- [ ] Refactor: better command/handler contracts for easier/streamlined development
- [x] Included commands: ping, list-commands, command help
- [x] DevOps: build, test and deployment pipeline
- [ ] DevOps: automatic nuget release
- [x] DevOps: sample site deployment pipeline

## Diagrams

[Sequence diagrams](docs/sequence-diagrams.MD), use https://www.websequencediagrams.com/ to render.