namespace webRenovoTesouraria.AcessoDados.Repositories.Contracts
{
    public interface IDetalheTipoRepos
    {
        bool Inserir(int idHead, int idPessoa, int idTipo, decimal vlTipo, string obs = "");
    }
}
