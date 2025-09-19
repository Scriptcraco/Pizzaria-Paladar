using UnityEngine;
using UnityEngine.AI;

public class NPC_CarregadorDrive : MonoBehaviour
{
    [Header("Referências automáticas via Tags")]

    public Collider balcaoEmpacotarTriggerPegarEmpacotados;
    public BalcaoEmpacotar balcaoEmpacotar;

    public Collider freezerTrigger;
    public Freezer freezer;

    public Collider caixaDriveTrigger;
    public CaixaDoDrive caixaDrive;

    [Header("Configurações NPC")]
    public float velocidade = 3.5f;
    public int capacidadeProdutoEmpacotado = 10; // Produto empacotado que carrega para entregar no caixa
    public int capacidadeBebida = 10;
    public float distanciaInteracao = 1.2f;

    [Header("Rotação e animação")]
    public float rotacaoSpeed = 8f;

    private NavMeshAgent agent;
    private Animator animator;

    private enum Estado
    {
        IndoParaBalcaoEmpacotarPegarEmpacotados,
        PegandoProdutoEmpacotadoNoBalcao,
        IndoParaCaixaDriveComProdutoEmpacotado,
        EntregandoProdutoEmpacotado,
        IndoParaFreezer,
        PegandoBebida,
        IndoParaCaixaDriveComBebida,
        EntregandoBebida,
        Esperando
    }

    private Estado estadoAtual = Estado.Esperando;

    private int produtoEmpacotadoCarregado = 0;
    private int bebidaCarregada = 0;

    private Vector3 destinoAtual = Vector3.zero;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (agent != null)
        {
            agent.speed = velocidade;
            agent.updateRotation = false;
        }

        AssignCollider("BalcaoEmpacotarTriggerPegarEmpacotados", ref balcaoEmpacotarTriggerPegarEmpacotados);
        AssignComponent("BalcaoEmpacotar", ref balcaoEmpacotar);

        AssignCollider("FreezerTrigger", ref freezerTrigger);
        AssignComponent("Freezer", ref freezer);

        AssignCollider("CaixaDriveTrigger", ref caixaDriveTrigger);
        AssignComponent("CaixaDoDrive", ref caixaDrive);

