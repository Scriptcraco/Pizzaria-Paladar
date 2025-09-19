using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class CaixaVendaTrigger : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Caixa caixa;
    [SerializeField] private PlayerController player;
    [SerializeField] private FilaCaixaManager filaManager;

    public int valorbebida = 5;
    public int valorpizza = 30;

    [Header("Configurações")]
    [SerializeField] public float intervaloVenda = 0.5f;

    private Coroutine vendaCoroutine;

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (vendaCoroutine == null)
        {
            vendaCoroutine = StartCoroutine(ProcessarVendas());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (vendaCoroutine != null)
        {
            StopCoroutine(vendaCoroutine);
            vendaCoroutine = null;
        }
    }

    private IEnumerator ProcessarVendas()
    {
        while (true)
        {
            GameObject clienteGO = filaManager.ObterClienteFrente();

            if (clienteGO == null)
            {
                yield return new WaitForSeconds(intervaloVenda);
                continue;
            }

            Client cliente = clienteGO.GetComponent<Client>();
            if (cliente == null || cliente.pedidoEntregue)
            {
                yield return new WaitForSeconds(intervaloVenda);
                continue;
            }

            // VERIFICA SE CLIENTE ESTÁ PARADO na fila
            if (!cliente.EstaParado())
            {
                yield return new WaitForSeconds(intervaloVenda);
                continue;
            }

            int qtdPizza = cliente.quantidadePizza;
            int qtdBebida = cliente.quantidadeBebida;

            if (qtdPizza <= 0 && qtdBebida <= 0)
            {
                yield return new WaitForSeconds(intervaloVenda);
                continue;
            }

            bool podeAtender =
                caixa.GetestoqueItem() >= qtdPizza &&
                caixa.GetestoqueBebida() >= qtdBebida;

            if (podeAtender)
            {
                caixa.RemoverItem(qtdPizza);
                caixa.RemoverBebida(qtdBebida);

                int valor = (qtdPizza * valorpizza) + (qtdBebida * valorbebida);
                player.AdicionarDinheiro(valor);

                cliente.pedidoEntregue = true;
                cliente.IrParaMesa();

                filaManager.SairDaFila(cliente.gameObject);
                filaManager.AvancarFila();
            }

            yield return new WaitForSeconds(intervaloVenda);
        }
    }
}
