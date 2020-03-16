namespace webRenovoTesouraria.AcessoDados.Repositories.Contracts
{
    public interface IDetalheCategoriaRepos
    {
        bool Inserir(int idHead, int idPessoa, int idCategoria, decimal vlCategoria, string obs = "");
    }
}
