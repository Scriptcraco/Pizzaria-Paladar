using UnityEngine;
using UnityEngine.AI;

public class NPC_Empacotador : MonoBehaviour
{
    [Header("Referências automáticas via Tags")]
    public Collider fornoTrigger;
    public ProductionUnit forno;

    public Collider balcaoEmpacotarTriggerLargarCru;
    public Collider balcaoEmpacotarTriggerEmpacotar;
    public BalcaoEmpacotar balcaoEmpacotar;

    [Header("Configurações NPC")]
    public float velocidade = 3.5f;
    public int capacidadeProdutoCru = 10;
    public float distanciaInteracao = 1.2f;

    [Header("Rotação e animação")]
    public float rotacaoSpeed = 8f;

    private NavMeshAgent agent;
    private Animator animator;

    private enum Estado
    {
        IndoParaForno,
        PegandoProdutoCruNoForno,
        IndoParaBalcaoEmpacotarLargarCru,
        LargandoProdutoCruNoBalcao,
        IndoParaBalcaoEmpacotarEmpacotar,
        EmpacotandoProdutos,
    }

    private Estado estadoAtual = Estado.IndoParaForno;

    private int produtoCruCarregado = 0;
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

        AssignCollider("FornoTrigger", ref fornoTrigger);
        AssignComponent("Forno", ref forno);

        AssignCollider("BalcaoEmpacotarTriggerLargarCru", ref balcaoEmpacotarTriggerLargarCru);
        AssignCollider("BalcaoEmpacotarTriggerEmpacotar", ref balcaoEmpacotarTriggerEmpacotar);
        AssignComponent("BalcaoEmpacotar", ref balcaoEmpacotar);
    }

    private void Start()
    {
        IrParaForno();
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
                case Estado.IndoParaForno:
                    estadoAtual = Estado.PegandoProdutoCruNoForno;
                    break;

                case Estado.PegandoProdutoCruNoForno:
                    if (TemEspacoParaProdutoCru() && forno.GetEstoqueAtual() > 0)
                    {
                        if (forno.ConsumirProduto())
                            produtoCruCarregado++;
                    }
                    if (produtoCruCarregado >= capacidadeProdutoCru || forno.GetEstoqueAtual() == 0)
                        IrParaBalcaoEmpacotarLargarCru();
                    break;

                case Estado.IndoParaBalcaoEmpacotarLargarCru:
                    estadoAtual = Estado.LargandoProdutoCruNoBalcao;
                    break;

                case Estado.LargandoProdutoCruNoBalcao:
                    if (produtoCruCarregado > 0 && balcaoEmpacotar.GetCapacidadeProduto() > 0)
                    {
                        int paraLargar = Mathf.Min(produtoCruCarregado, balcaoEmpacotar.GetCapacidadeProduto());
                        balcaoEmpacotar.AdicionarProduto(paraLargar);
                        produtoCruCarregado -= paraLargar;
                    }
                    if (produtoCruCarregado <= 0)
                        IrParaBalcaoEmpacotarEmpacotar();
                    break;

                case Estado.IndoParaBalcaoEmpacotarEmpacotar:
                    estadoAtual = Estado.EmpacotandoProdutos;
                    break;
                    // if (balcaoEmpacotar.GetEstoqueProduto() > 0)
                   // {
                    //    int qtdEmpacotar = Mathf.Min(balcaoEmpacotar.GetEstoqueProduto(), capacidadeProdutoCru);
                     //   balcaoEmpacotar.PodeEmpacotar(qtdEmpacotar);
                case Estado.EmpacotandoProdutos:
                    if (balcaoEmpacotar.GetEstoqueProduto() > 0)
                    {
                        int quantidadeEmpacotar = Mathf.Min(balcaoEmpacotar.GetEstoqueProduto(), capacidadeProdutoCru);
                        balcaoEmpacotar.EmpacotarProduto(quantidadeEmpacotar);
                        Debug.Log($"NPC: Empacotando {quantidadeEmpacotar} produtos.");
                    }
                    else
                        IrParaForno();
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

    private void IrParaForno()
    {
        estadoAtual = Estado.IndoParaForno;
        agent.isStopped = false;
        destinoAtual = fornoTrigger.bounds.center;
        agent.SetDestination(destinoAtual);
    }

    private void IrParaBalcaoEmpacotarLargarCru()
    {
        estadoAtual = Estado.IndoParaBalcaoEmpacotarLargarCru;
        agent.isStopped = false;
        destinoAtual = balcaoEmpacotarTriggerLargarCru.bounds.center;
        agent.SetDestination(destinoAtual);
    }

    private void IrParaBalcaoEmpacotarEmpacotar()
    {
        estadoAtual = Estado.IndoParaBalcaoEmpacotarEmpacotar;
        agent.isStopped = false;
        destinoAtual = balcaoEmpacotarTriggerEmpacotar.bounds.center;
        agent.SetDestination(destinoAtual);
    }

    private bool TemEspacoParaProdutoCru()
    {
        return produtoCruCarregado < capacidadeProdutoCru;
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
}
