using UnityEngine;
using System.Collections.Generic;

public class CaixaTaskGenerator : MonoBehaviour
{
    public Caixa caixa;

    public List<Tarefa> GerarTarefas()
    {
        List<Tarefa> tarefas = new List<Tarefa>();

        // Condição exemplo para gerar tarefa de atendimento
        // Pode ser expandido conforme demanda real da fila

        tarefas.Add(new TarefaAtenderCaixa(caixa));

        return tarefas;
    }
}
