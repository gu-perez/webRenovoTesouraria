using System.ComponentModel.DataAnnotations.Schema;

namespace webRenovoTesouraria.Models
{
    [Table("fin_tipo_dinheiro_entrada")]
    public class TipoEntrada
    {
        [Column("id_tipo_dinheiro_entrada")]
        public int ID { get; set; }

        [Column("tx_tipo_dinheiro_entrada")]
        public string Nome { get; set; }
    }
}