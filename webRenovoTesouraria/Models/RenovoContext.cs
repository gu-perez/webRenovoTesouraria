using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webRenovoTesouraria.Models
{
    public class RenovoContext : DbContext
    {
        public RenovoContext(DbContextOptions<RenovoContext> options) :
            base(options)
        {
        }
        public DbSet<CabecalhoEntrada> CabecalhoEntrada { get; set; }
        public DbSet<CategoriaEntrada> CategoriaEntrada { get; set; }
        public DbSet<DetalheCategoriaEntrada> DetalheCategoriaEntrada { get; set; }
        public DbSet<DetalheTipoEntrada> DetalheTipoEntrada { get; set; }
        public DbSet<Pessoa> Pessoa { get; set; }
        public DbSet<TipoEntrada> TipoEntrada { get; set; }
    }
}
