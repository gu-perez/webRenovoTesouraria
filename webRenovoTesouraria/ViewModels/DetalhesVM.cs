using System.Collections.Generic;
using webRenovoTesouraria.Models;

namespace webRenovoTesouraria.ViewModels
{
    public class DetalhesVM
    {
        public int ID_Head { get; set; }
        public List<DetalheCategoriaEntrada> ItensCategoria { get; set; }
        public List<DetalheTipoEntrada> ItensTipo { get; set; }

        //public DetalheCategoriaEntrada ItensCategoria { get; set; }
        //public DetalheTipoEntrada ItensTipo { get; set; }
    }

    public class ListaVM
    {
        //public int IDItem { get; set; }
        public int ID_Head { get; set; }
        public Pessoa Pessoa { get; set; }
        public decimal Dizimo { get; set; }
        public decimal Oferta { get; set; }
        public decimal Missoes { get; set; }
        public decimal Reforma { get; set; }
    }
}