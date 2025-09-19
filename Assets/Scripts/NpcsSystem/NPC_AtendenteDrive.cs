using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class NPC_AtendenteDrive : MonoBehaviour
{
    [Header("Referências (Inspector ou busca automática)")]
    public Collider caixaVendaTrigger;     
    public CarSpawner carSpawner; // colocar junto às outras referências    
    public CaixaDoDrive caixaDrive;             
    public FilaDriveThruManager filaDrive;     
    public PlayerController player;

    [Header("Configurações de Movimento")]
    public float velocidade = 2.5f;             
    public float distanciaInteracao = 1.2f;    
    public float intervaloVenda = 0.5f;        

    [Header("Valores de Venda")]
    public int valorBebidaNoDrive = 10;         
    public int valorPizzaNoDrive = 60;          

    private NavMeshAgent agent;

    private enum Estado { IndoParaCaixa, AtendendoCliente, Esperando }
    private Estado estadoAtual = Estado.Esperando;

    private Coroutine atendimentoCoroutine;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = velocidade;
        agent.updateRotation = false;

        BuscarReferenciaSeNulo(ref carSpawner, "CarSpawner");
        BuscarReferenciaSeNulo(ref caixaVendaTrigger, "CaixaAtenderDoDrive");
        BuscarReferenciaSeNulo(ref caixaDrive, "CaixaDoDrive");
        BuscarReferenciaSeNulo(ref filaDrive, "FilaDriveThruManager");
        BuscarReferenciaSeNulo(ref player, "Player");
    }

    private void BuscarReferenciaSeNulo<T>(ref T componente, string tag) where T : Component
    {
        if (componente == null)
        {
            GameObject go = GameObject.FindGameObjectWithTag(tag);
            if (go != null)
                componente = go.GetComponent<T>();
        }
    }

    private void Start()
    {
        IrParaCaixa();
    }

    private void Update()
    {
        if (ChegouNoDestino())
        {
            if (estadoAtual == Estado.IndoParaCaixa)
            {
                estadoAtual = Estado.AtendendoCliente;
                if (atendimentoCoroutine == null)
                    atendimentoCoroutine = StartCoroutine(ProcessarVendasDrive());
            }
        }
        else
        {
            if (estadoAtual == Estado.AtendendoCliente && agent.velocity.sqrMagnitude > 0.01f)
            {
                PararAtendimento();
                estadoAtual = Estado.IndoParaCaixa;
            }
        }

        if (estadoAtual == Estado.Esperando && filaDrive != null && filaDrive.TemCarroNaPosicao(0))
        {
            estadoAtual = Estado.IndoParaCaixa;
            IrParaCaixa();
        }
    }

    private bool ChegouNoDestino()
    {
        return !agent.pathPending && agent.remainingDistance <= distanciaInteracao;
    }

    private void IrParaCaixa()
    {
        if (caixaVendaTrigger == null)
            return;

        estadoAtual = Estado.IndoParaCaixa;
        agent.isStopped = false;
        agent.SetDestination(caixaVendaTrigger.bounds.center);
    }

    private void PararAtendimento()
    {
        if (atendimentoCoroutine != null)
        {
            StopCoroutine(atendimentoCoroutine);
            atendimentoCoroutine = null;
        }
    }

    private IEnumerator ProcessarVendasDrive()
    {
        while (true)
        {
            GameObject carroGO = filaDrive.ObterCarroPosicao1();

            if (carroGO == null)
            {
                estadoAtual = Estado.Esperando;
                atendimentoCoroutine = null;
                yield break;
            }

            CarroCliente cliente = carroGO.GetComponent<CarroCliente>();

            if (cliente == null || cliente.PedidoAtendido() || !cliente.EstaParado())
            {
                yield return new WaitForSeconds(intervaloVenda);
                continue;
            }

            int qtdPizza = cliente.quantidadePizza;
            int qtdBebida = cliente.quantidadeBebida;

            bool podeAtender = caixaDrive.GetEstoqueProdutoEmbalado() >= qtdPizza &&
                               caixaDrive.GetEstoqueBebidaEmbalada() >= qtdBebida;

            if (podeAtender)
            {
                caixaDrive.RemoverProdutoEmbalado(qtdPizza);
                caixaDrive.RemoverBebidaEmbalada(qtdBebida);

                int valor = (qtdPizza * valorPizzaNoDrive) + (qtdBebida * valorBebidaNoDrive);
                player.AdicionarDinheiro(valor);

                cliente.AtenderPedido();
                cliente.IrEmbora();

                filaDrive.SairDaFila(carroGO);
                if (carSpawner != null)
{
    carSpawner.SairDaFila(carroGO);
}
else
{
    DevLog.Warn("[NPC_AtendenteDrive] carSpawner não atribuído! Carro não removido do spawner.");
}
            }
            else
            {
                DevLog.Info("[NPC_AtendenteDrive] Estoque insuficiente para atender o cliente.");
            }

            yield return new WaitForSeconds(intervaloVenda);
        }
    }
}
