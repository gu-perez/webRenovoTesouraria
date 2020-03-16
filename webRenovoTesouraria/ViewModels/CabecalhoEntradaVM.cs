using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webRenovoTesouraria.ViewModels
{
    public class CabecalhoEntradaVM
    {
        public string ID { get; set; }
        public string Nome1 { get; set; }
        public string Nome2 { get; set; }
        public DateTime DataInclusao { get; set; }

        public DetalhesVM Detalhes { get; set; }

    }
}
