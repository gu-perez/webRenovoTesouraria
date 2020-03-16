using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webRenovoTesouraria.AcessoDados.Repositories.Contracts;
using webRenovoTesouraria.Models;

namespace webRenovoTesouraria.AcessoDados.Repositories
{
    public class DetalheCategoriaRepos : IDetalheCategoriaRepos
    {
        private readonly RenovoContext _context;
        public DetalheCategoriaRepos(RenovoContext context) { _context = context; }

        public bool Inserir(int idHead, int idPessoa, int idCategoria, decimal vlCategoria, string obs = "")
        {
            _context.DetalheCategoriaEntrada.Add(new DetalheCategoriaEntrada
            {
                ID_HeadCategoria = idHead,
                IdPessoa = idPessoa,
                IdCategoria = idCategoria,
                Valor = vlCategoria,
                Obs = obs
            });

            if (_context.SaveChanges() > 0)
                return true;
            else
                return false;

        }
    }
}
