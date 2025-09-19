using UnityEngine;

public class FreezerTaskGenerator : MonoBehaviour
{
    public Freezer freezer;
    public Caixa caixa;

    void Update()
    {
        int congelado = freezer.GetQuantidadeCongelada();
        int espaçoCaixa = caixa.capacidadeBebida - caixa.estoqueBebida;

        if (congelado > 0 && espaçoCaixa > 0)
        {
            int quantidade = Mathf.Min(congelado, espaçoCaixa);
            var tarefa = new TarefaCarregarFreezerCaixa(freezer, caixa, quantidade);
            TaskSystem.Instance.AdicionarTarefa(tarefa);
        }
    }
}
