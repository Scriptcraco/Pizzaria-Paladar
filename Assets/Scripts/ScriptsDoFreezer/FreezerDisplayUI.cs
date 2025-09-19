using UnityEngine;
using TMPro;

public class FreezerDisplayUI : MonoBehaviour
{
    [SerializeField] private Freezer freezer;
    [SerializeField] private TextMeshProUGUI textoUI;

    private void Update()
    {
        if (freezer != null && textoUI != null)
        {
            textoUI.text = $"Gelados: {freezer.GetQuantidadeCongelada()}/{freezer.GetCapacidadeMaxima()}";
        }
    }
}
