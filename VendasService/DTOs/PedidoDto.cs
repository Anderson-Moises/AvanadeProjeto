using System;
using System.Collections.Generic;

namespace VendasService.Dtos
{
    public class PedidoDto
    {
        public int Id { get; set; }
        public List<PedidoItemDto> Itens { get; set; } = new();
        public decimal Total { get; set; }
        public DateTime Data { get; set; }
    }
}
