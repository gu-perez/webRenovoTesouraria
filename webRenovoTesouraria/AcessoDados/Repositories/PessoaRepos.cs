using System.Collections.Generic;
using webRenovoTesouraria.AcessoDados.Repositories.Contracts;
using webRenovoTesouraria.Models;
using webRenovoTesouraria.ViewModels;
using System.Linq;

namespace webRenovoTesouraria.AcessoDados.Repositories
{
    public class PessoaRepos : IPessoaRepos
    {
        private readonly RenovoContext _context;
        public PessoaRepos(RenovoContext context)
        {
            _context = context;
        }

        public int Incluir(string nome)
        {
            var linha = new Pessoa{ Nome = nome };
            _context.Pessoa.Add(linha);

            if (_context.SaveChanges() > 0)
                return linha.ID;
            else
                return 0;
        }

        public List<PessoaVM> Selecionar(int id = 0, string nome = "", string nomeCripto = "")
        {
            var retorno = from p in _context.Pessoa select p;
            if (id > 0) retorno = retorno.Where(w => w.ID == id);
            if (nome != "") retorno = retorno.Where(w => w.Nome.Contains(nome));
            if (nomeCripto !="") retorno = retorno.Where(w => w.Nome == nomeCripto);

            return (from r in retorno select new PessoaVM { ID = r.ID, Nome = r.Nome }).ToList();
        }
    }
}
