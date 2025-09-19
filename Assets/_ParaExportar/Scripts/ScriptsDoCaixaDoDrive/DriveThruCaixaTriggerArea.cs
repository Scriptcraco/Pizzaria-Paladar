using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class DriveThruCaixaTriggerArea : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private CaixaDoDrive caixaDoDrive;

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
                transferenciaCoroutine = StartCoroutine(TransferirItensEmbalados(player));
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

    private IEnumerator TransferirItensEmbalados(PlayerController player)
    {
        while (true)
        {
            bool transferiu = false;

            // PRODUTO EMBALADO
            if (player.estoqueAtualEmpacotado > 0 && caixaDoDrive.GetEstoqueProdutoEmbalado() < caixaDoDrive.GetCapacidadeProdutoEmbalado())
            {
                player.estoqueAtualEmpacotado--;
                caixaDoDrive.AdicionarProdutoEmbalado(1);
                transferiu = true;
            }

            // BEBIDA EMBALADA
            else if (player.estoqueAtualBebida > 0 && caixaDoDrive.GetEstoqueBebidaEmbalada() < caixaDoDrive.GetCapacidadeBebidaEmbalada())
            {
                player.estoqueAtualBebida--;
                caixaDoDrive.AdicionarBebidaEmbalada(1);
                transferiu = true;
            }

            if (!transferiu)
                break;

            yield return new WaitForSeconds(intervaloTransferencia);
        }

        transferenciaCoroutine = null;
    }
}
