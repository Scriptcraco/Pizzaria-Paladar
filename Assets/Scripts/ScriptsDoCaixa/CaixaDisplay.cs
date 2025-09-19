using UnityEngine;
using TMPro;

public class CaixaDisplay : MonoBehaviour
{

    [SerializeField] private Caixa caixa;
    [SerializeField] private TextMeshProUGUI textoUI2;
    [SerializeField] private TextMeshProUGUI textoUI3;
    


    private void Update()
    {
        if (caixa != null && textoUI2 != null)
        {
            textoUI2.text = $"Bebidas: {caixa.GetestoqueBebida()}/{caixa.GetCapacidadeBebida()}";
            textoUI3.text = $"Itens: {caixa.GetestoqueItem()}/{caixa.GetCapacidadeItem()}";


        }
    }
}
