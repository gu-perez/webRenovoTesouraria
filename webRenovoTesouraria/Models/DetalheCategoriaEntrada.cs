using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace webRenovoTesouraria.Models
{
    [Table("fin_detalhe_entrada")]
    public class DetalheCategoriaEntrada
    {
        [Column("id")]
        public int ID { get; set; }

        [Column("id_head_entrada")]
        public int ID_HeadCategoria { get; set; }

        [Column("id_pessoa")]
        public int IdPessoa { get; set; }

        [Column("id_categoria_entrada")]
        public int IdCategoria { get; set; }

        [Column("vl_valor")]
        public decimal Valor { get; set; }

        [Column("tx_obs")]
        public string Obs { get; set; }

    }
}
