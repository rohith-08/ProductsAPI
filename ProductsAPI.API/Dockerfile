# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["ProductsAPI.API/ProductsAPI.API.csproj", "ProductsAPI.API/"]
COPY ["ProductsAPI.Application/ProductsAPI.Application.csproj", "ProductsAPI.Application/"]
COPY ["ProductsAPI.Domain/ProductsAPI.Domain.csproj", "ProductsAPI.Domain/"]
COPY ["ProductsAPI.Infrastructure/ProductsAPI.Infrastructure.csproj", "ProductsAPI.Infrastructure/"]

RUN dotnet restore "ProductsAPI.API/ProductsAPI.API.csproj"

COPY . .

WORKDIR /src/ProductsAPI.API
RUN dotnet build "ProductsAPI.API.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "ProductsAPI.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=publish /app/publish .

EXPOSE 8080
EXPOSE 8081

ENTRYPOINT ["dotnet", "ProductsAPI.API.dll"]