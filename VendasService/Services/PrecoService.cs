using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VendasService.Services
{
    public class PrecoService : IPrecoService
    {
        // Mantém preços em memória (pode ser persistido se necessário)
        private readonly ConcurrentDictionary<int, decimal> _precos = new();

        public decimal ObterPreco(int produtoId)
        {
            return _precos.TryGetValue(produtoId, out var preco) ? preco : 0;
        }

        public void AtualizarPreco(int produtoId, decimal preco)
        {
            _precos[produtoId] = Math.Round(preco, 2);
        }
    }
}