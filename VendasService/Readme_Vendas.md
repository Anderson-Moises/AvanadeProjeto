# VendasService – Microserviço de Gestão de Vendas

Gerencia pedidos de venda, valida estoque com EstoqueService e comunica via RabbitMQ.

💰 VendasService

Local: vendas-service/

Função: Gerencia os pedidos e se comunica com o EstoqueService.

Utiliza RabbitMQ para enviar mensagens assíncronas e controlar o enfileiramento em alta demanda.

Requer token JWT obtido pelo EstoqueService para acessar seus recursos.

🧩 Endpoints Principais
Método	Rota	Descrição
GET	/api/pedidos	Lista todos os pedidos realizados
GET	/api/pedidos/{id}	Consulta pedido por ID
POST	/api/pedidos	Cria um novo pedido e envia mensagem ao RabbitMQ
DELETE	/api/pedidos/{id}	Cancela o pedido e devolve as quantidades de volta ao estoque

🧠 Regras de Negócio

Para cada pedido criado, é enviada uma mensagem ao RabbitMQ para o estoque atualizar as quantidades.

Em caso de cancelamento, as quantidades são restauradas no estoque.

O sistema é protegido por autenticação via JWT.

Integração direta com o EstoqueService via API Gateway e RabbitMQ.