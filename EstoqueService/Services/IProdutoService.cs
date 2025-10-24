using EstoqueService.Models;
using EstoqueService.Dtos;

namespace EstoqueService.Services
{
    public interface IProdutoService
    {
        Task<List<Produto>> ListarAsync();
        Task<Produto?> ObterPorIdAsync(int id);
        Task<Produto> AdicionarOuAtualizarAsync(ProdutoCreateDto dto);
        Task<Produto?> ReporInterno(int id, int quantidade);
        Task<Produto?> AtualizarQuantidade(int id, int quantidade);
        Task<bool> RemoverAsync(int id, int? quantidade = null);
    }
}
