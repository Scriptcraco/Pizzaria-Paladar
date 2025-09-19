using UnityEngine;
using UnityEngine.UI;

public class TrocaCorUITrigger : MonoBehaviour
{
    [Header("Imagem que vai trocar de cor (arraste aqui a Image da UI)")]
    [SerializeField] private Image imagemUI;

    [Header("Cor quando o Player entrar no trigger")]
    [SerializeField] private Color corAoEntrar = Color.green;

    [Header("Cor quando o Player sair do trigger")]
    [SerializeField] private Color corOriginal = Color.white;

    private void Reset()
    {
        // Preenche automaticamente caso o script seja colocado junto de uma Image
        if (imagemUI == null)
            imagemUI = GetComponent<Image>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && imagemUI != null)
        {
            imagemUI.color = corAoEntrar;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && imagemUI != null)
        {
            imagemUI.color = corOriginal;
        }
    }
}
