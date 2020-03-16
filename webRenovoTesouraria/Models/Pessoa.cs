using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace webRenovoTesouraria.Models
{
    [Table("fin_pessoa")]
    public class Pessoa
    {
        [Column("id_pessoa")]
        public int ID { get; set; }

        [Column("tx_pessoa")]
        public string Nome { get; set; }
    }
}
