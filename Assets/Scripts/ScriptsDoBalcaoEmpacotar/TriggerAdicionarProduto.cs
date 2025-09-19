using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TriggerAdicionarProduto : MonoBehaviour
{
    [SerializeField] private BalcaoEmpacotar balcao;
    [SerializeField] private float intervalo = 1f;

    private Coroutine coroutine;

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (coroutine == null)
            coroutine = StartCoroutine(AdicionarProdutos(other.GetComponent<PlayerController>()));
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

    private System.Collections.IEnumerator AdicionarProdutos(PlayerController player)
    {
        while (balcao.PodeAdicionarProduto(1) && player.GetEstoqueAtual() > 0) // Passa a quantidade
        {
            balcao.AdicionarProduto(1); // Passa a quantidade
            player.RemoverItem(1);
            Debug.Log("📦 Produto adicionado ao balcão.");
            yield return new WaitForSeconds(intervalo);
        }

        coroutine = null;
    }
}
