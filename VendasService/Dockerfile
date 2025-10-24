# Dockerfile para VendasService
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copia apenas os csproj para restaurar dependências
COPY VendasService/VendasService.csproj ./VendasService/
COPY Common/Common.csproj ./Common/

# Restaura dependências
RUN dotnet restore VendasService/VendasService.csproj

# Copia o código fonte sem bin/ e obj/
COPY VendasService/. ./VendasService/
COPY Common/. ./Common/

# Publica
RUN dotnet publish VendasService/VendasService.csproj -c Release -o out

# Imagem final runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "VendasService.dll"]
