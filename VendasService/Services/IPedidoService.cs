using VendasService.Dtos;
using VendasService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VendasService.Services
{
    public interface IPedidoService
{
    Task<Pedido> CriarPedidoAsync(PedidoCreateDto dto, string token);
    Task<bool> CancelarPedidoAsync(int id, string token);
    Task<Pedido?> ObterPorIdAsync(int id);
    Task<List<Pedido>> ListarAsync();
}
}
