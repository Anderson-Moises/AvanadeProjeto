using Microsoft.EntityFrameworkCore;
using VendasService.Data;
using VendasService.Dtos;
using VendasService.Mensagem;
using VendasService.Models;
using Common.Messages;

namespace VendasService.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly PedidoContext _context;
        private readonly IRabbitMQPublisher _publisher;
        private readonly IProdutoService _produtoService; // ← Http service agora

        public PedidoService(PedidoContext context, IRabbitMQPublisher publisher, IProdutoService produtoService, IPrecoService precoService)
        {
            _context = context;
            _publisher = publisher;
            _produtoService = produtoService;
        }

        public async Task<Pedido> CriarPedidoAsync(PedidoCreateDto dto, string token = "")
    {
        var pedido = new Pedido { Itens = new List<PedidoItem>() };
        decimal total = 0;

        foreach (var itemDto in dto.Itens)
        {
            decimal preco = 0;
            string nome = $"Produto {itemDto.ProdutoId}";

            try
            {
                var produto = await _produtoService.ObterPorIdAsync(itemDto.ProdutoId, token);
                if (produto != null)
                {
                    preco = produto.Preco;
                    nome = produto.Nome;
                }
            }
            catch (HttpRequestException)
            {
                Console.WriteLine($"⚠ Estoque offline para Produto {itemDto.ProdutoId}, preço usado: 0");
            }

            pedido.Itens.Add(new PedidoItem
            {
                ProdutoId = itemDto.ProdutoId,
                Nome = nome,
                Quantidade = itemDto.Quantidade,
                Preco = Math.Round(preco, 2)
            });

            total += Math.Round(preco * itemDto.Quantidade, 2);

            try
            {
                _publisher.PublicarPedido(new VendaMessage
                {
                    ProdutoId = itemDto.ProdutoId,
                    Quantidade = itemDto.Quantidade,
                    Preco = Math.Round(preco, 2)
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠ Não foi possível enviar mensagem para RabbitMQ: {ex.Message}");
            }
        }

        pedido.Total = total;
        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();

        return pedido;
    }


        public async Task<Pedido?> ObterPorIdAsync(int id)
        {
            return await _context.Pedidos
                .Include(p => p.Itens)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> CancelarPedidoAsync(int id, string token = "")
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Itens)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null)
                return false;

            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<Pedido>> ListarAsync()
        {
            return await _context.Pedidos
                .Include(p => p.Itens)
                .ToListAsync();
        }
    }
}
