using UnityEngine;

public class Freezer : MonoBehaviour
{
    [Header("Configuração de Congelamento")]
    public float velocidadeCongelamento = 0.5f; // unidades por segundo
    public int capacidadeFreezer = 10;

    [Header("Estado Atual")]
    [SerializeField] private int quantidadeCongelada = 0;

    private float timer;

    public int nivelDoFreezer = 1;

    private void Update()
    {
        Congelar(Time.deltaTime);
    }

    private void Congelar(float deltaTime)
    {
        if (quantidadeCongelada >= capacidadeFreezer) return;

        timer += deltaTime;

        if (timer >= 1f / velocidadeCongelamento)
        {
            quantidadeCongelada++;
            timer = 0f;
            Debug.Log($"Freezer: item congelado! Total congelado: {quantidadeCongelada}");
        }
    }

    // Acesso externo seguro
    public int GetQuantidadeCongelada() => quantidadeCongelada;
    public int GetCapacidadeMaxima() => capacidadeFreezer;
    public float GetVelocidadeCongelamento() => velocidadeCongelamento;

    public int GetNivelDoFreezer() => nivelDoFreezer;
    // Consome item congelado (1 unidade)
    public bool ConsumirItemCongelado()
    {
        if (quantidadeCongelada <= 0) return false;

        quantidadeCongelada--;
        Debug.Log("Freezer: item consumido (descongelado).");
        return true;
    }

    // Limpa todo o conteúdo
    public void ResetarFreezer()
    {
        quantidadeCongelada = 0;
        Debug.Log("Freezer: resetado.");
    }
    public void ReduzirQuantidade(int quantidade)
{
    quantidadeCongelada = Mathf.Max(0, quantidadeCongelada - quantidade);
    Debug.Log($"Freezer: quantidade reduzida. Atual: {quantidadeCongelada}");
}

 
}
