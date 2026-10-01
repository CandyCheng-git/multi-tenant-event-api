FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/MultiTenantEventApi/MultiTenantEventApi.csproj", "src/MultiTenantEventApi/"]
RUN dotnet restore "src/MultiTenantEventApi/MultiTenantEventApi.csproj"
COPY . .
RUN dotnet publish "src/MultiTenantEventApi/MultiTenantEventApi.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "MultiTenantEventApi.dll"]
