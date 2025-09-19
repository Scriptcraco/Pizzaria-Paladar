using UnityEngine;

public class TarefaCarregarCaixa : Tarefa
{
    private ProductionUnit forno;
    private Caixa caixa;

    public TarefaCarregarCaixa(ProductionUnit forno, Caixa caixa, int quantidade)
        : base(TipoTarefa.BuscarComidaForno, quantidade, forno.transform, caixa.transform, exclusiva:true)
    {
        this.forno = forno;
        this.caixa = caixa;
    }

    public override bool ExecutarAcaoOrigem(int quantidadeParaPegar)
    {
        int disponivel = forno.GetEstoqueAtual();
        int pegar = Mathf.Min(disponivel, quantidadeParaPegar);
        if (pegar <= 0) return false;

        forno.ReduzirEstoque(pegar);
        Debug.Log($"Forno: pegou {pegar} unidades.");
        return true;
    }

    public override bool ExecutarAcaoDestino(int quantidadeParaEntregar)
    {
        int espaçoLivre = caixa.capacidadeItem - caixa.estoqueItem;
        int entregar = Mathf.Min(quantidadeParaEntregar, espaçoLivre);
        if (entregar <= 0) return false;

        caixa.AdicionarProduto(entregar);
        Debug.Log($"Caixa: entregou {entregar} produtos.");
        return true;
    }
}
