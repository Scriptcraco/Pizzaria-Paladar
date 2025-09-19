using UnityEngine;

public class TarefaAtenderCaixaDrive : Tarefa
{
    private CaixaDoDrive _caixaDoDrive;

    public TarefaAtenderCaixaDrive(CaixaDoDrive caixaDoDrive)
        : base(TipoTarefa.AtenderCaixaDrive, 1, caixaDoDrive.transform, caixaDoDrive.transform, true)
    {
       _caixaDoDrive = caixaDoDrive;
    }

   
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