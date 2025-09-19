using UnityEngine;
using UnityEngine.AI;

public class ClientSpawner : MonoBehaviour
{

    [Header("Prefabs dos Clientes")]
    [SerializeField] private Client[] clientesPrefabs;
    [Header("Pooling")]
    [SerializeField] private int poolSize = 20;
    private ObjectPool<Client>[] clientPools;

    [Header("Configuração de Spawn")]
    [SerializeField] private float intervaloSpawn = 3f;
    [SerializeField] private bool spawnAutomatico = false;
    [SerializeField] private float maxDistanciaNavMesh = 2f;

    [Header("Referência da Fila")]
    [SerializeField] private FilaCaixaManager filaCaixaManager;



    private void Awake()
    {
        if (PoolHub.Instance == null)
        {
            new GameObject("PoolHub").AddComponent<PoolHub>();
        }
        clientPools = new ObjectPool<Client>[clientesPrefabs.Length];
        for (int i = 0; i < clientesPrefabs.Length; i++)
        {
            var prefab = clientesPrefabs[i];
            clientPools[i] = new ObjectPool<Client>(prefab, poolSize, null);
            string key = $"client/{i}";
            PoolHub.Instance.Register(key, clientPools[i]);
        }
    }

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
            DevLog.Warn("[Spawner] Nenhum prefab de cliente atribuído.");
            return;
        }

        int index = Random.Range(0, clientesPrefabs.Length);
        var pool = clientPools[index];
        Vector3 pos = transform.position;
        NavMeshHit hit;

        if (!NavMesh.SamplePosition(pos, out hit, maxDistanciaNavMesh, NavMesh.AllAreas))
        {
            DevLog.Error("[Spawner] Posição de spawn fora da NavMesh.");
            return;
        }

        Client novoCliente = pool.Get();
        novoCliente.transform.position = hit.position;
        novoCliente.transform.rotation = Quaternion.identity;
        var pooled = novoCliente.GetComponent<PooledObject>();
        if (pooled == null) pooled = novoCliente.gameObject.AddComponent<PooledObject>();
        // Discover key by finding pool index
        for (int i = 0; i < clientPools.Length; i++)
        {
            if (clientPools[i] == pool)
            {
                pooled.SetKey($"client/{i}");
                break;
            }
        }

        if (filaCaixaManager != null)
        {
            bool entrou = filaCaixaManager.TentarEntrarNaFila(novoCliente.gameObject);
            if (!entrou)
            {
                pool.Release(novoCliente);
                DevLog.Info("[Spawner] Fila cheia. Cliente devolvido ao pool.");
            }
        }
        else
        {
            pool.Release(novoCliente);
        }
    }
}
