public class EntradaRequest
{
    public string Nome { get; set; }
    public string TipoEntrada { get; set; }
    public string Observacao { get; set; }

    public decimal VlDizimo { get; set; }
    public decimal VlOferta { get; set; }
    public decimal VlMissoes { get; set; }
    public decimal VlReforma { get; set; }

    //public EntradaTipo Tipo { get; set; }

    
    public decimal VlNotas { get; set; }
    public decimal VlMoedas { get; set; }
    public decimal VlCheques { get; set; }
    public decimal VlDebito { get; set; }
    public decimal VlCredito { get; set; }
    public decimal VlTransf { get; set; }
    

}

public class EntradaTipo
{
    public decimal Valor { get; set; }
    public int IdTipo { get; set; }
}