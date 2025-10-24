using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EstoqueService.Models
{
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18,2)")]
        public decimal Preco { get; set; }
        public int Quantidade { get; set; }
    }
}