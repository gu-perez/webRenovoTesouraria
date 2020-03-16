using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace webRenovoTesouraria.Models
{
    [Table("fin_head_entrada")]
    public class CabecalhoEntrada
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_head_entrada")]
        public int ID { get; set; }

        [Column("tx_header1")]
        public string Nome1 { get; set; }

        [Column("tx_header2")]
        public string Nome2 { get; set; }

        [Column("dt_entrada")]
        public DateTime DataInclusao { get; set; }

    }
}
