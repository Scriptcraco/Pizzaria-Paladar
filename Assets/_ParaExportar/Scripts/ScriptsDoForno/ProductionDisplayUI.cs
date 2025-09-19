using UnityEngine;
using UnityEngine.UI;
using TMPro; // Certifique-se de ter o TextMeshPro instalado
public class ProductionDisplayUI : MonoBehaviour
{
    [SerializeField] private ProductionUnit producao;
    [SerializeField] public TextMeshProUGUI textoUI; // Pode ser TMP_Text se usar TextMeshPro

    private void Update()
    {
        if (producao != null && textoUI != null)
        {
            textoUI.text = $"Estoque: {producao.GetEstoqueAtual()}/{producao.GetEstoqueMaximo()}";
        }
    }
}
