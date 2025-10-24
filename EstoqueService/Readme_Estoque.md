# EstoqueService – Microserviço de Gestão de Estoque

Gerencia produtos e estoque, preparado para integração com VendasService via RabbitMQ e API Gateway.

📦 EstoqueService

Local: estoque-service/

Função: Gerencia o ciclo de vida dos produtos no estoque.

Requer autenticação JWT para qualquer requisição.

Integra com o RabbitMQ para receber mensagens de atualização de pedidos vindas do VendasService.

🧩 Endpoints Principais
Método	Rota	Descrição
POST	/api/produtos	Cria um novo produto (não aceita valores nulos ou negativos; se já existir produto com mesmo nome — mesmo que com diferença de maiúsculas/minúsculas — as quantidades são somadas)
GET	/api/produtos	Retorna a lista de produtos
GET	/api/produtos/{id}	Retorna um produto específico
PUT	/api/produtos/{id}	Atualiza a quantidade em estoque
DELETE	/api/produtos/{id}	Remove um produto do estoque

🧠 Regras de Negócio

Nomes de produtos são tratados de forma case-insensitive.

Se o produto for adicionado novamente, o sistema soma a quantidade ao existente.

Se a exclusão de quantidade for maior que o disponível, o produto é removido.

Nenhum produto pode ter valor nulo ou negativo.

Acesso somente com token JWT válido.