
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using EstoqueService.Models;
using EstoqueService.Dtos;
using EstoqueService.Services;
using Common.Jwt;

namespace EstoqueService.Controllers
{
   
    // ✅ Controlador para gerenciar produtos (protegido por JWT)
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // 🔒 Exige token JWT para acessar
    public class ProdutosController : ControllerBase
    {
        private readonly IProdutoService _produtoService;

        public ProdutosController(IProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

        /// <summary>
        /// Lista todos os produtos
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos()
        {
            var produtos = await _produtoService.ListarAsync();
            return Ok(produtos);
        }

        /// <summary>
        /// Busca um produto pelo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Produto>> GetProduto(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "O id deve ser maior que zero." });

            var produto = await _produtoService.ObterPorIdAsync(id);
            if (produto == null)
                return NotFound(new { message = $"Produto {id} não encontrado." });

            return Ok(produto);
        }

        /// <summary>
        /// Adiciona ou atualiza um produto
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Produto>> PostProduto([FromBody] ProdutoCreateDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "O corpo da requisição não pode ser vazio." });

            var produto = await _produtoService.AdicionarOuAtualizarAsync(dto);
            return Ok(produto);
        }

        /// <summary>
        /// Atualiza a quantidade em estoque
        /// </summary>
        [HttpPut("{id}/estoque")]
        public async Task<IActionResult> AtualizarEstoque(int id, [FromBody] int quantidade)
        {
            if (id <= 0 || quantidade < 0)
                return BadRequest(new { message = "Id ou quantidade inválida." });

            var produto = await _produtoService.AtualizarQuantidade(id, quantidade);
            if (produto == null)
                return NotFound(new { message = $"Produto {id} não encontrado." });

            return NoContent();
        }

        /// <summary>
        /// Remove um produto ou reduz quantidade
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduto(int id, [FromQuery] int? quantidade = null)
        {
            if (id <= 0)
                return BadRequest(new { message = "O id deve ser maior que zero." });

            var removido = await _produtoService.RemoverAsync(id, quantidade);
            if (!removido)
                return NotFound(new { message = $"Produto {id} não encontrado." });

            return NoContent();
        }

        /// <summary>
        /// Reposição interna (uso interno, não exposto na documentação)
        /// </summary>
        [HttpPut("{id}/repor")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ReporProduto(int id, [FromQuery] int quantidade)
        {
            var produtoAtual = await _produtoService.ObterPorIdAsync(id);
            if (produtoAtual == null)
                return NotFound(new { message = $"Produto {id} não encontrado." });

            var produtoAtualizado = await _produtoService.ReporInterno(id, quantidade);
            if (produtoAtualizado == null)
                return NotFound(new { message = $"Falha ao atualizar o produto {id}." });

            return Ok(new { message = $"Produto {produtoAtualizado.Nome} reabastecido com {quantidade} unidades." });
        }
    }
}
