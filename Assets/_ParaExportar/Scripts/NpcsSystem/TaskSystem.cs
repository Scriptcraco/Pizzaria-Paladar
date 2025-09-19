using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class TaskSystem : MonoBehaviour
{
    public static TaskSystem Instance;

    private List<Tarefa> tarefas = new List<Tarefa>();
    private HashSet<Tarefa> tarefasExclusivasOcupadas = new HashSet<Tarefa>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AdicionarTarefa(Tarefa t)
    {
        if (t == null) return;

        if (t.exclusiva)
        {
            foreach (var tarefaExistente in tarefas)
            {
                if (tarefaExistente.tipo == t.tipo &&
                    tarefaExistente.origem == t.origem &&
                    tarefaExistente.destino == t.destino)
                    return;
            }
        }

        tarefas.Add(t);
        Debug.Log($"TaskSystem: tarefa adicionada {t.tipo} x{t.quantidade}");
    }

    public Tarefa ObterProximaTarefa(string funcaoNPC)
{
    for (int i = 0; i < tarefas.Count; i++)
    {
        Tarefa t = tarefas[i];

        if (t.exclusiva && tarefasExclusivasOcupadas.Contains(t))
            continue;

        if (FuncaoAceitaTarefa(funcaoNPC, t.tipo))
        {
            tarefas.RemoveAt(i);
            if (t.exclusiva)
                tarefasExclusivasOcupadas.Add(t);

            return t;
        }
    }
    return null;
}

private bool FuncaoAceitaTarefa(string funcaoNPC, TipoTarefa tipo)
{
    switch (funcaoNPC)
    {
        case "CarregadorCaixa":
            return tipo == TipoTarefa.BuscarComidaForno ||
                   tipo == TipoTarefa.BuscarRefrigeranteFreezer ||
                   tipo == TipoTarefa.ReporEstoqueCaixa;
        case "CarregadorDrive":
            return tipo == TipoTarefa.BuscarComidaForno ||
                   tipo == TipoTarefa.BuscarRefrigeranteFreezer ||
                   tipo == TipoTarefa.ReporEstoqueCaixa;
        case "AtendenteCaixa":
            return tipo == TipoTarefa.AtenderCaixa;
        case "AtendenteDrive":
            return tipo == TipoTarefa.AtenderCaixaDrive;
        case "Empacotador":
            return tipo == TipoTarefa.EmpacotarELevarDrive;
        default:
            return false;
    }
}
    public void FinalizarTarefa(Tarefa t)
    {
        if (t == null) return;

        if (t.exclusiva && tarefasExclusivasOcupadas.Contains(t))
            tarefasExclusivasOcupadas.Remove(t);
    }

   
}
