using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webRenovoTesouraria.ViewModels
{
    public class EntradaResponse
    {
        //public EntradaResponse() { }

        //CATEGORIA
        public decimal VlTotalDizimo { get; set; }
        public decimal VlTotalOferta { get; set; }
        public decimal VlTotalMissoes { get; set; }
        public decimal VlTotalReforma { get; set; }

        public decimal VlTotalCategoria { get; set; }

        //TIPO
        public decimal VlTotalNotas { get; set; }
        public decimal VlTotalMoedas { get; set; }
        public decimal VlTotalCheque { get; set; }
        public decimal VlTotalCredito { get; set; }
        public decimal VlTotalDebito { get; set; }
        public decimal VlTotalTransf { get; set; }

        public decimal VlTotalTipo { get; set; }
    }
}
