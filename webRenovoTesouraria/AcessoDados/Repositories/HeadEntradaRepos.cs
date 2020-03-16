using System.Collections.Generic;
using webRenovoTesouraria.AcessoDados.Repositories.Contracts;
using webRenovoTesouraria.Models;
using webRenovoTesouraria.ViewModels;
using System.Linq;

namespace webRenovoTesouraria.AcessoDados.Repositories
{
    public class HeadEntradaRepos : IHeadEntradaRepos
    {
        private readonly RenovoContext _context;

        public HeadEntradaRepos(RenovoContext context) { _context = context; }

        public int Incluir(CabecalhoEntradaVM item)
        {
            var linha = new CabecalhoEntrada
            {
                Nome1 = item.Nome1,
                Nome2 = item.Nome2,
                DataInclusao = System.DateTime.Now
            };

            _context.CabecalhoEntrada.Add(linha);

            if (_context.SaveChanges() > 0)
                return linha.ID;
            else
                return 0;
        }

        public List<DetalhesVM> ListaDetalhes(int idHead)
        {
            var lista = from c in _context.CabecalhoEntrada
                        where c.ID == idHead
                        select new DetalhesVM
                        {
                            ID_Head = idHead,
                            ItensCategoria = (from cat in _context.DetalheCategoriaEntrada where c.ID == cat.ID_HeadCategoria select cat).ToList(),
                            ItensTipo = (from tipo in _context.DetalheTipoEntrada where c.ID == tipo.ID_HeadTipo select tipo).ToList()
                        };

            return lista.ToList();

        }

        public List<ListaVM> ListarResponse(int idHead)
        {

            //RECUPERA TODAS AS ENTRADAS, EXCETO O ANONIMO
            var listaPessoas = (from c in _context.DetalheCategoriaEntrada
                                where c.ID_HeadCategoria == idHead
                                select c.IdPessoa).Distinct();

            List<ListaVM> listaRetorno = new List<ListaVM>();

            foreach (var pessoa in listaPessoas.Where(w => w != 1))
            {
                ListaVM item = new ListaVM()
                {
                    Pessoa = (from p in _context.Pessoa where p.ID == pessoa select new Pessoa { ID = p.ID, Nome = Util.DataCript.Decriptar(p.Nome).ToUpper() }).FirstOrDefault(),
                    Dizimo = (from c in _context.DetalheCategoriaEntrada where c.ID_HeadCategoria == idHead && c.IdPessoa == pessoa && c.IdCategoria == 1 select c.Valor).Sum(),
                    Oferta = (from c in _context.DetalheCategoriaEntrada where c.ID_HeadCategoria == idHead && c.IdPessoa == pessoa && c.IdCategoria == 2 select c.Valor).Sum(),
                    Missoes = (from c in _context.DetalheCategoriaEntrada where c.ID_HeadCategoria == idHead && c.IdPessoa == pessoa && c.IdCategoria == 3 select c.Valor).Sum(),
                    Reforma = (from c in _context.DetalheCategoriaEntrada where c.ID_HeadCategoria == idHead && c.IdPessoa == pessoa && c.IdCategoria == 4 select c.Valor).Sum()
                };

                listaRetorno.Add(item);
                item = null;
            }

            //ADICIONA O ANÔNIMO
            if (listaPessoas.Where(w => w == 1).Count() > 0)
            {
                listaRetorno.Add(new ListaVM()
                {
                    Pessoa = (from p in _context.Pessoa where p.ID == 1 select new Pessoa { ID = p.ID, Nome = p.Nome.ToUpper() }).FirstOrDefault(),
                    Dizimo = (from c in _context.DetalheCategoriaEntrada where c.ID_HeadCategoria == idHead && c.IdPessoa == 1 && c.IdCategoria == 1 select c.Valor).Sum(),
                    Oferta = (from c in _context.DetalheCategoriaEntrada where c.ID_HeadCategoria == idHead && c.IdPessoa == 1 && c.IdCategoria == 2 select c.Valor).Sum(),
                    Missoes = (from c in _context.DetalheCategoriaEntrada where c.ID_HeadCategoria == idHead && c.IdPessoa == 1 && c.IdCategoria == 3 select c.Valor).Sum(),
                    Reforma = (from c in _context.DetalheCategoriaEntrada where c.ID_HeadCategoria == idHead && c.IdPessoa == 1 && c.IdCategoria == 4 select c.Valor).Sum()
                });
            }

            return listaRetorno;


        }

        public bool ExcluirEntrada(int idHead, int idPessoa)
        {
            var apagar_categoria = (from c in _context.DetalheCategoriaEntrada where c.ID_HeadCategoria == idHead && c.IdPessoa == idPessoa select c);
            foreach(var linha in apagar_categoria)
            {
                _context.DetalheCategoriaEntrada.Attach(linha);
                _context.DetalheCategoriaEntrada.Remove(linha);
            }

            var apagar_tipo = (from c in _context.DetalheTipoEntrada where c.ID_HeadTipo == idHead && c.IdPessoa == idPessoa select c);
            foreach (var linha in apagar_tipo)
            {
                _context.DetalheTipoEntrada.Attach(linha);
                _context.DetalheTipoEntrada.Remove(linha);
            }                

            if (_context.SaveChanges() > 0)
                return true;
            else
                return false;

        }

    }
}
