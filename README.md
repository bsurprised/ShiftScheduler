# Onyx Shift Scheduler
Onyx Shift Scheduler is a boilerplate ASP.NET Core project with clean architecture in mind. 
It uses Google OR Tools to solve the problem of scheduling shift between employees to support the business.

## Goals
The goal of this project is to provide a sample starter kit solution structure that can be used to build Domain-Driven Design (DDD)-based or simply a SOLID applications using .NET Core. 
The sample business idea in the starter kit can be used to automatically solve scheduling problems with multiple constraints.

## Tech
The project demonstrates a basic usage of the following:
- A loosely-coupled, dependency-inverted architecture, also known as **clean architecture**.
- A basic solution structure that can be used to build **Domain-Driven Design (DDD)**.
- Well-factored, **SOLID** pattern using .NET Core.
- **N-Tier architecture** (UI, Infrastructure, Core) plus Tests.
- Barebone API starter kit with Swagger support and **API versioning**.
- **IoC/DI** using StructureMap, the oldest in town.
- A **Repository** class with extensive routines as a base for the DAL layer of your entities.
- **Integration Tests**
- Visual Studio **Docker support** for faster and simpler tests.
- And more (DbContext Seeding, InMemory Databases, ...)

And a sample usage of the application, using Google OrTools Constraint Solver to handle employee shift scheduling. 

## Dependencies
The solution does not include every possible framework, tool, or feature that an enterprise application might need. It uses common, accessible technology for most business software developers. It doesn't include extensive support for logging or analytics, though these can all be added easily. The nature of this architecture is to support modularity and encapsulation. Most so many current dependencies can easily be swapped out for your technology of choice.

## Solution Structure
The solution includes the following projects.

### Core Project
The Core project has very few external dependencies. It is the shared center of the solution. The Core project includes things like, entities, dtos, interfaces, etc.

### Infrastructure Project
Most of your application's dependencies on external resources are implemented in Infrastructure project. The Infrastucture holds implementations of interfaces defined in Core. The sample also includes data access and domain event implementations.

### API Project
A consumable API with swagger implementation that can be used to deliver data to the frontend UI project. 

### UI Project
A very simple and basic Angular project coupled with the API to retrieve a sample schedule. Please note that this is just a demo project. It has been loaded with a csproject into Visual Studio for convenience and fast development. Please note that it must not be included in build configuarion, since there's nothing to build there. 

### Test Project
In a real application I will likely have separate test projects, organized based on the kind of test (unit, functional, integration, etc.) or by the project they are testing (Core, Infrastructure, Web). 
For this simple boilerplate, there is just one test project, with folders representing the projects being tested. The test project uses xunit, Moq and ASP.NET Core's TestHost.

## About Google OR-Tools

Google Operations Research and Optimization Tools (a.k.a., OR-Tools) is an open-source, fast and portable software suite for solving combinatorial optimization problems. It's written in C++, and this project uses wrappers for C# to unleash its powers.
The [Qoollo.OptTools.Core](https://www.nuget.org/packages/Qoollo.OptTools.Core/) has been used in this project for cross platform dotnetCore portability.

# Build
Instructions to build and run the project

## Tools
You need the following to build the project: 

- Main projects
```
Visual Studio 15.7.5 and above
.NET Core SDK 2.1
```

- Angular UI
```
@angular >= 6.1.0
@angular/cli >= 6.1.0
```

- Docker
You need `Latest Docker CE` switched to `linux` containers.

## Development

Run the `API project` from Visual Studio. The Swagger UI will be launched on port `44019` of the IIS Express.
Use `npm install` to install the missing packages. 
Run the `Web project` from the command line with `npm start` for a dev server, or `npm run hmr` for hot module replacement support. Navigate to `http://localhost:4200/`
If by any change, you get an error from the sass module, it might be that you opened the Visual Studio and not fetching all node modules correctly. Run a `npm rebuild node-sass` to fix the issue.

## Build and Docker

Load the `Onyx.ShiftScheduler.Docker.sln` solution instead for Visual Studio docker support and start the container. The containerized solution uses `9087` as the service port.
Either use `npm run build` or `ng build` to build the Angular project. The artifacts will be saved in `dist\` folder, or run the project by using the PowerShell script `up.ps1` provided in `docker\ui`.
Remember to use `docker-compose build` before running the script and starting a new container. 
The Angular project uses port `9089` in production mode and will consume the API on port `9087`.
