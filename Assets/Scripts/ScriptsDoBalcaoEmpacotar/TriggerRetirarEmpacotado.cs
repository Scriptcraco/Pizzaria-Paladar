using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TriggerRetirarEmpacotado : MonoBehaviour
{
    [SerializeField] private BalcaoEmpacotar balcao;
    [SerializeField] private float intervalo = 1f;

    private Coroutine coroutine;

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (coroutine == null)
            coroutine = StartCoroutine(RetirarProdutos(other.GetComponent<PlayerController>()));
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    private System.Collections.IEnumerator RetirarProdutos(PlayerController player)
    {
        while (balcao.PodeRetirarEmpacotado(1)) // Passa a quantidade
        {
            balcao.RetirarEmpacotado(1); // Passa a quantidade
            player.AdicionarItem(1);
            Debug.Log("📦 Produto empacotado retirado.");
            yield return new WaitForSeconds(intervalo);
        }

        coroutine = null;
    }
}
