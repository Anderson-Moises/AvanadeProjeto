using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Messages
{
    public class ProdutoAtualizadoMessage
    {
        public int ProdutoId { get; set; }
        public decimal Preco { get; set; }
    }
}