using UnityEngine;

public class TarefaAtenderCaixa : Tarefa
{
    private Caixa _caixa;

    public TarefaAtenderCaixa(Caixa caixa)
        : base(TipoTarefa.AtenderCaixa, 1, caixa.transform, caixa.transform, exclusiva:true)
    {
        _caixa = caixa;
    }

    // Ajustar os métodos para sobrepor os métodos com parâmetro quantidade
    public override bool ExecutarAcaoOrigem(int quantidadeParaPegar)
    {
        // Não há ação na origem para essa tarefa
        return true;
    }

    public override bool ExecutarAcaoDestino(int quantidadeParaEntregar)
    {
        Debug.Log("NPC atendente do caixa está atendendo.");
        // Aqui coloque a lógica de atendimento real
        return true;
    }
}
