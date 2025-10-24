# Common – Biblioteca Compartilhada

Contém DTOs, modelos, interfaces e utilitários usados por EstoqueService e VendasService.

🧱 Common – Biblioteca Compartilhada

Local: common/

Função: Centraliza componentes e bibliotecas compartilhadas entre EstoqueService e VendasService. Garante padronização e redução de código duplicado entre os microserviços.

📦 Estrutura

Dtos/ — Data Transfer Objects usados entre serviços.

Models/ — Modelos de dados compartilhados.

Interfaces/ — Contratos e interfaces comuns.

Utilities/ — Funções utilitárias como geração e validação de tokens.

⚙️ Principais Componentes

JwtUtils — Gera e valida tokens JWT.

VendaMessage — Modelo para enviar/receber mensagens via RabbitMQ entre Vendas e Estoque.

🔧 Uso

Referenciar Common.csproj nos microserviços.

Importar namespaces conforme necessário.