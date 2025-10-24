
# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copia os arquivos de projeto
COPY ApiGateway/ApiGateway.csproj ./ApiGateway/
COPY Common/Common.csproj ./Common/

# Restaura dependências
RUN dotnet restore ApiGateway/ApiGateway.csproj

# Copia o código fonte
COPY ApiGateway/. ./ApiGateway/
COPY Common/. ./Common/

# Publica o projeto
RUN dotnet publish ApiGateway/ApiGateway.csproj -c Release -o out

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copia os arquivos publicados
COPY --from=build /app/out ./

# Expõe a porta padrão
EXPOSE 80

# Executa o serviço
ENTRYPOINT ["dotnet", "ApiGateway.dll"]
