using UnityEngine;

public class JustFollow : MonoBehaviour
{
    [Header("Posicionamento")]
    [SerializeField] private Transform targetToFollow; // Player
    [SerializeField] private Vector3 offset = Vector3.up * 2.5f;

   
    private PlayerController playerController;

    void Start()
    {
      

        if (targetToFollow == null)
            targetToFollow = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (targetToFollow != null)
            playerController = targetToFollow.GetComponent<PlayerController>();
    }

    void LateUpdate()
    {
        if (targetToFollow == null || playerController == null ) return;

        // 1. Segue o player com offset (sem girar)
        transform.position = targetToFollow.position + offset;

        // 2. NÃO gira para câmera nem para o player
        // (mantém rotação fixa original do texto)

        // 3. Atualiza o texto com dados do player
    }
}