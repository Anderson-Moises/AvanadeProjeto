using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Common.Messages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using VendasService.Services;

namespace VendasService.Mensagem
{
    public class ProdutoAtualizadoConsumer : BackgroundService
    {
        private readonly IPrecoService _precoService;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private const string QueueName = "produto-preco-queue";

        public ProdutoAtualizadoConsumer(IPrecoService precoService, string host = "rabbitmq")
        {
            _precoService = precoService;

            var factory = new ConnectionFactory { HostName = host, DispatchConsumersAsync = true };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var produto = JsonSerializer.Deserialize<ProdutoAtualizadoMessage>(message);

                if (produto != null)
                {
                    _precoService.AtualizarPreco(produto.ProdutoId, produto.Preco);
                    Console.WriteLine($"[PrecoService] Produto {produto.ProdutoId} atualizado: {produto.Preco:C2}");
                }

                _channel.BasicAck(ea.DeliveryTag, false);
                await Task.Yield();
            };

            _channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);
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