# ApiGateway – Roteador de Requisições

Centraliza o acesso aos microserviços usando Ocelot. Implementa autenticação JWT e redireciona chamadas para EstoqueService e VendasService.

🔄 ApiGateway – Roteador de Requisições

Local: api-gateway/

Função: Roteia e centraliza o acesso aos serviços EstoqueService e VendasService. Baseado em Ocelot, que faz o roteamento HTTP e validação de tokens JWT.

📌 Funcionalidades

Roteamento de endpoints via ocelot.json.

Autenticação JWT centralizada.

Integração com Docker e RabbitMQ.

⚙️ Configuração

O arquivo principal é o ocelot.json.

Define rotas downstream e upstream para os serviços Estoque e Vendas.

📄 Exemplo de Rota
{
  "DownstreamPathTemplate": "/api/produtos",
  "DownstreamScheme": "http",
  "DownstreamHostAndPorts": [
    { "Host": "estoque-service", "Port": 5118 }
  ],
  "UpstreamPathTemplate": "/estoque/api/produtos",
  "UpstreamHttpMethod": [ "GET" ],
  "AuthenticationOptions": {
    "AuthenticationProviderKey": "Bearer",
    "AllowedScopes": []
  }
}

🚀 Execução

Porta: 5103

Endpoint de login: POST /login

Rotas principais: /estoque/{...}, /vendas/{...}