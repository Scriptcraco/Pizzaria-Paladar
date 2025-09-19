using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class DriveThruVendaTrigger : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private CaixaDoDrive caixaDrive;
    [SerializeField] private PlayerController player;
    [SerializeField] private CarSpawner spawner;
     public int valorbebidanodrive = 10;
    public int valorpizzanodrive = 60;


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
            GameObject carroGO = spawner.ObterCarroFrente();

            if (carroGO == null)
            {
                yield return new WaitForSeconds(intervaloVenda);
                continue;
            }

            CarroCliente cliente = carroGO.GetComponent<CarroCliente>();

            if (cliente == null || cliente.PedidoAtendido() || !cliente.EstaParado())
            {
                yield return new WaitForSeconds(intervaloVenda);
                continue;
            }

            int qtdPizza = cliente.quantidadePizza;
            int qtdBebida = cliente.quantidadeBebida;

            bool podeAtender = caixaDrive.GetEstoqueProdutoEmbalado() >= qtdPizza &&
                               caixaDrive.GetEstoqueBebidaEmbalada() >= qtdBebida;

            if (podeAtender)
            {
                caixaDrive.RemoverProdutoEmbalado(qtdPizza);
                caixaDrive.RemoverBebidaEmbalada(qtdBebida);

                int valor = (qtdPizza * valorpizzanodrive) + (qtdBebida * valorbebidanodrive);
                player.AdicionarDinheiro(valor);

                cliente.AtenderPedido();
                cliente.IrEmbora();

                spawner.SairDaFila(carroGO);
            }

            yield return new WaitForSeconds(intervaloVenda);
        }
    }
}
