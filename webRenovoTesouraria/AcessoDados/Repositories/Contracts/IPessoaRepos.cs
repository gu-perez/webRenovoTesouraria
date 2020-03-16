using System.Collections.Generic;
using webRenovoTesouraria.ViewModels;

namespace webRenovoTesouraria.AcessoDados.Repositories.Contracts
{
    public interface IPessoaRepos
    {
        int Incluir(string nome);
        List<PessoaVM> Selecionar(int id = 0, string nome = "", string nomeCripto = "");
    }
}
