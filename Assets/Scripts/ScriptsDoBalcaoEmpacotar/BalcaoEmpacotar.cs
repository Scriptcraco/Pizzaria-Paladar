using UnityEngine;
using TMPro;

public class BalcaoEmpacotar : MonoBehaviour
{
    [Header("Configuração de Estoque")]
    [SerializeField] public int capacidadeProduto = 10;
    [SerializeField] public int capacidadeEmpacotado = 10;

    public int estoqueProduto = 0;
    public int estoqueEmpacotado = 0;

    public int nivelDoBalcao = 1;

    [SerializeField] private TextMeshProUGUI saldoUI;
    [SerializeField] private TextMeshProUGUI saldoUI2;

    void Update()
    {
        saldoUI.text = $"Produtos: {estoqueProduto}/{capacidadeProduto}";
        saldoUI2.text = $"Empacotados: {estoqueEmpacotado}/{capacidadeEmpacotado}";
    }


    // PRODUTO PRONTO
    public bool PodeAdicionarProduto(int quantidade) => estoqueProduto + quantidade <= capacidadeProduto;
    public void AdicionarProduto(int quantidade)
    {
        if (PodeAdicionarProduto(quantidade))
            estoqueProduto += quantidade;
    }

    public bool PodeEmpacotar(int quantidade) => estoqueProduto > 0 && estoqueEmpacotado < capacidadeEmpacotado;
    public void EmpacotarProduto(int quantidade)
    {
        if (PodeEmpacotar(quantidade))
        {
            estoqueProduto -= quantidade;
            estoqueEmpacotado += quantidade;
        }
    }

    // RETIRADA
    public bool PodeRetirarEmpacotado(int quantidade) => estoqueEmpacotado >= quantidade;
    public void RetirarEmpacotado(int quantidade)
    {
        if (PodeRetirarEmpacotado(quantidade))
            estoqueEmpacotado -= quantidade;
    }

    // Acesso externo para debug ou UI
    public int GetEstoqueProduto() => estoqueProduto;
    public int GetEstoqueEmpacotado() => estoqueEmpacotado;
    public int GetCapacidadeProduto() => capacidadeProduto;
    public int GetCapacidadeEmpacotado() => capacidadeEmpacotado;

    public int GetNivelDoBalcao() => nivelDoBalcao;
}
