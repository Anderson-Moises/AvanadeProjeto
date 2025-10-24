# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copia apenas os csproj para restaurar dependências
COPY EstoqueService/EstoqueService.csproj ./EstoqueService/
COPY Common/Common.csproj ./Common/

# Restaura dependências
RUN dotnet restore EstoqueService/EstoqueService.csproj

# Copia o código fonte sem bin/ e obj/
COPY EstoqueService/. ./EstoqueService/
COPY Common/. ./Common/

# Publica
RUN dotnet publish EstoqueService/EstoqueService.csproj -c Release -o out

# Etapa final: runtime + dockerize
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Instala o dockerize v0.9.6 para aguardar o RabbitMQ
RUN apt-get update && apt-get install -y wget \
    && wget https://github.com/jwilder/dockerize/releases/download/v0.9.6/dockerize-linux-amd64-v0.9.6.tar.gz \
    && tar -C /usr/local/bin -xzvf dockerize-linux-amd64-v0.9.6.tar.gz \
    && rm dockerize-linux-amd64-v0.9.6.tar.gz

# Copia os arquivos publicados
COPY --from=build /app/out ./

# Aguarda RabbitMQ antes de iniciar o serviço
CMD ["dockerize", "-wait", "tcp://rabbitmq:5672", "-timeout", "180s", "dotnet", "EstoqueService.dll"]