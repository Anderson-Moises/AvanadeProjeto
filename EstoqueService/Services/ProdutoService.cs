
using EstoqueService.Models;
using EstoqueService.Data;
using EstoqueService.Dtos;
using Microsoft.EntityFrameworkCore;

namespace EstoqueService.Services
{
    public class ProdutoService : IProdutoService
    {
        private readonly EstoqueContext _context;

        public ProdutoService(EstoqueContext context)
        {
            _context = context;
        }

        public async Task<List<Produto>> ListarAsync()
        {
            return await _context.Produtos.ToListAsync();
        }

        public async Task<Produto?> ObterPorIdAsync(int id)
        {
            return await _context.Produtos.FindAsync(id);
        }

        public async Task<Produto> AdicionarOuAtualizarAsync(ProdutoCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome))
                throw new ArgumentException("O nome do produto é obrigatório.");

            if (dto.Quantidade < 0) // ✅ Corrigido: substituído &lt; por <
                throw new ArgumentException("A quantidade não pode ser negativa.");

            var nomeNormalizado = dto.Nome.Trim().ToLower();

            var existente = await _context.Produtos
                .FirstOrDefaultAsync(p => p.Nome.ToLower() == dto.Nome.ToLower()); // ✅ Corrigido: &gt; para >

            if (existente != null)
            {
                existente.Quantidade += dto.Quantidade;
                existente.Descricao = dto.Descricao;
                existente.Preco = dto.Preco;

                _context.Produtos.Update(existente);
                await _context.SaveChangesAsync();
                return existente;
            }

            var novo = new Produto
            {
                Nome = dto.Nome,
                Descricao = dto.Descricao,
                Preco = dto.Preco,
                Quantidade = dto.Quantidade
            };

            _context.Produtos.Add(novo);
            await _context.SaveChangesAsync();
            return novo;
        }

        public async Task<Produto?> AtualizarQuantidade(int id, int quantidade)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null) return null;

            produto.Quantidade = quantidade; // substitui
            _context.Produtos.Update(produto);
            await _context.SaveChangesAsync();

            return produto;
        }

        public async Task<Produto?> ReporInterno(int produtoId, int quantidade)
        {
            var produto = await _context.Produtos.FindAsync(produtoId);
            if (produto == null) return null;

            Console.WriteLine($"[ReporInterno] Produto {produtoId} devolvendo {quantidade} unidades");

            produto.Quantidade += quantidade; // devolve a quantidade no estoque

            _context.Produtos.Update(produto);
            await _context.SaveChangesAsync();

            return produto;
        }

        public async Task<bool> RemoverAsync(int id, int? quantidade = null)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null) return false;

            if (quantidade == null)
            {
                _context.Produtos.Remove(produto); // remove o produto inteiro
            }
            else
            {
                if (produto.Quantidade < quantidade) // ✅ Corrigido: &lt; para <
                    throw new InvalidOperationException("Estoque insuficiente para remover.");

                produto.Quantidade -= quantidade.Value; // baixa apenas a quantidade especificada
                _context.Produtos.Update(produto);
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
