using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VendasService.Dtos;

namespace VendasService.Services
{
    public interface IProdutoService
{
    Task<IEnumerable<ProdutoDto>> ListarAsync(string token);
    Task<ProdutoDto?> ObterPorIdAsync(int id, string token);
    Task<bool> RemoverAsync(int id, int quantidade, string token);
    Task<bool> ReporInterno(int produtoId, int quantidade, string token);
}


}