        estadoAtual = Estado.Esperando;
    }

    private void AssignCollider(string tag, ref Collider col)
    {
        var go = GameObject.FindGameObjectWithTag(tag);
        if (go != null) col = go.GetComponent<Collider>();
        else Debug.LogError($"{tag} não encontrado!");
    }

    private void AssignComponent<T>(string tag, ref T comp) where T : Component
    {
        var go = GameObject.FindGameObjectWithTag(tag);
        if (go != null)
        {
            comp = go.GetComponent<T>();
            if (comp == null)
                Debug.LogError($"{typeof(T).Name} não encontrado no objeto com tag {tag}");
        }
        else Debug.LogError($"{tag} não encontrado!");
    }

    private void Start()
    {
        estadoAtual = Estado.Esperando;
    }

    private void Update()
    {
        AtualizarAnimacaoERotacao();

        if (!agent.pathPending && agent.remainingDistance <= distanciaInteracao)
        {
            agent.isStopped = true;
            animator.SetFloat("Speed_f", 0f);
            OlharParaAlvo(destinoAtual);

            switch (estadoAtual)
            {
                case Estado.Esperando:
                    if (balcaoEmpacotar.GetEstoqueEmpacotado() > 0 && produtoEmpacotadoCarregado < capacidadeProdutoEmpacotado)
                    {
                        IrParaBalcaoEmpacotarPegarEmpacotados();
                    }
                    else if (bebidaCarregada < capacidadeBebida && freezer.GetQuantidadeCongelada() > 0)
                    {
                        IrParaFreezer();
                    }
                    else
                    {
                        Debug.Log("NPC: Sem tarefas disponíveis, aguardando...");
                    }
                    break;

                case Estado.IndoParaBalcaoEmpacotarPegarEmpacotados:
                    estadoAtual = Estado.PegandoProdutoEmpacotadoNoBalcao;
                    Debug.Log("NPC: Chegou para pegar produtos empacotados.");
                    break;

                case Estado.PegandoProdutoEmpacotadoNoBalcao:
                    if (TemEspacoParaProdutoEmpacotado() && balcaoEmpacotar.GetEstoqueEmpacotado() > 0)
                    {
                        int paraPegar = Mathf.Min(balcaoEmpacotar.GetEstoqueEmpacotado(), capacidadeProdutoEmpacotado - produtoEmpacotadoCarregado);
                        balcaoEmpacotar.PodeRetirarEmpacotado(paraPegar);
                        produtoEmpacotadoCarregado += paraPegar;
                        balcaoEmpacotar.RetirarEmpacotado(paraPegar);
                        Debug.Log($"NPC: Pegou {paraPegar} produtos empacotados. Total carregado: {produtoEmpacotadoCarregado}");
                    }
                    if (produtoEmpacotadoCarregado > 0)
                    {
                        IrParaCaixaDriveComProdutoEmpacotado();
                    }
                    else
                    {
                        estadoAtual = Estado.Esperando;
                    }
                    break;

                case Estado.IndoParaCaixaDriveComProdutoEmpacotado:
                    estadoAtual = Estado.EntregandoProdutoEmpacotado;
                    Debug.Log("NPC: Chegou no caixa drive com produto empacotado.");
                    break;

                case Estado.EntregandoProdutoEmpacotado:
                    if (produtoEmpacotadoCarregado > 0 && TemEspacoNoCaixaDriveParaProduto())
                    {
                        int espaço = caixaDrive.GetCapacidadeProdutoEmbalado() - caixaDrive.GetEstoqueProdutoEmbalado();
                        int paraEntregar = Mathf.Min(produtoEmpacotadoCarregado, espaço);
                        caixaDrive.AdicionarProdutoEmbalado(paraEntregar);
                        produtoEmpacotadoCarregado -= paraEntregar;
                        Debug.Log($"NPC: Entregou {paraEntregar} produtos empacotados. Produto restante: {produtoEmpacotadoCarregado}");
                    }
                    if (produtoEmpacotadoCarregado <= 0)
                    {
                        if (bebidaCarregada < capacidadeBebida && freezer.GetQuantidadeCongelada() > 0)
                        {
                            IrParaFreezer();
                        }
                        else
                        {
                            estadoAtual = Estado.Esperando;
                        }
                    }
                    break;

                case Estado.IndoParaFreezer:
                    estadoAtual = Estado.PegandoBebida;
                    Debug.Log("NPC: Chegou no freezer, pegando bebida.");
                    break;

                case Estado.PegandoBebida:
                    if (TemEspacoParaBebida() && freezer.GetQuantidadeCongelada() > 0)
                    {
                        if (freezer.ConsumirItemCongelado())
                        {
                            bebidaCarregada++;
                            Debug.Log($"NPC: Bebida coletada. Total carregado: {bebidaCarregada}");
                        }
                        else
                        {
                            Debug.LogWarning("NPC: Falha ao coletar bebida do freezer.");
                        }
                    }
                    if (bebidaCarregada >= capacidadeBebida || freezer.GetQuantidadeCongelada() == 0)
                    {
                        IrParaCaixaDriveComBebida();
                    }
                    break;

                case Estado.IndoParaCaixaDriveComBebida:
                    estadoAtual = Estado.EntregandoBebida;
                    Debug.Log("NPC: Chegou no caixa drive com bebida.");
                    break;

                case Estado.EntregandoBebida:
                    if (bebidaCarregada > 0 && TemEspacoNoCaixaDriveParaBebida())
                    {
                        int espaço = caixaDrive.GetCapacidadeBebidaEmbalada() - caixaDrive.GetEstoqueBebidaEmbalada();
                        int paraEntregar = Mathf.Min(bebidaCarregada, espaço);
                        caixaDrive.AdicionarBebidaEmbalada(paraEntregar);
                        bebidaCarregada -= paraEntregar;
                        Debug.Log($"NPC: Entregou {paraEntregar} bebidas. Bebida restante: {bebidaCarregada}");
                    }
                    if (bebidaCarregada <= 0)
                    {
                        estadoAtual = Estado.Esperando;
                    }
                    break;
            }
        }
        else
        {
            if (agent != null) agent.isStopped = false;
        }
    }

    private void AtualizarAnimacaoERotacao()
    {
        float velocidadeAtual = agent != null ? agent.velocity.magnitude : 0f;
        if (animator != null)
            animator.SetFloat("Speed_f", velocidadeAtual > 0.1f ? 0.5f : 0f);

        if (agent != null && agent.velocity.sqrMagnitude > 0.001f)
        {
            Vector3 dirMov = agent.velocity.normalized;
            dirMov.y = 0f;
            if (dirMov != Vector3.zero)
            {
                Quaternion alvoRot = Quaternion.LookRotation(dirMov);
                transform.rotation = Quaternion.Slerp(transform.rotation, alvoRot, Time.deltaTime * rotacaoSpeed);
            }
        }
    }

    private void OlharParaAlvo(Vector3 posicaoAlvo)
    {
        if (posicaoAlvo == Vector3.zero) return;
        Vector3 direcao = posicaoAlvo - transform.position;
        direcao.y = 0f;
        if (direcao.sqrMagnitude <= 0.0001f) return;
        Quaternion rotacaoFinal = Quaternion.LookRotation(direcao.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotacaoFinal, Time.deltaTime * rotacaoSpeed);
    }

    // Métodos para mandar NPC para pontos de interesse
    private void IrParaBalcaoEmpacotarPegarEmpacotados()
    {
        estadoAtual = Estado.IndoParaBalcaoEmpacotarPegarEmpacotados;
        agent.isStopped = false;
        destinoAtual = balcaoEmpacotarTriggerPegarEmpacotados.bounds.center;
        agent.SetDestination(destinoAtual);
        Debug.Log("NPC: Indo para balcão empacotar (pegar empacotados).");
    }

    private void IrParaCaixaDriveComProdutoEmpacotado()
    {
        estadoAtual = Estado.IndoParaCaixaDriveComProdutoEmpacotado;
        agent.isStopped = false;
        destinoAtual = caixaDriveTrigger.bounds.center;
        agent.SetDestination(destinoAtual);
        Debug.Log("NPC: Indo para caixa drive com produto empacotado.");
    }

    private void IrParaFreezer()
    {
        estadoAtual = Estado.IndoParaFreezer;
        agent.isStopped = false;
        destinoAtual = freezerTrigger.bounds.center;
        agent.SetDestination(destinoAtual);
        Debug.Log("NPC: Indo para freezer.");
    }

    private void IrParaCaixaDriveComBebida()
    {
        estadoAtual = Estado.IndoParaCaixaDriveComBebida;
        agent.isStopped = false;
        destinoAtual = caixaDriveTrigger.bounds.center;
        agent.SetDestination(destinoAtual);
        Debug.Log("NPC: Indo para caixa drive com bebida.");
    }

    // Condições e verificações de espaço
    private bool TemEspacoParaProdutoEmpacotado()
    {
        return produtoEmpacotadoCarregado < capacidadeProdutoEmpacotado;
    }

    private bool TemEspacoParaBebida()
    {
        return bebidaCarregada < capacidadeBebida;
    }

    private bool TemEspacoNoCaixaDriveParaProduto()
    {
        if (caixaDrive == null) return false;
        return caixaDrive.GetEstoqueProdutoEmbalado() < caixaDrive.GetCapacidadeProdutoEmbalado();
    }

    private bool TemEspacoNoCaixaDriveParaBebida()
    {
        if (caixaDrive == null) return false;
        return caixaDrive.GetEstoqueBebidaEmbalada() < caixaDrive.GetCapacidadeBebidaEmbalada();
    }
}
