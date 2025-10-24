using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace VendasService.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public List<PedidoItem> Itens { get; set; } = new();
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }
        public DateTime Data { get; set; } = DateTime.Now;
    }
}
