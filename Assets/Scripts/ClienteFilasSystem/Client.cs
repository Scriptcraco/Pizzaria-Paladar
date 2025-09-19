using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class Client : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private Vector3 ultimoDestino;
    private bool destinoValido = false;

    private bool estaComendo = false;
    private bool estaLargandoLixo = false;
    private bool indoParaSaida = false;

    public Transform focoOlharMesa;
    private Transform pontoSaida;
    public Transform cadeiraOcupada;

    public bool pedidoEntregue = false;
    public int quantidadePizza;
    public int quantidadeBebida;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.updateRotation = false;

        quantidadePizza = Random.Range(1, 5);
        quantidadeBebida = Random.Range(1, 5);

        pontoSaida = GameObject.FindGameObjectWithTag("saida")?.transform;
        if (pontoSaida == null)
            Debug.LogError("[Client] Objeto com tag 'saida' não encontrado na cena.");
    }

    public void IrParaMesa()
    {
        FilaMesaManager filaMesa = FindFirstObjectByType<FilaMesaManager>();
        if (filaMesa == null)
        {
            Debug.LogError("[Client] FilaMesaManager não encontrado na cena.");
            return;
        }

        bool conseguiuLugar = filaMesa.TentarReservarCadeiraDisponivel(this);

        if (!conseguiuLugar)
        {
            filaMesa.EnfileirarClienteParaMesa(this);
        }
        else
        {
            StartCoroutine(SequenciaPosMesa());
        }
    }

    public void ComecarSequencia()
    {
        StartCoroutine(SequenciaPosMesa());
    }

    private IEnumerator SequenciaPosMesa()
    {
        yield return EsperarParar();

        animator.SetFloat("Speed_f", 0f);
        yield return new WaitForSeconds(1f);
        animator.SetBool("Comendo_b", true);
       
        estaComendo = true;
                
        yield return new WaitForSeconds(2f);
        estaComendo = false;

        animator.SetBool("Comendo_b", false);
        yield return new WaitForSeconds(1f);
        animator.SetBool("LargandoLixo_b", true);
        estaLargandoLixo = true;
       ;
        
        yield return new WaitForSeconds(2f);
        estaLargandoLixo = false;
         animator.SetBool("LargandoLixo_b", false);
         yield return new WaitForSeconds(1f);
        if (pontoSaida == null)
        {
            Debug.LogError("[Client] Ponto de saída não configurado.");
            yield break;
        }

        // Liberar cadeira
        if (cadeiraOcupada != null)
        {
            FilaMesaManager filaMesa = FindFirstObjectByType<FilaMesaManager>();
            if (filaMesa != null)
            {
                filaMesa.LiberarCadeira(cadeiraOcupada);
                cadeiraOcupada = null;
            }
            else
            {
                Debug.LogError("[Client] FilaMesaManager não encontrado ao tentar liberar cadeira.");
            }
        }

        // Ir embora
        indoParaSaida = true;
        MoverPara(pontoSaida.position);
        yield return EsperarParar();

        var pooled = GetComponent<PooledObject>();
        if (pooled != null)
            pooled.ReturnToPool();
        else
            gameObject.SetActive(false);
    }

    public void MoverPara(Vector3 destino)
    {
        if (!agent.isOnNavMesh) return;

        if (NavMesh.SamplePosition(destino, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            if ((hit.position - ultimoDestino).sqrMagnitude > 0.01f)
            {
                agent.isStopped = false;
                agent.SetDestination(hit.position);
                ultimoDestino = hit.position;
                destinoValido = true;
            }
        }
        else
        {
            destinoValido = false;
        }
    }

    private IEnumerator EsperarParar()
    {
        while (true)
        {
            bool parado = !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance &&
                          (!agent.hasPath || agent.velocity.sqrMagnitude < 0.01f);
            if (parado) break;
            yield return null;
        }
    }

    private void Update()
    {
        if (!destinoValido || !agent.isOnNavMesh)
        {
            animator.SetFloat("Speed_f", 0f);
            return;
        }

        bool parado = !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance &&
                      (!agent.hasPath || agent.velocity.sqrMagnitude < 0.01f);

        animator.SetFloat("Speed_f", parado ? 0f : 0.5f);

        if (parado)
        {
            if (!agent.isStopped)
                agent.isStopped = true;

            if (estaComendo || estaLargandoLixo)
            {
                if (focoOlharMesa != null)
                {
                    Vector3 direcaoOlhar = (focoOlharMesa.position - transform.position).normalized;
                    direcaoOlhar.y = 0;
                    if (direcaoOlhar.sqrMagnitude > 0.01f)
                    {
                        Quaternion rot = Quaternion.LookRotation(direcaoOlhar);
                        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 5f);
                    }
                }
            }

            return;
        }

        if (agent.isStopped)
            agent.isStopped = false;

        Vector3 direcao = agent.velocity.normalized;
        if (direcao.sqrMagnitude > 0.01f)
        {
            Quaternion lookRot = Quaternion.LookRotation(direcao);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 10f);
        }
    }
    public bool EstaParado()
{
    if (agent == null || !agent.isOnNavMesh) return false;

    bool parado = !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance &&
                  (!agent.hasPath || agent.velocity.sqrMagnitude < 0.01f);

    return parado;
}
}
