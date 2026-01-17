# Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish SharpHack.Server/SharpHack.Server.csproj -c Release -o /app/publish

# Runtime
FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 2323
ENTRYPOINT ["dotnet", "SharpHack.Server.dll", "2323"]
