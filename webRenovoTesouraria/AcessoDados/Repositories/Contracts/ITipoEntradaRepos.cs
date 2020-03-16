using System.Collections.Generic;

namespace webRenovoTesouraria.AcessoDados.Repositories.Contracts
{
    public interface ITipoEntradaRepos
    {
        List<Models.TipoEntrada> Listar();
    }
}
