using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TriggerCompraMesa : MonoBehaviour
{
    [Header("Configuração da Compra")]
    public int custoInicial = 200;
    public float expoenteCusto = 1.15f; // fator de escalonamento do custo
    public int custoAtual; // custo recalculado
    public float tempoInicialDesconto = 0.3f;
    public float taxaAceleracao = 0.1f;
    public float tempoMinimoDesconto = 0.05f;
    public float multiplicadorQuantidadePorSegundo = 1f;

    [Header("Referências")]
    public GameObject mesaParaAtivar;
    public GameObject displayCompra;
    public GameObject visualBloqueado; // efeito "locked"

    [Header("UI")]
    public TextMeshProUGUI textoCusto;
    public Image barraProgresso;

    private int pagoAtual;
    private bool playerDentro;
    private bool disponivelParaCompra;
    private Coroutine rotinaDesconto;
    private float tempoParado;
    private PlayerController player;

    void Start()
    {
        player = Object.FindFirstObjectByType<PlayerController>();

        if (mesaParaAtivar != null)
            mesaParaAtivar.SetActive(false);

        RecalcularCusto();
        AtualizarUI();
    }

    public void SetDisponivelParaCompra(bool valor)
    {
        disponivelParaCompra = valor;

        if (visualBloqueado != null)
            visualBloqueado.SetActive(!valor);

        if (displayCompra != null)
            displayCompra.SetActive(valor);
    }

    void AtualizarUI()
    {
        if (barraProgresso != null)
        {
            RectTransform rt = barraProgresso.GetComponent<RectTransform>();
            if (rt != null)
            {
                float novoTop = Mathf.Lerp(470f, 0f, (float)pagoAtual / custoAtual);
                Vector2 offsetMax = rt.offsetMax;
                offsetMax.y = -novoTop;
                rt.offsetMax = offsetMax;
            }
        }

        if (textoCusto != null)
            textoCusto.text = (custoAtual - pagoAtual).ToString();
    }

    // recalcula custo baseado na quantidade de mesas compradas
    void RecalcularCusto()
    {
        int quantidadeComprada = MesaManager.Instance.GetQuantidadeMesasCompradas();
        custoAtual = Mathf.FloorToInt(custoInicial * Mathf.Pow(quantidadeComprada + 1, expoenteCusto));
        pagoAtual = 0;
        Debug.Log($"Custo recalculado: {custoAtual} (mesas compradas: {quantidadeComprada})");
    }

    IEnumerator ProcessoDesconto()
    {
        tempoParado = 0f;

        while (playerDentro && disponivelParaCompra)
        {
            if (pagoAtual < custoAtual && player.dinheiro > 0)
            {
                int quantidadeDesconto = Mathf.FloorToInt(tempoParado * multiplicadorQuantidadePorSegundo);
                quantidadeDesconto = Mathf.Max(1, quantidadeDesconto);

                int desconto = Mathf.Min(quantidadeDesconto, player.dinheiro, custoAtual - pagoAtual);

                player.dinheiro -= desconto;
                pagoAtual += desconto;

                AtualizarUI();

                if (pagoAtual >= custoAtual)
                {
                    CompraConcluida();
                    yield break;
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

    void CompraConcluida()
    {
        if (displayCompra != null)
            displayCompra.SetActive(false);

        if (mesaParaAtivar != null)
            mesaParaAtivar.SetActive(true);

        // pega cadeiras automaticamente em todos os níveis filhos e adiciona ao gerenciador
        FilaMesaManager fila = Object.FindFirstObjectByType<FilaMesaManager>();
        if (fila != null)
        {
            List<Transform> cadeirasMesa = new List<Transform>();

            // pega todos os filhos e descendentes da mesa ativada
            Transform[] todosFilhos = mesaParaAtivar.GetComponentsInChildren<Transform>(true);
            foreach (Transform filho in todosFilhos)
            {
                if (filho.CompareTag("Cadeira") && filho.gameObject.activeInHierarchy)
                {
                    cadeirasMesa.Add(filho);
                }
            }

            Debug.Log($"Adicionando {cadeirasMesa.Count} cadeiras da mesa comprada.");
            fila.AdicionarCadeiras(cadeirasMesa);
        }
        else
        {
            Debug.LogWarning("FilaMesaManager não encontrado na cena!");
        }

        // avisa o gerenciador que foi comprada
        MesaManager.Instance.MesaComprada(this);

        // recalcula custo da próxima mesa
        RecalcularCusto();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && disponivelParaCompra)
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
