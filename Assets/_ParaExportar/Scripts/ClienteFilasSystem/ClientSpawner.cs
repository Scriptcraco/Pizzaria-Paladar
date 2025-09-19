using UnityEngine;
using UnityEngine.AI;

public class ClientSpawner : MonoBehaviour
{
    [Header("Prefabs dos Clientes")]
    [SerializeField] private GameObject[] clientesPrefabs;

    [Header("Configuração de Spawn")]
    [SerializeField] private float intervaloSpawn = 3f;
    [SerializeField] private bool spawnAutomatico = false;
    [SerializeField] private float maxDistanciaNavMesh = 2f;

    [Header("Referência da Fila")]
    [SerializeField] private FilaCaixaManager filaCaixaManager;

    private void Start()
    {
        if (spawnAutomatico)
            InvokeRepeating(nameof(TentarSpawnarCliente), 0f, intervaloSpawn);
    }

    public void SpawnManual()
    {
        TentarSpawnarCliente();
    }

    public void TentarSpawnarCliente()
    {
        if (clientesPrefabs.Length == 0)
        {
            Debug.LogWarning("[Spawner] Nenhum prefab de cliente atribuído.");
            return;
        }

        int index = Random.Range(0, clientesPrefabs.Length);
        GameObject prefabSelecionado = clientesPrefabs[index];

        Vector3 pos = transform.position;
        NavMeshHit hit;

        if (!NavMesh.SamplePosition(pos, out hit, maxDistanciaNavMesh, NavMesh.AllAreas))
        {
            Debug.LogError("[Spawner] Posição de spawn fora da NavMesh.");
            return;
        }

        GameObject novoCliente = Instantiate(prefabSelecionado, hit.position, Quaternion.identity);

        if (filaCaixaManager != null)
        {
            bool entrou = filaCaixaManager.TentarEntrarNaFila(novoCliente);

            if (!entrou)
            {
                Destroy(novoCliente);
                Debug.Log("[Spawner] Fila cheia. Cliente destruído.");
            }
        }
        else
        {
            Debug.LogError("[Spawner] FilaCaixaManager não atribuído no spawner.");
            Destroy(novoCliente);
        }
    }
}
