
using Microsoft.AspNetCore.Mvc;
using VendasService.Dtos;
using VendasService.Models;
using VendasService.Services;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;

namespace VendasService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // 🔒 Exige token JWT para acessar qualquer rota
    public class PedidosController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;
        private readonly IProdutoService _produtoService;
       

        public PedidosController(IPedidoService pedidoService, IProdutoService produtoService, IHttpClientFactory httpClientFactory)
        {
            _pedidoService = pedidoService;
            _produtoService = produtoService;
           
        }

        [HttpGet("produtos")]
        public async Task<ActionResult<IEnumerable<ProdutoDto>>> ListarProdutos()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            try
            {
                // Chama o EstoqueService via serviço HTTP com o token
                var produtos = await _produtoService.ListarAsync(token);
                return Ok(produtos);
            }

            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Erro ao consultar produtos: {ex.Message}");
                return StatusCode(503, "Erro ao consultar produtos no EstoqueService");
            }

        }

        [HttpGet]
        public async Task<ActionResult<List<Pedido>>> GetPedidos()
        {
            Console.WriteLine("🔍 Tentando listar pedidos...");
            var pedidos = await _pedidoService.ListarAsync();
            Console.WriteLine($"✅ Pedidos retornados: {pedidos.Count}");
            return Ok(pedidos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Pedido>> GetPedido(int id)
        {
            if (id <= 0) // ✅ Corrigido: validação simplificada
                return BadRequest(new { message = "O id deve ser maior que zero." });

            var pedido = await _pedidoService.ObterPorIdAsync(id);
            if (pedido == null)
                return NotFound(new { message = $"Pedido {id} não encontrado." });

            return Ok(pedido);
        }

        [HttpPost]
        public async Task<ActionResult<Pedido>> CriarPedido([FromBody] PedidoCreateDto dto)
        {
            if (dto == null || dto.Itens == null || !dto.Itens.Any())
                return BadRequest(new { message = "O pedido deve conter pelo menos um item." });


            // ✅ Propaga token JWT para EstoqueService
            var token = Request.Headers["Authorization"].ToString();
            
            foreach (var item in dto.Itens)
            {
                if (item.ProdutoId <= 0 || item.Quantidade <= 0)
                    return BadRequest(new { message = "ProdutoId e Quantidade devem ser maiores que zero." });

                // ✅ Chamada ao EstoqueService via serviço
                var produto = await _produtoService.ObterPorIdAsync(item.ProdutoId, token);
                if (produto == null)
                    return BadRequest(new { message = $"Produto {item.ProdutoId} não encontrado no estoque." });

                if (produto.Quantidade < item.Quantidade)
                    return BadRequest(new { message = $"Estoque insuficiente para o produto {item.ProdutoId}." });
            }

            try
            {
                var pedido = await _pedidoService.CriarPedidoAsync(dto, token);
                return CreatedAtAction(nameof(GetPedido), new { id = pedido.Id }, pedido);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("cancelar/{id}")]
        public async Task<ActionResult> CancelarPedido(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "O id deve ser maior que zero." });

            
            // ✅ Recupera o token do cabeçalho
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");


            var sucesso = await _pedidoService.CancelarPedidoAsync(id, token);
            if (!sucesso)
                return NotFound(new { message = $"Pedido {id} não encontrado." });

            return NoContent();
        }
    }
}
