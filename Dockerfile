# Çalışma zamanı için temel imaj
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build işlemi için SDK imajı
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Sadece .csproj dosyasını kopyala ve bağımlılıkları yükle (Cache optimizasyonu için)
COPY ["ChatAppApi.csproj", "./"]
RUN dotnet restore "./ChatAppApi.csproj"

# Tüm kodları kopyala ve derle
COPY . .
RUN dotnet build "ChatAppApi.csproj" -c Release -o /app/build

# Yayınlama (Publish) işlemi
FROM build AS publish
RUN dotnet publish "ChatAppApi.csproj" -c Release -o /app/publish

# Son aşama: Çalıştırma
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatAppApi.dll"]