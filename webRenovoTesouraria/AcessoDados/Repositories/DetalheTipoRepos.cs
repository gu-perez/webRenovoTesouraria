using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webRenovoTesouraria.AcessoDados.Repositories.Contracts;
using webRenovoTesouraria.Models;

namespace webRenovoTesouraria.AcessoDados.Repositories
{
    public class DetalheTipoRepos : IDetalheTipoRepos
    {
        private readonly RenovoContext _context;
        public DetalheTipoRepos(RenovoContext context) { _context = context; }

        public bool Inserir(int idHead, int idPessoa, int idTipo, decimal vlTipo, string obs = "")
        {
            _context.DetalheTipoEntrada.Add(new DetalheTipoEntrada
            {
                ID_HeadTipo = idHead,
                IdPessoa = idPessoa,
                IdTipo = idTipo,
                Valor = vlTipo,
                Obs = obs
            });

            if (_context.SaveChanges() > 0)
                return true;
            else
                return false;
        }
    }
}
