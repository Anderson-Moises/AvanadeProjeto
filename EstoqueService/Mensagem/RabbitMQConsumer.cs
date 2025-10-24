using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using EstoqueService.Services;
using Common.Messages;

namespace EstoqueService.Mensagem
{
    public class RabbitMQConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly string _host;
        private readonly int _port;
        private readonly string _username;
        private readonly string _password;
        private IConnection _connection = null!;
        private IModel _channel = null!;
        private const string QueueName = "fila_pedidos"; // ✅ igual ao publisher

        public RabbitMQConsumer(
            string host,
            int port,
            string username,
            string password,
            IServiceProvider serviceProvider)
        {
            _host = host;
            _port = port;
            _username = username;
            _password = password;
            _serviceProvider = serviceProvider;

            ConnectRabbitMQ();
        }

        private void ConnectRabbitMQ()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _host,
                Port = _port,
                UserName = _username,
                Password = _password,
                DispatchConsumersAsync = true
            };

            int maxRetries = 30;
            int delaySeconds = 2;

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    _connection = factory.CreateConnection();
                    _channel = _connection.CreateModel();

                    _channel.QueueDeclare(
                        queue: QueueName,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                    );

                    _channel.BasicQos(0, 1, false);
                    Console.WriteLine("[RabbitMQConsumer] Conectado ao RabbitMQ.");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[RabbitMQConsumer] Tentativa {i + 1} falhou: {ex.Message}");
                    Thread.Sleep(delaySeconds * 1000);
                }
            }

            if (_connection == null || !_connection.IsOpen)
                throw new Exception("Falha ao conectar ao RabbitMQ após várias tentativas.");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine($"[RabbitMQConsumer] Mensagem recebida: {message}");

                try
                {
                    var venda = JsonSerializer.Deserialize<VendaMessage>(message);

                    if (venda is null)
                    {
                        Console.WriteLine("[RabbitMQConsumer] Mensagem inválida ou nula.");
                        _channel.BasicNack(ea.DeliveryTag, false, false); // descarta mensagem
                        return;
                    }

                    using var scope = _serviceProvider.CreateScope();
                    var produtoService = scope.ServiceProvider.GetRequiredService<IProdutoService>();

                    await produtoService.RemoverAsync(venda.ProdutoId, venda.Quantidade);

                    Console.WriteLine($"[RabbitMQConsumer] Estoque atualizado: Produto {venda.ProdutoId}");

                    // simula processamento pesado
                    Console.WriteLine("[Teste] Pausa de 5s antes do ack...");
                    await Task.Delay(5000);

                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[RabbitMQConsumer] Erro ao processar mensagem: {ex.Message}");
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };

            _channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);
            Console.WriteLine("[RabbitMQConsumer] Aguardando mensagens...");
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}
