using System.Collections.Generic;
using UnityEngine;

public class FilaCaixaManager : MonoBehaviour
{
    [SerializeField] private Transform[] posicoesFila; // Posições da fila em ordem exata
    private List<ClienteNaFila> clientesNaFila = new List<ClienteNaFila>();

    // Tenta entrar na fila na última posição disponível
    public bool TentarEntrarNaFila(GameObject cliente)
    {
        if (clientesNaFila.Count >= posicoesFila.Length)
            return false; // fila cheia

        int posIndex = clientesNaFila.Count;
        Transform posicao = posicoesFila[posIndex];

        clientesNaFila.Add(new ClienteNaFila(cliente, posicao));
        cliente.GetComponent<Client>().MoverPara(posicao.position);
        return true;
    }

    // Remove o cliente da fila (normalmente o da frente)
    public void SairDaFila(GameObject cliente)
    {
        int index = clientesNaFila.FindIndex(c => c.cliente == cliente);
        if (index == -1) return;

        clientesNaFila.RemoveAt(index);
        AvancarFila();
    }

    // Cliente que está na frente da fila
    public GameObject ObterClienteFrente()
    {
        if (clientesNaFila.Count == 0) return null;
        return clientesNaFila[0].cliente;
    }

    // Move todos os clientes para a posição anterior (avança a fila)
    public void AvancarFila()
    {
        for (int i = 0; i < clientesNaFila.Count; i++)
        {
            Transform novaPosicao = posicoesFila[i];
            clientesNaFila[i].posicao = novaPosicao;
            clientesNaFila[i].cliente.GetComponent<Client>().MoverPara(novaPosicao.position);
        }
    }

    public bool TemClientesNaFila()
    {
        return clientesNaFila.Count > 0;
    }

    private class ClienteNaFila
    {
        public GameObject cliente;
        public Transform posicao;

        public ClienteNaFila(GameObject c, Transform p)
        {
            cliente = c;
            posicao = p;
        }
    }
}
