# Onyx Shift Scheduler

Onyx Shift Scheduler is a boilerplate ASP.NET Core project with clean architecture in mind. 
It uses Google OR Tools to solve the problem of scheduling shift between employees to support the business.

# Goals

The goal of this project is to provide a sample starter kit solution structure that can be used to build Domain-Driven Design (DDD)-based or simply a SOLID applications using .NET Core. 
The sample business idea in the starter kit can be used to automatically solve scheduling problems with multiple constraints.

# Dependencies

The solution does not include every possible framework, tool, or feature that an enterprise application might need. It uses common, accessible technology for most business software developers. It doesn't include extensive support for logging or analytics, though these can all be added easily. The nature of this architecture is to support modularity and encapsulation. Most so many current dependencies can easily be swapped out for your technology of choice.

# Solution Structure

The solution includes the following projects.

## Core Project

The Core project has very few external dependencies. It is the shared center of the solution. The Core project includes things like, entities, dtos, interfaces, etc.

## Infrastructure Project

Most of your application's dependencies on external resources are implemented in Infrastructure project. The Infrastucture holds implementations of interfaces defined in Core. The sample also includes data access and domain event implementations.

## API Project

A consumable API with swagger implementation that can be used to deliver data to the frontend UI project. 

## UI Project

A very simple and basic Angular project coupled with the API to retrieve a sample schedule.

## Test Project

In a real application I will likely have separate test projects, organized based on the kind of test (unit, functional, integration, etc.) or by the project they are testing (Core, Infrastructure, Web). 
For this simple boilerplate, there is just one test project, with folders representing the projects being tested. The test project uses xunit, Moq and ASP.NET Core's TestHost.

