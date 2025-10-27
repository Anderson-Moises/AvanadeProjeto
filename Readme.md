🏗️ AvanadeProjeto – Sistema Distribuído de Estoque e Vendas

Este repositório contém a implementação do desafio técnico da trilha Avanade – Back-end com .NET e IA, oferecida pela Digital Innovation One (DIO) em parceria com a Avanade.

O projeto simula um sistema de e-commerce distribuído, construído com arquitetura de microserviços, utilizando mensageria, autenticação JWT e orquestração via Docker, seguindo boas práticas do ecossistema .NET moderno.

🚀 Visão Geral

O sistema é composto por quatro microserviços principais, executando de forma independente e orquestrados via Docker Compose:

Serviço	Função Principal
ApiGateway	Controla o roteamento entre os microserviços e centraliza a autenticação (via Ocelot).
EstoqueService	Gerencia o ciclo de vida dos produtos (cadastro, atualização e controle de estoque).
VendasService	Controla os pedidos, integração com o estoque e o processamento via RabbitMQ.
Common	Contém classes e componentes compartilhados entre os serviços, incluindo autenticação JWT.

Cada serviço possui banco de dados próprio, garantindo isolamento e independência entre os domínios.

🧩 Tecnologias Utilizadas

.NET 9 (C#) – Base do desenvolvimento backend

Entity Framework Core (MySQL) – ORM para persistência de dados

Docker – Containerização e orquestração dos serviços

RabbitMQ – Mensageria e enfileiramento assíncrono

JWT (JSON Web Token) – Autenticação e autorização seguras

Ocelot – API Gateway para roteamento centralizado

Avanade + DIO – Trilha oficial “Back-end com .NET e IA”

🧠 Arquitetura

A comunicação entre os serviços segue o padrão event-driven, garantindo baixo acoplamento e alta escalabilidade.
O fluxo principal ocorre assim:

O VendasService cria um pedido e envia uma mensagem para a fila RabbitMQ.

O EstoqueService consome essa mensagem, atualiza o estoque e confirma o processamento.

Caso o pedido seja cancelado, o processo inverso é executado automaticamente.

Essa abordagem permite que cada serviço evolua independentemente, sem dependência direta de chamadas síncronas.

📂 Estrutura de Pastas Atualizada
AvanadeProjeto/
│
├── ApiGateway/
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   ├── Dockerfile
│   ├── ocelot.json
│   ├── Program.cs
│   └── Readme_ApiGateway.md
│
├── Common/
│   ├── Auth/
│   ├── JWT/
│   ├── Messages/
│   ├── SharedModels/
│   └── Readme_Common.md
│
├── EstoqueService/
│   ├── Controllers/
│   ├── Data/
│   ├── DTOs/
│   ├── Mensagem/
│   ├── Migrations/
│   ├── Models/
│   ├── Services/
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   ├── Dockerfile
│   ├── Program.cs
│   └── Readme_Estoque.md
│
├── VendasService/
│   ├── Controllers/
│   ├── Data/
│   ├── DTOs/
│   ├── Mensagem/
│   ├── Migrations/
│   ├── Models/
│   ├── Services/
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   ├── Dockerfile
│   ├── Program.cs
│   └── Readme_Vendas.md
│
├── mysql-initi/
│   └── init-databases.sql
│
├── docker-compose.yml
├── paths.txt
└── README.md

🐳 Execução via Docker
🔹 Subir todo o ambiente
docker-compose up --build

🔹 Containers esperados
Serviço	Porta Local	Porta Container	Descrição
ApiGateway	5103	80	Roteamento e autenticação central
EstoqueService	5118	5118	Controle de estoque
VendasService	5119	5119	Gerenciamento de pedidos
MySQL	3307	3306	Banco de dados relacional
RabbitMQ	15672 / 5672	15672 / 5672	Painel e mensageria

Credenciais RabbitMQ (modo Docker):
Usuário: guest
Senha: guest

🔹 Endpoints principais

Swagger Estoque: http://localhost:5118/swagger

Swagger Vendas: http://localhost:5119/swagger

RabbitMQ: http://localhost:15672

API Gateway: http://localhost:5103

🧪 Execução Local (sem Docker)

Certifique-se de que o MySQL local está ativo na porta 3306.

Rode o RabbitMQ localmente (http://127.0.0.1:15672
).

Ajuste as conexões nos appsettings.Development.json.

Execute os projetos na seguinte ordem:

Common -> EstoqueService -> VendasService -> ApiGateway


⚠️ Evite rodar local e Docker simultaneamente, pois utilizam as mesmas portas e podem causar conflitos.

🔄 Migrations Local

Para garantir que o banco local esteja atualizado, execute as migrations sempre que houver alterações no modelo de dados:

Abra o terminal na pasta do projeto do serviço (EstoqueService ou VendasService).

Execute:

dotnet ef database update


O Entity Framework criará/atualizará as tabelas conforme as migrations existentes.

⚠️ Lembre-se: no Docker, o banco reseta se o container for destruído, então mantenha os dados críticos fora do container ou configure volumes permanentes.

🧾 Testes de Funcionalidade
Cenário	Serviço	Ação	Resultado Esperado
Cadastro de Produto	EstoqueService	POST /api/produtos	Produto criado e persistido no banco
Criação de Pedido	VendasService	POST /api/pedidos	Mensagem publicada no RabbitMQ
Cancelamento de Pedido	VendasService	PUT /api/pedidos/{id}/cancelar	Estoque restaurado
Consumo da Fila	EstoqueService	RabbitMQ	Atualização automática do estoque
🔄 Próximos Passos

✅ Testes finais de integração RabbitMQ

✅ Atualização do README principal

🔄 Publicação no GitHub (branch principal)

🚀 Preparar documentação para entrega final na trilha Avanade – Back-end com .NET e IA

💼 Autor

Anderson Moisés
Desenvolvedor em formação • Trilha Avanade – Back-end com .NET e IA (DIO)
Universidade Estácio / Unigranrio

“De dev fullstack a líder estratégico — visão de carreira: DevSecOps → CEO.”

🔗 LinkedIn – Anderson Moisés