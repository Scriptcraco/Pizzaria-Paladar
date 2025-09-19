using System.Collections.Generic;
using UnityEngine;

public class FilaDriveThruManager : MonoBehaviour
{
    [SerializeField] private Transform[] posicoesFila;

    private class CarroNaFila
    {
        public GameObject carro;
        public Transform posicao;

        public CarroNaFila(GameObject c, Transform p)
        {
            carro = c;
            posicao = p;
        }
    }

    private List<CarroNaFila> carrosNaFila = new List<CarroNaFila>();

    private void Update()
    {
        LimparCarrosInvalidos();
    }

    public bool TentarEntrarNaFila(GameObject carro)
    {
        if (carrosNaFila.Count >= posicoesFila.Length)
        {
            Debug.LogWarning($"[FilaDriveThruManager] Fila cheia para {carro.name}.");
            return false;
        }

        Transform posicao = posicoesFila[carrosNaFila.Count];
        carrosNaFila.Add(new CarroNaFila(carro, posicao));
        carro.GetComponent<CarroCliente>().MoverPara(posicao.position);

        Debug.Log($"[FilaDriveThruManager] Carro {carro.name} entrou na fila na posição {carrosNaFila.Count - 1}.");
        return true;
    }

    public void SairDaFila(GameObject carro)
    {
        int index = carrosNaFila.FindIndex(c => c.carro == carro);
        if (index == -1)
        {
            Debug.LogWarning($"[FilaDriveThruManager] Tentativa de remover carro não presente: {carro.name}");
            return;
        }

        carrosNaFila.RemoveAt(index);
        Debug.Log($"[FilaDriveThruManager] Carro {carro.name} saiu da fila.");
        AvancarFila();
    }

    public void AvancarFila()
    {
        for (int i = 0; i < carrosNaFila.Count; i++)
        {
            carrosNaFila[i].posicao = posicoesFila[i];
            carrosNaFila[i].carro.GetComponent<CarroCliente>().MoverPara(posicoesFila[i].position);
            Debug.Log($"[FilaDriveThruManager] Carro {carrosNaFila[i].carro.name} avançou para a posição {i}.");
        }
    }

    private void LimparCarrosInvalidos()
    {
        int antes = carrosNaFila.Count;
        carrosNaFila.RemoveAll(c => c.carro == null || !c.carro.activeInHierarchy);
        int depois = carrosNaFila.Count;
        if (depois < antes)
        {
            Debug.Log($"[FilaDriveThruManager] Removidos {antes - depois} carros inválidos.");
            AvancarFila();
        }
    }

    public Vector3 ObterPosicaoFila(int index)
    {
        if (index >= 0 && index < posicoesFila.Length)
            return posicoesFila[index].position;
        Debug.LogWarning("[FilaDriveThruManager] Índice inválido para posição na fila.");
        return Vector3.zero;
    }

    public int ObterQuantidadeCarros()
    {
        return carrosNaFila.Count;
    }

    public int CapacidadeMaxima()
    {
        return posicoesFila.Length;
    }

    public bool TemCarroNaPosicao(int index)
    {
        if (index < 0 || index >= carrosNaFila.Count)
            return false;

        GameObject carro = carrosNaFila[index].carro;
        return carro != null && carro.activeInHierarchy;
    }

    public GameObject ObterCarroPosicao1()
    {
        if (carrosNaFila.Count == 0)
            return null;
        return carrosNaFila[0].carro;
    }
}
