using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class ComprarAreaNovaTrigger : MonoBehaviour
{
    [Header("Configuração da Compra")]
    public int custoInicial = 500;
    public float tempoInicialDesconto = 0.3f;
    public float taxaAceleracao = 0.1f;
    public float tempoMinimoDesconto = 0.05f;
    public float multiplicadorQuantidadePorSegundo = 1f;

    [Header("UI")]
    public TextMeshProUGUI textoCusto;  
    public Image barraProgresso;

    [Header("Referências")]
    public GameObject objetoParaDestruir; // objeto que bloqueia a área

    private int custoAtual;
    private int pagoAtual;
    private bool playerDentro;
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
        if (barraProgresso != null)
        {
            RectTransform rt = barraProgresso.GetComponent<RectTransform>();
            if (rt != null)
            {
                // Mapeia a barra preenchendo da base para cima proporcional ao progresso
                float novoTop = Mathf.Lerp(470f, 0f, (float)pagoAtual / custoAtual);
                Vector2 offsetMax = rt.offsetMax;
                offsetMax.y = -novoTop;
                rt.offsetMax = offsetMax;
            }
        }

        if (textoCusto != null)
            textoCusto.text = (custoAtual - pagoAtual).ToString();
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

                Debug.Log($"Pagando: {pagoAtual}/{custoAtual}");

                if (pagoAtual >= custoAtual)
                {
                    if (objetoParaDestruir != null)
                    {
                        Destroy(objetoParaDestruir);
                        Debug.Log($"Objeto '{objetoParaDestruir.name}' destruído - área liberada.");
                    }
                    Destroy(gameObject);
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
