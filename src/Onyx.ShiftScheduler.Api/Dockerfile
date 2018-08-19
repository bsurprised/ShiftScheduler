FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY src/Onyx.ShiftScheduler.Api/Onyx.ShiftScheduler.Api.csproj src/Onyx.ShiftScheduler.Api/
COPY src/Onyx.ShiftScheduler.Core/Onyx.ShiftScheduler.Core.csproj src/Onyx.ShiftScheduler.Core/
COPY src/Onyx.ShiftScheduler.Infrastructure/Onyx.ShiftScheduler.Infrastructure.csproj src/Onyx.ShiftScheduler.Infrastructure/
RUN dotnet restore src/Onyx.ShiftScheduler.Api/Onyx.ShiftScheduler.Api.csproj
COPY . .
WORKDIR /src/src/Onyx.ShiftScheduler.Api
RUN dotnet build Onyx.ShiftScheduler.Api.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Onyx.ShiftScheduler.Api.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Onyx.ShiftScheduler.Api.dll"]
