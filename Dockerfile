 
# Sử dụng image runtime cho .NET 8.0
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5283

# Sử dụng image SDK để build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BackendTaskManagement.csproj", "."]
RUN dotnet restore "BackendTaskManagement.csproj"
COPY . .
RUN dotnet build "BackendTaskManagement.csproj" -c Release -o /app/build

# Publish ứng dụng
FROM build AS publish
RUN dotnet publish "BackendTaskManagement.csproj" -c Release -o /app/publish

# Tạo image cuối
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BackendTaskManagement.dll"]