using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webRenovoTesouraria.AcessoDados.Repositories;
using webRenovoTesouraria.AcessoDados.Repositories.Contracts;
using webRenovoTesouraria.Models;

namespace webRenovoTesouraria.AcessoDados
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RenovoContext _context;

        private ITipoEntradaRepos _tipoEntradaRepository;
        private IHeadEntradaRepos _headerRepository;
        private IPessoaRepos _pessoaRepository;
        private IDetalheCategoriaRepos _detalheCategoriaRepository;
        private IDetalheTipoRepos _detalheTipoRepository;

        public UnitOfWork(RenovoContext context)
        {
            _context = context;
        }

        public ITipoEntradaRepos TiposEntrada => _tipoEntradaRepository ?? (_tipoEntradaRepository = new TipoEntradaRepos(_context));
        public IHeadEntradaRepos CabecalhosEntradas => _headerRepository ?? (_headerRepository = new HeadEntradaRepos(_context));
        public IPessoaRepos Pessoas => _pessoaRepository ?? (_pessoaRepository = new PessoaRepos(_context));
        public IDetalheCategoriaRepos DetalhesCategoria => _detalheCategoriaRepository ?? (_detalheCategoriaRepository = new DetalheCategoriaRepos(_context));
        public IDetalheTipoRepos DetalhesTipo => _detalheTipoRepository ?? (_detalheTipoRepository = new DetalheTipoRepos(_context));        

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
