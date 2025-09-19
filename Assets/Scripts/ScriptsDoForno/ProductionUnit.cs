using UnityEngine;

public class ProductionUnit : MonoBehaviour
{
    [Header("Configuração de Produção")]
    public float velocidadeProducao = 0.1f; // unidades por segundo
    public int estoqueMaximo = 10;

    public int nivelDoForno = 1;

    [Header("Estado Atual")]
    [SerializeField] private int estoqueAtual = 0;

    private float timer;

    private void Update()
    {
        Produzir(Time.deltaTime);
    }

    private void Produzir(float deltaTime)
    {
        if (estoqueAtual >= estoqueMaximo) return;

        timer += deltaTime;

        if (timer >= 1f / velocidadeProducao)
        {
            estoqueAtual++;
            timer = 0f;
            Debug.Log($"Produzido! Estoque atual: {estoqueAtual}");
        }
    }

    // Acesso externo seguro
    public int GetEstoqueAtual() => estoqueAtual;
    public int GetEstoqueMaximo() => estoqueMaximo;
    public float GetVelocidadeProducao() => velocidadeProducao;

    public int GetNivelDoForno() => nivelDoForno;

    // Métodos úteis para interação externa
    public bool ConsumirProduto()
    {
        if (estoqueAtual <= 0) return false;

        estoqueAtual--;
        Debug.Log("Produto consumido!");
        return true;
    }

    public void ResetarEstoque()
    {
        estoqueAtual = 0;
        Debug.Log("Estoque resetado.");
    }
    public void ReduzirEstoque(int quantidade)
{
    estoqueAtual = Mathf.Max(0, estoqueAtual - quantidade);
    Debug.Log($"Forno: estoque reduzido. Atual: {estoqueAtual}");
}
}
