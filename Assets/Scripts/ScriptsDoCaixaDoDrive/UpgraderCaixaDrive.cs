using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UpgraderCaixaDrive : MonoBehaviour
{
    [Header("Referência ao Pacote")]
    public CaixaDoDrive caixaDAlvo;
    public DriveThruVendaTrigger driveThruVendaTrigger;

    [Header("Configuração Inicial")]
    public int custoInicial = 100;
    public float expoenteCusto = 2.2f;

    [Header("Multiplicadores de Upgrade")]
    public float multiplicadorCapacidade = 1.5f;   // +50% por nível
    public float multiplicadorVelocidade = 1.10f;  // +10% por nível

    [Header("UI de Progresso")]
    public TextMeshProUGUI nivelTexto;  // texto que mostra o nível
    public Image barraProgresso;        // barra que será preenchida
    public TextMeshProUGUI textoCusto;  // texto que mostra valor restante

    [Header("Configuração de Desconto")]
    public float tempoInicialDesconto = 0.3f;  // tempo inicial entre descontos (segundos)
    public float taxaAceleracao = 0.1f;        // redução do tempo a cada segundo parado (segundos)
    public float tempoMinimoDesconto = 0.05f;  // limite mínimo de tempo entre descontos (segundos)

    [Header("Multiplicador de Quantidade por Tempo Parado")]
    public float multiplicadorQuantidadePorSegundo = 1f; // quantidade aumenta conforme tempo parado

    private int custoAtual;
    private int pagoAtual;
    private bool playerDentro;
    private Coroutine rotinaDesconto;
    private float tempoParado;

    private PlayerController player;  // referência ao script do player

    void Start()
    {
        if (caixaDAlvo == null)
        {
            Debug.LogError("Caixa alvo não foi atribuído no Upgrader!");
            enabled = false;
            return;
        }

        player = Object.FindFirstObjectByType<PlayerController>();

        RecalcularCusto();
        AtualizarUI();
    }

    void RecalcularCusto()
    {
        custoAtual = Mathf.FloorToInt(custoInicial * Mathf.Pow(caixaDAlvo.GetNivelDoCaixaDrive(), expoenteCusto));
        pagoAtual = 0;
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

        if (nivelTexto != null)
            nivelTexto.text = "Nível: " + caixaDAlvo.GetNivelDoCaixaDrive();
    }

    IEnumerator ProcessoDesconto()
    {
        tempoParado = 0f;

        while (playerDentro)
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
                    AplicarUpgrade();
                    RecalcularCusto();
                    AtualizarUI();
                    tempoParado = 0f;
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

    void AplicarUpgrade()
    {
        caixaDAlvo.nivelDocaixaDrive++;

        caixaDAlvo.capacidadeBebidaEmbalada = Mathf.FloorToInt(caixaDAlvo.capacidadeBebidaEmbalada * multiplicadorCapacidade);
        caixaDAlvo.capacidadeProdutoEmbalado = Mathf.FloorToInt(caixaDAlvo.GetCapacidadeProdutoEmbalado() * multiplicadorCapacidade);

        driveThruVendaTrigger.intervaloVenda = Mathf.Max(0.1f, driveThruVendaTrigger.intervaloVenda / multiplicadorVelocidade);

        Debug.Log($"Upgrade aplicado! Novo nível: {caixaDAlvo.GetNivelDoCaixaDrive()} | Capacidade Bebida: {caixaDAlvo.GetCapacidadeBebidaEmbalada()} | Capacidade Produto: {caixaDAlvo.GetCapacidadeProdutoEmbalado()} | Velocidade: {driveThruVendaTrigger.intervaloVenda}s");
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
