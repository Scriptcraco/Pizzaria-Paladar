using UnityEngine;

public class TarefaCarregarFreezerCaixa : Tarefa
{
    private Freezer freezer;
    private Caixa caixa;
    private int carregado;

    public TarefaCarregarFreezerCaixa(Freezer freezer, Caixa caixa, int quantidade)
        : base(TipoTarefa.BuscarRefrigeranteFreezer, quantidade, freezer.transform, caixa.transform, exclusiva:true)
    {
        this.freezer = freezer;
        this.caixa = caixa;
    }

    public override bool ExecutarAcaoOrigem(int quantidadeParaPegar)
    {
        int disponivel = freezer.GetQuantidadeCongelada();
        int pegar = Mathf.Min(disponivel, quantidadeParaPegar);
        if (pegar <= 0) return false;

        freezer.ReduzirQuantidade(pegar);
        carregado = pegar;
        Debug.Log($"Freezer: pegou {pegar} bebidas.");
        return true;
    }

    public override bool ExecutarAcaoDestino(int quantidadeParaEntregar)
    {
        int espacoLivre = caixa.capacidadeBebida - caixa.estoqueBebida;
        int entregar = Mathf.Min(carregado, espacoLivre);
        if (entregar <= 0) return false;

        caixa.AdicionarBebida(entregar);
        carregado -= entregar;
        Debug.Log($"Caixa: entregou {entregar} bebidas.");
        return true;
    }
}
