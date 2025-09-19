using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class FornoTriggerArea : MonoBehaviour
{
    [SerializeField] private ProductionUnit forno; // atribuir no Inspector
    [SerializeField] private float intervaloTransferencia = 0.5f;

    private Coroutine transferenciaCoroutine;

    private void Reset()
    {
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (transferenciaCoroutine == null)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            transferenciaCoroutine = StartCoroutine(TransferirItensGradualmente(player));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (transferenciaCoroutine != null)
        {
            StopCoroutine(transferenciaCoroutine);
            transferenciaCoroutine = null;
        }
    }

    private IEnumerator TransferirItensGradualmente(PlayerController player)
    {
        if (forno == null || player == null)
        {
            Debug.LogWarning("Forno ou Player não configurados corretamente.");
            yield break;
        }

        while (true)
        {
            int capacidadeRestante = player.GetEstoqueMaximo() - player.GetEstoqueAtual();
            if (capacidadeRestante <= 0)
            {
                Debug.Log("Player com estoque cheio. Parando transferência.");
                break;
            }

            int disponivelForno = forno.GetEstoqueAtual();
            if (disponivelForno <= 0)
            {
                Debug.Log("Forno vazio. Parando transferência.");
                break;
            }

            if (forno.ConsumirProduto())
            {
                player.AdicionarProduto(1); // adapte se for outro tipo de produto
                Debug.Log("Item transferido do forno para o player.");
            }
            else
            {
                Debug.LogWarning("Falha ao consumir item do forno.");
                break;
            }

            yield return new WaitForSeconds(intervaloTransferencia);
        }

        transferenciaCoroutine = null;
    }
}
