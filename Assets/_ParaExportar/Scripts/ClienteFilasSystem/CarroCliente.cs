using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class CarroCliente : MonoBehaviour
{
    private NavMeshAgent agent;
    private Vector3 destinoAtual;
    public bool pedidoAtendido = false;

    public int quantidadePizza = 0;
    public int quantidadeBebida = 0;

    private Transform pontoSaida;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = true;

        quantidadePizza = Random.Range(1, 3);
        quantidadeBebida = Random.Range(1, 3);

        pontoSaida = GameObject.FindGameObjectWithTag("saida")?.transform;
        if (pontoSaida == null)
            Debug.LogError("[CarroCliente] Objeto com tag 'saida' não encontrado na cena.");
    }

    public void MoverPara(Vector3 destino)
    {
        if (!agent.isOnNavMesh) return;

        if (NavMesh.SamplePosition(destino, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            destinoAtual = hit.position;
        }
    }

    public bool EstaParado()
    {
        return !agent.pathPending &&
               agent.remainingDistance <= agent.stoppingDistance &&
               (!agent.hasPath || agent.velocity.sqrMagnitude < 0.01f);
    }

    public void AtenderPedido()
    {
        pedidoAtendido = true;
    }

    public bool PedidoAtendido()
    {
        return pedidoAtendido;
    }

    public void IrEmbora()
    {
        if (pontoSaida != null)
        {
            MoverPara(pontoSaida.position);
            StartCoroutine(VerificarSaida());
        }
        else
        {
            Debug.LogWarning("[CarroCliente] pontoSaida não atribuído.");
            Destroy(gameObject, 5f);
        }
    }

    private IEnumerator VerificarSaida()
    {
        yield return new WaitUntil(() => EstaParado());
        Destroy(gameObject);
    }
}
