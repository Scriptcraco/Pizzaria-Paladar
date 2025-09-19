    using UnityEngine;
    using UnityEngine.AI;
    using System.Collections;

    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    public class NPC_AtendenteCaixa : MonoBehaviour
    {
        [Header("Referências automáticas via Tags")]
        public Collider caixaVendaTrigger;
        public Caixa caixa;
        public FilaCaixaManager filaManager;
        public PlayerController player;

        [Header("Configurações NPC")]
        public float velocidade = 2.5f;
        public float distanciaInteracao = 1.2f;
        public float intervaloVenda = 0.5f;

        [Header("Valores de venda")]
        public int valorBebida = 5;
        public int valorPizza = 30;

        private NavMeshAgent agent;
        private Animator animator;

        private enum Estado
        {
            IndoParaCaixa,
            AtendendoCliente,
            Esperando
        }

        private Estado estadoAtual = Estado.Esperando;
        private Coroutine atendimentoCoroutine;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            agent.speed = velocidade;
            agent.updateRotation = false; // Controle manual da rotação

            // Busca automática por tags (se não estiverem atribuídos no Inspector)
            if (caixaVendaTrigger == null)
            {
                GameObject caixaVendaTriggerGO = GameObject.FindGameObjectWithTag("CaixaVendaTrigger");
                if (caixaVendaTriggerGO != null)
                    caixaVendaTrigger = caixaVendaTriggerGO.GetComponent<Collider>();
                else
                    DevLog.Error("CaixaVendaTrigger não encontrado com tag 'CaixaVendaTrigger'");
            }

            if (caixa == null)
            {
                GameObject caixaGO = GameObject.FindGameObjectWithTag("Caixa");
                if (caixaGO != null)
                    caixa = caixaGO.GetComponent<Caixa>();
                else
                    DevLog.Error("Caixa não encontrado com tag 'Caixa'");
            }

            if (filaManager == null)
            {
                GameObject filaManagerGO = GameObject.FindGameObjectWithTag("FilaCaixaManager");
                if (filaManagerGO != null)
                    filaManager = filaManagerGO.GetComponent<FilaCaixaManager>();
                else
                    DevLog.Error("FilaCaixaManager não encontrado com tag 'FilaCaixaManager'");
            }

            if (player == null)
            {
                GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
                if (playerGO != null)
                    player = playerGO.GetComponent<PlayerController>();
                else
                    DevLog.Error("Player não encontrado com tag 'Player'");
            }
        }

        private void Start()
        {
            IrParaCaixa();
        }

        private void Update()
        {
            float speed = agent.velocity.magnitude > 0.1f ? 0.5f : 0f;
            if (animator != null)
                animator.SetFloat("Speed_f", speed);

            if (!agent.pathPending && agent.remainingDistance <= distanciaInteracao)
            {
                // Rotação fixa para olhar para o objeto Caixa (não o trigger)
                if (caixa != null)
                {
                    Vector3 direcaoAlvo = caixa.transform.position - transform.position;
                    direcaoAlvo.y = 0f;
                    if (direcaoAlvo.sqrMagnitude > 0.001f)
                    {
                        Quaternion rotFinal = Quaternion.LookRotation(direcaoAlvo.normalized);
                        transform.rotation = rotFinal; // rotação fixa, sem Lerp
                    }
                }

                if (estadoAtual == Estado.IndoParaCaixa)
                {
                    estadoAtual = Estado.AtendendoCliente;
                    if (atendimentoCoroutine == null)
                        atendimentoCoroutine = StartCoroutine(ProcessarVendas());
                }
            }
            else
            {
                // Se está andando, rotaciona suavemente na direção do movimento
                if (agent.velocity.sqrMagnitude > 0.01f)
                {
                    Vector3 dir = agent.velocity.normalized;
                    dir.y = 0;
                    if (dir != Vector3.zero)
                    {
                        Quaternion rot = Quaternion.LookRotation(dir);
                        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 8f);
                    }
                }

                if (estadoAtual == Estado.AtendendoCliente)
                {
                    // Se começou a se afastar da caixa enquanto atendia, cancela atendimento
                    if (atendimentoCoroutine != null)
                    {
                        StopCoroutine(atendimentoCoroutine);
                        atendimentoCoroutine = null;
                    }
                    estadoAtual = Estado.IndoParaCaixa;
                }
            }

            // Se está esperando e a fila não está vazia, vai para o caixa
            if (estadoAtual == Estado.Esperando && filaManager != null && filaManager.TemClientesNaFila())
            {
                IrParaCaixa();
            }
        }

        private void IrParaCaixa()
        {
            if (caixaVendaTrigger != null)
            {
                estadoAtual = Estado.IndoParaCaixa;
                agent.isStopped = false;
                agent.SetDestination(caixaVendaTrigger.bounds.center);
            }
        }

        private IEnumerator ProcessarVendas()
        {
            while (estadoAtual == Estado.AtendendoCliente)
            {
                GameObject clienteGO = filaManager.ObterClienteFrente();

                if (clienteGO != null)
                {
                    Client cliente = clienteGO.GetComponent<Client>();

                    // Só atende se cliente estiver parado e não tiver pedido entregue ainda
                    if (cliente != null && !cliente.pedidoEntregue && cliente.EstaParado())
                    {
                        int qtdPizza = cliente.quantidadePizza;
                        int qtdBebida = cliente.quantidadeBebida;

                        bool podeAtender =
                            caixa.GetestoqueItem() >= qtdPizza &&
                            caixa.GetestoqueBebida() >= qtdBebida;

                        if (podeAtender)
                        {
                            caixa.RemoverItem(qtdPizza);
                            caixa.RemoverBebida(qtdBebida);

                            int valor = (qtdPizza * valorPizza) + (qtdBebida * valorBebida);
                            player.AdicionarDinheiro(valor);

                            cliente.pedidoEntregue = true;
                            cliente.IrParaMesa();

                            filaManager.SairDaFila(cliente.gameObject);
                            filaManager.AvancarFila();

                            DevLog.Info($"Cliente atendido. Pedido: {qtdPizza} pizza(s), {qtdBebida} bebida(s). Valor ganho: {valor}");
                        }
                        else
                        {
                            DevLog.Info("Estoque insuficiente no caixa para atender o cliente.");
                        }
                    }
                }
                else
                {
                    // Não tem cliente na fila, volta a esperar
                    estadoAtual = Estado.Esperando;
                    atendimentoCoroutine = null;
                    yield break;
                }

                yield return new WaitForSeconds(intervaloVenda);
            }
        }
    }
