using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class ContratarPlayer : MonoBehaviour
{
    public enum FuncaoNPC
    {
        CarregadorCaixa,
        CarregadorDrive,
        AtendenteCaixa,
        AtendenteDrive,
        Empacotador
    }

    [Header("Configuração Inicial")]
    public int custoInicial = 300;
    public float tempoInicialDesconto = 0.3f;
    public float taxaAceleracao = 0.1f;
    public float tempoMinimoDesconto = 0.05f;
    public float multiplicadorQuantidadePorSegundo = 1f;

    [Header("UI")]
    public TextMeshProUGUI textoCusto;
    public Image barraProgresso;

    private int npcPrefabIndex = 0;

    [Header("Referências")]
    public FuncaoNPC funcaoNPC;
    public GameObject npcCarregadorCaixa;
    public GameObject npcCarregadorDrive;
    public GameObject npcAtendenteCaixa;
    public GameObject npcAtendenteDrive;
    public GameObject npcEmpacotador;

    public Transform pontoSpawnNPC;

    private int custoAtual;
    private int pagoAtual;
    private int numeroContratados = 0;

    private bool playerDentro = false;
    private Coroutine rotinaDesconto;
    private float tempoParado;

    private PlayerController player;

    void Start()
    {
        custoAtual = custoInicial;
        pagoAtual = 0;
        player = Object.FindFirstObjectByType<PlayerController>();

        AtualizarUI();
    }

    void AtualizarUI()
    {
        // Atualiza a barra de progresso da compra
        if (barraProgresso != null)
        {
            RectTransform rt = barraProgresso.GetComponent<RectTransform>();
            if (rt != null)
            {
                // Preenchimento da barra de baixo para cima proporcional ao progresso
                float novoTop = Mathf.Lerp(470f, 0f, (float)pagoAtual / custoAtual);
                Vector2 offsetMax = rt.offsetMax;
                offsetMax.y = -novoTop;
                rt.offsetMax = offsetMax;
            }
        }

        if (textoCusto != null)
        {
            textoCusto.text = (custoAtual - pagoAtual).ToString();
        }
    }

    IEnumerator ProcessoDesconto()
    {
        tempoParado = 0f;

        while (playerDentro)
        {
            if (pagoAtual < custoAtual && player != null && player.dinheiro > 0)
            {
                int quantidadeDesconto = Mathf.FloorToInt(tempoParado * multiplicadorQuantidadePorSegundo);
                quantidadeDesconto = Mathf.Max(1, quantidadeDesconto);

                int desconto = Mathf.Min(quantidadeDesconto, player.dinheiro, custoAtual - pagoAtual);

                player.dinheiro -= desconto;
                pagoAtual += desconto;

                AtualizarUI();

                if (pagoAtual >= custoAtual)
                {
                    ContratarNPC();
                    ResetarCompra();
                }

                float tempoEntre = Mathf.Max(tempoMinimoDesconto, tempoInicialDesconto - (tempoParado * taxaAceleracao));
                yield return new WaitForSeconds(tempoEntre);
                tempoParado += tempoEntre;
            }
            else
            {
                yield return null;
            }
        }
    }

  void ContratarNPC()
{
    GameObject npcPrefab = null;

    switch (npcPrefabIndex)
    {
        case 0:
            npcPrefab = npcCarregadorCaixa;
            break;
        case 1:
            npcPrefab = npcCarregadorDrive;
            break;
        case 2:
            npcPrefab = npcAtendenteCaixa;
            break;
        case 3:
            npcPrefab = npcAtendenteDrive;
            break;
        case 4:
            npcPrefab = npcEmpacotador;
            break;
    }

    // Incrementa índice para próxima contratação
    npcPrefabIndex++;
    if (npcPrefabIndex > 4)
    {
        npcPrefabIndex = 0; // volta ao início da lista
    }

    // Aqui você pode instanciar o NPC usando npcPrefab


        if (npcPrefab != null && pontoSpawnNPC != null)
        {
            GameObject npc = Instantiate(npcPrefab, pontoSpawnNPC.position, pontoSpawnNPC.rotation);
            numeroContratados++;

            NPCController controller = npc.GetComponent<NPCController>();
            if (controller != null)
            {
                controller.funcaoNPC = funcaoNPC.ToString();
            }
            else
            {
                Debug.LogWarning("NPC prefab não tem NPCController");
            }

            Debug.Log($"NPC contratado! Função: {funcaoNPC} | Total: {numeroContratados}");
        }
        else
        {
            Debug.LogWarning("Prefab NPC ou ponto de spawn não configurados.");
        }
    }

    void ResetarCompra()
    {
        custoAtual = Mathf.FloorToInt(custoInicial * Mathf.Pow(numeroContratados + 1, 1.3f)); // expoente fixo
        pagoAtual = 0;
        tempoParado = 0f;
        AtualizarUI();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerDentro = true;
            if (rotinaDesconto == null)
                rotinaDesconto = StartCoroutine(ProcessoDesconto());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerDentro = false;
            if (rotinaDesconto != null)
            {
                StopCoroutine(rotinaDesconto);
                rotinaDesconto = null;
            }
        }
    }
}
