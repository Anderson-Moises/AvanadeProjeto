using Microsoft.EntityFrameworkCore;
using VendasService.Models;

namespace VendasService.Data
{
    public class VendasContext : DbContext
    {
        public VendasContext(DbContextOptions<VendasContext> options) : base(options) { }

        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoItem> ItensPedidos { get; set; }
    }
}
