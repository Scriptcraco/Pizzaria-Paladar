using UnityEngine;
using System.Collections.Generic;

public class Caixa : MonoBehaviour
{
    public int estoqueBebida = 2000;
    public int capacidadeBebida = 2000; // Exemplo de capacidade, ajuste conforme necessário
    public int capacidadeItem = 2000; // Exemplo de capacidade para outros itens, ajuste conforme necessário     
    public int estoqueItem = 2000;

    public int nivelDocaixa = 1;


    public void AdicionarBebida(int quantidade)
    {
        estoqueBebida = Mathf.Min(estoqueBebida + quantidade, capacidadeBebida);
        Debug.Log($"Player: bebida adicionada. Estoque atual = {estoqueBebida}");
    }

    public void AdicionarProduto(int quantidade)
    {
        estoqueItem = Mathf.Min(estoqueItem + quantidade, capacidadeItem);
        Debug.Log($"Player: produto adicionado. Estoque atual = {estoqueItem}");
    }
    public void RemoverBebida(int quantidade)
    {
        estoqueBebida = Mathf.Max(0, estoqueBebida - quantidade);
        Debug.Log($"Player: bebida removida. Estoque atual = {estoqueBebida}");
    }

    public void RemoverItem(int quantidade)
    {
        estoqueItem = Mathf.Max(0, estoqueItem - quantidade);
        Debug.Log($"Player: item removido. Estoque atual = {estoqueItem}");
    }

    public int GetestoqueBebida() => estoqueBebida;
    public int GetCapacidadeBebida() => capacidadeBebida;
    public float GetestoqueItem() => estoqueItem;

    public float GetCapacidadeItem() => capacidadeItem;
    public int GetNivelDoCaixa() => nivelDocaixa;

}