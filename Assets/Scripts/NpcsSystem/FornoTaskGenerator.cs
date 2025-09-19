using UnityEngine;

public class FornoTaskGenerator : MonoBehaviour
{
    public ProductionUnit forno;
    public Caixa caixa;

    void Update()
    {
        int estoqueForno = forno.GetEstoqueAtual();
        int espaçoCaixa = caixa.capacidadeItem - caixa.estoqueItem;

        if (estoqueForno > 0 && espaçoCaixa > 0)
        {
            int quantidade = Mathf.Min(estoqueForno, espaçoCaixa);
            var tarefa = new TarefaCarregarCaixa(forno, caixa, quantidade);
            TaskSystem.Instance.AdicionarTarefa(tarefa);
        }
    }
}
