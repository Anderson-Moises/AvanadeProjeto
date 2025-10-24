using Microsoft.EntityFrameworkCore;

namespace VendasService.Models
{
    public class PedidoItem
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        [Precision(18, 2)] // precision total = 18, scale = 2 (duas casas decimais)
        public decimal Preco { get; set; }
    }
}
