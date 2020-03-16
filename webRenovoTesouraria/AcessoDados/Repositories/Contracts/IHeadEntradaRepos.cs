using System.Collections.Generic;
using webRenovoTesouraria.ViewModels;

namespace webRenovoTesouraria.AcessoDados.Repositories.Contracts
{
    public interface IHeadEntradaRepos
    {
        int Incluir(CabecalhoEntradaVM item);
        List<DetalhesVM> ListaDetalhes(int idHead);
        List<ListaVM> ListarResponse(int idHead);

        bool ExcluirEntrada(int idHead, int idPessoa);
    }
}
