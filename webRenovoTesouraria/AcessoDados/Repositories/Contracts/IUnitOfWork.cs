namespace webRenovoTesouraria.AcessoDados.Repositories.Contracts
{
    public interface IUnitOfWork
    {
        ITipoEntradaRepos TiposEntrada { get; }
        IHeadEntradaRepos CabecalhosEntradas { get; }
        IPessoaRepos Pessoas { get; }
        IDetalheCategoriaRepos DetalhesCategoria { get; }
        IDetalheTipoRepos DetalhesTipo { get; }

        void SaveChanges();
    }
}
