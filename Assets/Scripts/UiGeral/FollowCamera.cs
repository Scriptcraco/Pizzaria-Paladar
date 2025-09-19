using UnityEngine;
using TMPro;

public class FollowCamera : MonoBehaviour
{
    [Header("Posicionamento")]
    [SerializeField] private Transform targetToFollow; // Player
    [SerializeField] private Vector3 offset = Vector3.up * 2.5f;

    [Header("Componentes")]
    public TextMeshProUGUI textMesh;
    public TextMeshProUGUI textMesh2;

      public TextMeshProUGUI textMesh3;
        public TextMeshProUGUI textMesh4;

    private PlayerController playerController;

    void Start()
    {
        if (textMesh == null)
            textMesh = GetComponentInChildren<TextMeshProUGUI>();

        if (textMesh2 == null)
            textMesh2 = GetComponentsInChildren<TextMeshProUGUI>()[1];
        if (textMesh3 == null)
            textMesh3 = GetComponentsInChildren<TextMeshProUGUI>()[2];
        if (textMesh4 == null)
            textMesh4 = GetComponentsInChildren<TextMeshProUGUI>()[3];
        if (targetToFollow == null)
            targetToFollow = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (targetToFollow != null)
            playerController = targetToFollow.GetComponent<PlayerController>();
    }

    void LateUpdate()
    {
        if (targetToFollow == null || playerController == null || textMesh == null) return;

        // 1. Segue o player com offset (sem girar)
        transform.position = targetToFollow.position + offset;

        // 2. NÃO gira para câmera nem para o player
        // (mantém rotação fixa original do texto)

        // 3. Atualiza o texto com dados do player
        textMesh.text = $"Estoque comida: {playerController.estoqueAtual}/{playerController.estoqueMaximo}";
        textMesh2.text = $"Estoque Bebida: {playerController.estoqueAtualBebida}/{playerController.estoqueMaximoBebida}";
        textMesh3.text = $"Dinheiro: ${playerController.dinheiro}";
        textMesh4.text = $"Estoque Empacotado: {playerController.estoqueAtualEmpacotado}/{playerController.estoqueMaximoEmpacotado}";

    }
}
