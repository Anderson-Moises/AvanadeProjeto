# Avanade – Desafio Técnico Microserviços

Este repositório contém a implementação do desafio técnico, simulando um sistema de e-commerce com arquitetura de microserviços.

🏗️ README – Raiz do Projeto
🚀 Sistema de Gestão de Estoque e Vendas Distribuído
🧩 Tecnologias Utilizadas

.NET 9 (C#) — Base do desenvolvimento backend.

Entity Framework Core (MySQL) — ORM para persistência de dados.

Docker — Containerização e orquestração dos serviços.

RabbitMQ — Mensageria e enfileiramento para alta demanda.

JWT (JSON Web Token) — Autenticação e autorização segura.

Ocelot — Gateway para roteamento centralizado e autenticação de APIs.

🧠 Visão Geral

O sistema é composto por quatro serviços principais, todos executando em containers Docker e se comunicando entre si via HTTP e RabbitMQ:

Serviço	Função Principal
ApiGateway	Controla o acesso e roteamento entre os microserviços.
EstoqueService	Gerencia o ciclo de vida dos produtos.
VendasService	Gerencia pedidos e integração com o estoque.
Common	Contém classes e componentes compartilhados entre os serviços.

📦 Cada serviço é independente, tem seu próprio banco de dados, e se comunica por meio do RabbitMQ e do Ocelot Gateway.
O sistema foi desenhado para suportar alta demanda e ser facilmente escalável em ambientes distribuídos.
