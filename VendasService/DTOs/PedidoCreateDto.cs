namespace VendasService.Dtos
{
    public class PedidoCreateDto
    {
        public List<PedidoItemCreateDto> Itens { get; set; } = new();
    }

    public class PedidoItemCreateDto
    {
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
        
    }
}
