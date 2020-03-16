using System.Collections.Generic;
using System.Linq;
using webRenovoTesouraria.AcessoDados.Repositories.Contracts;
using webRenovoTesouraria.Models;

namespace webRenovoTesouraria.AcessoDados.Repositories
{
    public class TipoEntradaRepos : ITipoEntradaRepos
    {
        private readonly RenovoContext _context;

        public TipoEntradaRepos(RenovoContext context)
        {
            _context = context;
        }

        public List<TipoEntrada> Listar()
        {
            return _context.TipoEntrada.ToList();
        }
    }
}
