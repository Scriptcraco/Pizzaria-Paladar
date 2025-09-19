using UnityEngine;
using TMPro;

public class CaixaDoDriveDisplay : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private CaixaDoDrive caixaDoDrive;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI textoProdutoEmbalado;
    [SerializeField] private TextMeshProUGUI textoBebidaEmbalada;

    private void Update()
    {
        if (caixaDoDrive != null)
        {
            if (textoProdutoEmbalado != null)
            {
                textoProdutoEmbalado.text = $"Itens Embalados: {caixaDoDrive.GetEstoqueProdutoEmbalado()}/{caixaDoDrive.GetCapacidadeProdutoEmbalado()}";
            }

            if (textoBebidaEmbalada != null)
            {
                textoBebidaEmbalada.text = $"Bebidas Embaladas: {caixaDoDrive.GetEstoqueBebidaEmbalada()}/{caixaDoDrive.GetCapacidadeBebidaEmbalada()}";
            }
        }
    }
}
