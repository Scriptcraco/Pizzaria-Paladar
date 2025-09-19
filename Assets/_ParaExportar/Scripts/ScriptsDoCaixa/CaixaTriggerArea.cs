using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class CaixaTriggerArea : MonoBehaviour
{
    [SerializeField] private Caixa caixa; // Referência ao script do caixa
    [SerializeField] private float intervaloTransferencia = 0.5f;

    private Coroutine transferenciaCoroutine;

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (transferenciaCoroutine == null)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
                transferenciaCoroutine = StartCoroutine(TransferirItensParaCaixa(player));
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

    private IEnumerator TransferirItensParaCaixa(PlayerController player)
    {
        while (true)
        {
            bool transferiu = false;

            // Transferência de PRODUTO
            if (player.GetEstoqueAtual() > 0 && caixa.GetestoqueItem() < caixa.GetCapacidadeItem()  )
            {
                player.RemoverItem(1);
                caixa.AdicionarProduto(1);
                Debug.Log("🍔 Produto transferido para o caixa.");
                transferiu = true;
            }

            // Transferência de BEBIDA
            else if (player.GetEstoqueAtualBebida() > 0 && caixa.GetestoqueBebida() < caixa.GetCapacidadeBebida())
            {
                player.RemoverBebida(1);
                caixa.AdicionarBebida(1);
                Debug.Log("🥤 Bebida transferida para o caixa.");
                transferiu = true;
            }

            if (!transferiu)
                break;

            yield return new WaitForSeconds(intervaloTransferencia);
        }

        transferenciaCoroutine = null;
    }
}
