using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class FreezerTriggerArea : MonoBehaviour
{
    [SerializeField] private Freezer freezer; // atribuir no Inspector
    [SerializeField] private float intervaloTransferencia = 0.5f; // segundos entre cada item transferido

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
        if (freezer == null || player == null)
        {
            Debug.LogWarning("Freezer ou Player não configurados corretamente.");
            yield break;
        }

        while (true)
        {
            int capacidadeRestante = player.GetEstoqueMaximoBebida() - player.GetEstoqueAtualBebida();
            if (capacidadeRestante <= 0)
            {
                Debug.Log("Player com estoque cheio. Parando transferência.");
                break;
            }

            int disponivelFreezer = freezer.GetQuantidadeCongelada();
            if (disponivelFreezer <= 0)
            {
                Debug.Log("Freezer vazio. Parando transferência.");
                break;
            }

            if (freezer.ConsumirItemCongelado())
            {
                player.AdicionarBebida(1);
                Debug.Log("Item transferido do freezer para o player.");
            }
            else
            {
                Debug.LogWarning("Falha ao consumir item do freezer.");
                break;
            }

            yield return new WaitForSeconds(intervaloTransferencia);
        }

        transferenciaCoroutine = null;
    }
}
