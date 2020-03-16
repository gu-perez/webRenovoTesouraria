using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace webRenovoTesouraria.Models
{
    [Table("fin_categoria_entrada")]
    public class CategoriaEntrada
    {
        [Column("id_categoria_entrada")]
        public int ID { get; set; }

        [Column("tx_categoria_entrada")]
        public string Nome { get; set; }
    }
}
