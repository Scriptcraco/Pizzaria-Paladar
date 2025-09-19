using UnityEngine;
using UnityEngine.AI;
using System.Collections;
[RequireComponent(typeof(NavMeshAgent))]
public class NPCController : MonoBehaviour
{
    public string funcaoNPC; // atribuído no ContratarPlayer via funcaoNPC.ToString()

    private NavMeshAgent agent;
    private TaskSystem taskSystem;

    private Tarefa tarefaAtual;
    private bool executandoTarefa = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        taskSystem = TaskSystem.Instance;
        if (taskSystem == null)
            Debug.LogError("TaskSystem.Instance está null no NPCController Awake!");
    }

    void Start()
    {
        // Em alguns casos a ordem de Awake pode causar null. Tente obter novamente no Start
        if (taskSystem == null)
        {
            taskSystem = TaskSystem.Instance;
            if (taskSystem == null)
                Debug.LogError("TaskSystem.Instance ainda null no Start do NPCController!");
        }
    }

    void Update()
    {
        if (taskSystem == null) return;

        if (!executandoTarefa)
        {
            tarefaAtual = taskSystem.ObterProximaTarefa(funcaoNPC); // usa variável funcaoNPC
            if (tarefaAtual != null)
            {
                StartCoroutine(ExecutarTarefa(tarefaAtual));
            }
        }
    }

    IEnumerator ExecutarTarefa(Tarefa tarefa)
    {
        executandoTarefa = true;

        if (tarefa.origem != null)
        {
            tarefa.estado = EstadoTarefa.IdaParaOrigem;
            agent.SetDestination(tarefa.origem.position);
            // Espera até chegar usando remainingDistance para evitar cálculo manual
            while (true)
            {
                if (!agent.pathPending)
                {
                    if (agent.remainingDistance <= agent.stoppingDistance)
                        break;
                }
                yield return null;
            }

            tarefa.estado = EstadoTarefa.ExecutandoOrigem;

            bool pegou = tarefa.ExecutarAcaoOrigem(tarefa.quantidade);
            while (!pegou)
            {
                yield return new WaitForSeconds(0.5f);
                pegou = tarefa.ExecutarAcaoOrigem(tarefa.quantidade);
            }
        }

        if (tarefa.destino != null)
        {
            tarefa.estado = EstadoTarefa.IdaParaDestino;
            agent.SetDestination(tarefa.destino.position);
            while (true)
            {
                if (!agent.pathPending)
                {
                    if (agent.remainingDistance <= agent.stoppingDistance)
                        break;
                }
                yield return null;
            }

            tarefa.estado = EstadoTarefa.ExecutandoDestino;

            bool entregou = tarefa.ExecutarAcaoDestino(tarefa.quantidade);
            while (!entregou)
            {
                yield return new WaitForSeconds(0.5f);
                entregou = tarefa.ExecutarAcaoDestino(tarefa.quantidade);
            }
        }

        tarefa.estado = EstadoTarefa.Concluida;
        taskSystem.FinalizarTarefa(tarefa);

        executandoTarefa = false;
    }
}
