using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace VendasService.Services
{
    public interface IPrecoService
    {
        public decimal ObterPreco(int produtoId);
        void AtualizarPreco(int produtoId, decimal preco);
    }
}