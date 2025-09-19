using UnityEngine;

public class CaixaDoDrive : MonoBehaviour
{
    [Header("Estoque de Produtos Embalados")]
    [SerializeField] public int estoqueProdutoEmbalado = 2000;
    [SerializeField] public int capacidadeProdutoEmbalado = 2000;

    [Header("Estoque de Bebidas Embaladas")]
    [SerializeField] public int estoqueBebidaEmbalada = 2000;
    [SerializeField] public int capacidadeBebidaEmbalada = 2000;

    public int nivelDocaixaDrive = 1;

    // Métodos públicos para adicionar/remover produto embalado
    public void AdicionarProdutoEmbalado(int quantidade)
    {
        estoqueProdutoEmbalado = Mathf.Min(estoqueProdutoEmbalado + quantidade, capacidadeProdutoEmbalado);
        Debug.Log($"📦 Produto embalado adicionado. Estoque atual = {estoqueProdutoEmbalado}");
    }

    public void RemoverProdutoEmbalado(int quantidade)
    {
        estoqueProdutoEmbalado = Mathf.Max(0, estoqueProdutoEmbalado - quantidade);
        Debug.Log($"📦 Produto embalado removido. Estoque atual = {estoqueProdutoEmbalado}");
    }

    // Métodos públicos para adicionar/remover bebida embalada
    public void AdicionarBebidaEmbalada(int quantidade)
    {
        estoqueBebidaEmbalada = Mathf.Min(estoqueBebidaEmbalada + quantidade, capacidadeBebidaEmbalada);
        Debug.Log($"🥤 Bebida embalada adicionada. Estoque atual = {estoqueBebidaEmbalada}");
    }

    public void RemoverBebidaEmbalada(int quantidade)
    {
        estoqueBebidaEmbalada = Mathf.Max(0, estoqueBebidaEmbalada - quantidade);
        Debug.Log($"🥤 Bebida embalada removida. Estoque atual = {estoqueBebidaEmbalada}");
    }

    // Getters
    public int GetEstoqueProdutoEmbalado() => estoqueProdutoEmbalado;
    public int GetCapacidadeProdutoEmbalado() => capacidadeProdutoEmbalado;

    public int GetEstoqueBebidaEmbalada() => estoqueBebidaEmbalada;
    public int GetCapacidadeBebidaEmbalada() => capacidadeBebidaEmbalada;

    public int GetNivelDoCaixaDrive() => nivelDocaixaDrive;
}
