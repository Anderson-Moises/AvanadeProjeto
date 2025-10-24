using Microsoft.EntityFrameworkCore;
using VendasService.Models;

namespace VendasService.Data
{
    public class PedidoContext : DbContext
    {
        public PedidoContext(DbContextOptions<PedidoContext> options) : base(options) { }

        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoItem> ItensPedidos { get; set; }
    }
}
