using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TriggerEmpacotar : MonoBehaviour
{
    [SerializeField] private BalcaoEmpacotar balcao;
    [SerializeField] private float intervalo = 1.0f;

    private Coroutine coroutine;

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (coroutine == null)
            coroutine = StartCoroutine(Empacotar());
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

    private System.Collections.IEnumerator Empacotar()
    {
        while (balcao.PodeEmpacotar(1)) // Passa a quantidade
        {
            balcao.EmpacotarProduto(1); // Passa a quantidade
            Debug.Log("📦 Produto empacotado.");
            yield return new WaitForSeconds(intervalo);
        }

        coroutine = null;
    }
}
