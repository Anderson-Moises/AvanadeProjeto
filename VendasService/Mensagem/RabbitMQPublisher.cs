using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Common.Messages;
using VendasService.Mensagem;

namespace VendasService.Services
{
    public class RabbitMQPublisher : IRabbitMQPublisher
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _queueName = "fila_pedidos";

        public RabbitMQPublisher()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "rabbitmq",
                UserName = "admin",
                Password = "123456"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
        }

        public void PublicarPedido(VendaMessage venda)
        {
            if (_channel == null || _connection == null)
                throw new InvalidOperationException("Conexão com RabbitMQ não foi inicializada corretamente.");

            var json = JsonSerializer.Serialize(venda);
            var body = Encoding.UTF8.GetBytes(json);

            // Propriedade para persistir mensagens
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;

            _channel.BasicPublish(
                exchange: "",
                routingKey: _queueName,
                basicProperties: properties,
                body: body
            );

            Console.WriteLine($"[RabbitMQPublisher] Pedido publicado com sucesso: {venda.ProdutoId}");
        }

        ~RabbitMQPublisher()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}
