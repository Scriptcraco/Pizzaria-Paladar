using UnityEngine;
using UnityEngine.AI;

public class NPC_CarregadorCaixa : MonoBehaviour
{
    // --- Referências para atribuir automaticamente no Awake ---
    [Header("Referências automáticas via Tags")]
    public Collider fornoTrigger;
    public ProductionUnit forno;

    public Collider freezerTrigger;
    public Freezer freezer;

    public Collider caixaTrigger;
    public Caixa caixa;

    [Header("Configurações NPC")]
    public float velocidade = 3.5f;
    public int capacidadeProduto = 10;
    public int capacidadeBebida = 10;
    public float distanciaInteracao = 1.2f;

    // --- Rotação e animação ---
    [Header("Rotação e animação")]
    [Tooltip("Velocidade de rotação ao girar para a direção do movimento / alvo")]
    public float rotacaoSpeed = 8f;

    // --- Estado interno ---
    private NavMeshAgent agent;
    private Animator animator;

    private enum Estado
    {
        IndoParaForno,
        PegandoProduto,
        IndoParaCaixaComProduto,
        EntregandoProduto,
        IndoParaFreezer,
        PegandoBebida,
        IndoParaCaixaComBebida,
        EntregandoBebida,
        Esperando
    }

    private Estado estadoAtual = Estado.Esperando;

    private int produtoCarregado = 0;
    private int bebidaCarregada = 0;

    // Alterna entre forno e freezer quando está esperando
    private bool alternaParaForno = true;

    // Guarda o destino atual para olhar quando chegar
    private Vector3 destinoAtual = Vector3.zero;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // garante que controle de rotação será manual
        if (agent != null)
        {
            agent.speed = velocidade;
            agent.updateRotation = false; // controlamos rotação manualmente
        }

        // Busca automática por tags e atribui referências
        GameObject fornoTriggerGO = GameObject.FindGameObjectWithTag("FornoTrigger");
        if (fornoTriggerGO != null)
            fornoTrigger = fornoTriggerGO.GetComponent<Collider>();
        else
            Debug.LogError("FornoTrigger não encontrado com tag 'FornoTrigger'");

        GameObject fornoGO = GameObject.FindGameObjectWithTag("Forno");
        if (fornoGO != null)
            forno = fornoGO.GetComponent<ProductionUnit>();
        else
            Debug.LogError("Forno não encontrado com tag 'Forno'");

        GameObject freezerTriggerGO = GameObject.FindGameObjectWithTag("FreezerTrigger");
        if (freezerTriggerGO != null)
            freezerTrigger = freezerTriggerGO.GetComponent<Collider>();
        else
            Debug.LogError("FreezerTrigger não encontrado com tag 'FreezerTrigger'");

        GameObject freezerGO = GameObject.FindGameObjectWithTag("Freezer");
        if (freezerGO != null)
            freezer = freezerGO.GetComponent<Freezer>();
        else
            Debug.LogError("Freezer não encontrado com tag 'Freezer'");

        GameObject caixaTriggerGO = GameObject.FindGameObjectWithTag("CaixaTrigger");
        if (caixaTriggerGO != null)
            caixaTrigger = caixaTriggerGO.GetComponent<Collider>();
        else
            Debug.LogError("CaixaTrigger não encontrado com tag 'CaixaTrigger'");

        GameObject caixaGO = GameObject.FindGameObjectWithTag("Caixa");
        if (caixaGO != null)
            caixa = caixaGO.GetComponent<Caixa>();
        else
            Debug.LogError("Caixa não encontrado com tag 'Caixa'");

        estadoAtual = Estado.Esperando;
    }

    private void Start()
    {
        TentarIrParaForno();
    }

    private void Update()
    {
        // Atualiza animação de acordo com velocidade do agente (mantendo seu valor 0.5 quando em movimento)
        float velocidadeAtual = agent != null ? agent.velocity.magnitude : 0f;
        if (animator != null)
            animator.SetFloat("Speed_f", velocidadeAtual > 0.1f ? 0.5f : 0f);

        // Rotaciona enquanto caminha (baseado na velocidade do agent)
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

        // Verifica se chegou no destino (chegada definida por remainingDistance <= distanciaInteracao)
        if (!agent.pathPending && agent.remainingDistance <= distanciaInteracao)
        {
            // Para o agente para não tentar se ajustar infinitamente
            agent.isStopped = true;

            // Garante que a animação de andar pare
            if (animator != null)
                animator.SetFloat("Speed_f", 0f);

            // Rotação final: encara o destino atual (suavemente)
            OlharParaAlvo(destinoAtual);

            switch (estadoAtual)
            {
                case Estado.IndoParaForno:
                    Debug.Log("Chegou ao forno, iniciando coleta de produto.");
                    estadoAtual = Estado.PegandoProduto;
                    break;

                case Estado.PegandoProduto:
                    if (TemEspacoParaProduto() && forno.GetEstoqueAtual() > 0)
                    {
                        PegarProduto();
                    }
                    else
                    {
                        if (produtoCarregado > 0)
                        {
                            Debug.Log("Produto coletado, indo para caixa entregar.");
                            IrParaCaixaComProduto();
                        }
                        else
                        {
                            Debug.Log("Sem produto para pegar, indo para freezer.");
                            IrParaFreezer();
                        }
                    }
                    break;

                case Estado.IndoParaCaixaComProduto:
                    Debug.Log("Chegou ao caixa com produto, entregando.");
                    estadoAtual = Estado.EntregandoProduto;
                    break;

                case Estado.EntregandoProduto:
                    if (produtoCarregado > 0 && TemEspacoNoCaixaParaProduto())
                    {
                        EntregarProduto();
                    }
                    else
                    {
                        if (freezer.GetQuantidadeCongelada() > 0 && TemEspacoParaBebida())
                        {
                            Debug.Log("Produto entregue, indo para freezer pegar bebida.");
                            IrParaFreezer();
                        }
                        else
                        {
                            Debug.Log("Tudo entregue, esperando nova tarefa.");
                            estadoAtual = Estado.Esperando;
                        }
                    }
                    break;

                case Estado.IndoParaFreezer:
                    Debug.Log("Chegou ao freezer, iniciando coleta de bebida.");
                    estadoAtual = Estado.PegandoBebida;
                    break;

                case Estado.PegandoBebida:
                    if (TemEspacoParaBebida() && freezer.GetQuantidadeCongelada() > 0)
                    {
                        PegarBebida();
                    }
                    else
                    {
                        if (bebidaCarregada > 0)
                        {
                            Debug.Log("Bebida coletada, indo para caixa entregar.");
                            IrParaCaixaComBebida();
                        }
                        else
                        {
                            Debug.Log("Sem bebida para pegar, esperando nova tarefa.");
                            estadoAtual = Estado.Esperando;
                        }
                    }
                    break;

                case Estado.IndoParaCaixaComBebida:
                    Debug.Log("Chegou ao caixa com bebida, entregando.");
                    estadoAtual = Estado.EntregandoBebida;
                    break;

                case Estado.EntregandoBebida:
                    if (bebidaCarregada > 0 && TemEspacoNoCaixaParaBebida())
                    {
                        EntregarBebida();
                    }
                    else
                    {
                        Debug.Log("Tudo entregue, esperando nova tarefa.");
                        estadoAtual = Estado.Esperando;
                    }
                    break;

                case Estado.Esperando:
                    // Aqui alterna entre tentar ir pro forno e freezer para evitar ficar parado
                    if (alternaParaForno)
                    {
                        TentarIrParaForno();
                    }
                    else
                    {
                        TentarIrParaFreezer();
                    }
                    alternaParaForno = !alternaParaForno;
                    break;
            }
        }
        else
        {
            // Continua andando se ainda não chegou
            if (agent != null)
                agent.isStopped = false;
        }
    }

    // Gira suavemente para olhar para uma posição alvo (usada quando chegou no destino)
    private void OlharParaAlvo(Vector3 posicaoAlvo)
    {
        if (posicaoAlvo == Vector3.zero) return;

        Vector3 direcao = posicaoAlvo - transform.position;
        direcao.y = 0f;
        if (direcao.sqrMagnitude <= 0.0001f) return;

        Quaternion rotacaoFinal = Quaternion.LookRotation(direcao.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotacaoFinal, Time.deltaTime * rotacaoSpeed);
    }

    // --- Métodos auxiliares ---
    private void TentarIrParaForno()
    {
        if (forno != null && forno.GetEstoqueAtual() > 0 && TemEspacoParaProduto())
        {
            estadoAtual = Estado.IndoParaForno;
            agent.isStopped = false;
            Vector3 pos = GetPosicaoInteracao(fornoTrigger);
            destinoAtual = pos;
            agent.SetDestination(pos);
            Debug.Log("Indo para o forno.");
        }
        else
        {
            TentarIrParaFreezer();
        }
    }

    private void TentarIrParaFreezer()
    {
        if (freezer != null && freezer.GetQuantidadeCongelada() > 0 && TemEspacoParaBebida())
        {
            IrParaFreezer();
        }
        else
        {
            estadoAtual = Estado.Esperando;
            Debug.Log("Sem tarefas disponíveis, aguardando.");
        }
    }

    private void IrParaCaixaComProduto()
    {
        estadoAtual = Estado.IndoParaCaixaComProduto;
        agent.isStopped = false;
        Vector3 pos = GetPosicaoInteracao(caixaTrigger);
        destinoAtual = pos;
        agent.SetDestination(pos);
        Debug.Log("Indo para o caixa com produto.");
    }

    private void IrParaCaixaComBebida()
    {
        estadoAtual = Estado.IndoParaCaixaComBebida;
        agent.isStopped = false;
        Vector3 pos = GetPosicaoInteracao(caixaTrigger);
        destinoAtual = pos;
        agent.SetDestination(pos);
        Debug.Log("Indo para o caixa com bebida.");
    }

    private void IrParaFreezer()
    {
        estadoAtual = Estado.IndoParaFreezer;
        agent.isStopped = false;
        Vector3 pos = GetPosicaoInteracao(freezerTrigger);
        destinoAtual = pos;
        agent.SetDestination(pos);
        Debug.Log("Indo para o freezer.");
    }

    private Vector3 GetPosicaoInteracao(Collider trigger)
    {
        if (trigger == null)
        {
            Debug.LogError("Trigger nulo ao tentar obter posição de interação!");
            return transform.position;
        }
        return trigger.bounds.center;
    }

    // --- Controle de carga e interação ---
    private bool TemEspacoParaProduto()
    {
        return produtoCarregado < capacidadeProduto;
    }

    private bool TemEspacoParaBebida()
    {
        return bebidaCarregada < capacidadeBebida;
    }

    private bool TemEspacoNoCaixaParaProduto()
    {
        if (caixa == null) return false;
        return caixa.estoqueItem < caixa.capacidadeItem;
    }

    private bool TemEspacoNoCaixaParaBebida()
    {
        if (caixa == null) return false;
        return caixa.estoqueBebida < caixa.capacidadeBebida;
    }

    private void PegarProduto()
    {
        if (forno.ConsumirProduto())
        {
            produtoCarregado++;
            Debug.Log($"Produto coletado do forno. Carga atual: {produtoCarregado}/{capacidadeProduto}");
        }
        else
        {
            Debug.LogWarning("Falha ao coletar produto do forno.");
        }
    }

    private void EntregarProduto()
    {
        int espaçoNoCaixa = caixa.capacidadeItem - caixa.estoqueItem;
        int paraEntregar = Mathf.Min(produtoCarregado, espaçoNoCaixa);

        if (paraEntregar <= 0)
        {
            Debug.Log("Caixa cheio, não pode entregar produto agora.");
            return;
        }

        caixa.AdicionarProduto(paraEntregar);
        produtoCarregado -= paraEntregar;
        Debug.Log($"Produto entregue no caixa. Produto restante no NPC: {produtoCarregado}");
    }

    private void PegarBebida()
    {
        if (freezer.ConsumirItemCongelado())
        {
            bebidaCarregada++;
            Debug.Log($"Bebida coletada do freezer. Carga atual: {bebidaCarregada}/{capacidadeBebida}");
        }
        else
        {
            Debug.LogWarning("Falha ao coletar bebida do freezer.");
        }
    }

    private void EntregarBebida()
    {
        int espaçoNoCaixa = caixa.capacidadeBebida - caixa.estoqueBebida;
        int paraEntregar = Mathf.Min(bebidaCarregada, espaçoNoCaixa);

        if (paraEntregar <= 0)
        {
            Debug.Log("Caixa cheio, não pode entregar bebida agora.");
            return;
        }

        caixa.AdicionarBebida(paraEntregar);
        bebidaCarregada -= paraEntregar;
        Debug.Log($"Bebida entregue no caixa. Bebida restante no NPC: {bebidaCarregada}");
    }
}
