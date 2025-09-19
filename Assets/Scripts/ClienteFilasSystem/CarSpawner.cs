using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class CarSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private CarroCliente[] prefabsCarro;
    [Header("Pooling")]
    [SerializeField] private int poolSize = 10;
    private ObjectPool<CarroCliente>[] carPools;

    [Header("Configuração")]
    [SerializeField] private float intervaloSpawn = 5f;
    [SerializeField] private float maxDistanciaNavMesh = 2f;
    [SerializeField] private bool spawnAutomatico = true;

    [Header("Referência à fila")]
    public FilaDriveThruManager filaDrive;

    private Coroutine spawnCoroutine;

    private void Awake()
    {
        if (PoolHub.Instance == null)
        {
            new GameObject("PoolHub").AddComponent<PoolHub>();
        }
        carPools = new ObjectPool<CarroCliente>[prefabsCarro.Length];
        for (int i = 0; i < prefabsCarro.Length; i++)
        {
            carPools[i] = new ObjectPool<CarroCliente>(prefabsCarro[i], poolSize, null);
            string key = $"car/{i}";
            PoolHub.Instance.Register(key, carPools[i]);
        }
    }

    private void Start()
    {
        DevLog.Info("[CarSpawner] Start iniciado.");
        if (spawnAutomatico)
        {
            spawnCoroutine = StartCoroutine(SpawnLoop());
        }
    }

    private IEnumerator SpawnLoop()
    {
        DevLog.Info("[CarSpawner] SpawnLoop iniciado.");
        while (true)
        {
            if (filaDrive == null)
            {
                DevLog.Error("[CarSpawner] filaDrive não atribuída. Parando SpawnLoop.");
                yield break;
            }

            if (filaDrive.ObterQuantidadeCarros() < filaDrive.CapacidadeMaxima())
            {
                DevLog.Info("[CarSpawner] Espaço disponível na fila, tentando spawnar carro.");
                TentarSpawnarCarro();
            }
            else
            {
                DevLog.Info("[CarSpawner] Fila cheia, aguardando espaço.");
            }

            yield return new WaitForSeconds(intervaloSpawn);
        }
    }

    public void TentarSpawnarCarro()
    {
        DevLog.Info("[CarSpawner] TentarSpawnarCarro chamado.");

        if (prefabsCarro.Length == 0)
        {
            DevLog.Warn("[CarSpawner] Nenhum prefab de carro atribuído.");
            return;
        }

        if (filaDrive == null)
        {
            DevLog.Error("[CarSpawner] Referência a filaDrive não atribuída.");
            return;
        }

        if (filaDrive.ObterQuantidadeCarros() >= filaDrive.CapacidadeMaxima())
        {
            DevLog.Info("[CarSpawner] Fila cheia, abortando spawn.");
            return;
        }

        Vector3 pos = transform.position;
        if (!NavMesh.SamplePosition(pos, out NavMeshHit hit, maxDistanciaNavMesh, NavMesh.AllAreas))
        {
            DevLog.Warn("[CarSpawner] Posição de spawn fora da NavMesh.");
            return;
        }

        int index = Random.Range(0, prefabsCarro.Length);
        var pool = carPools[index];
        CarroCliente carroCliente = pool.Get();
        GameObject novoCarro = carroCliente.gameObject;
        novoCarro.transform.position = hit.position;
        novoCarro.transform.rotation = Quaternion.identity;
    DevLog.Info($"[CarSpawner] Novo carro ativado do pool: {novoCarro.name}");
        var pooled = novoCarro.GetComponent<PooledObject>();
        if (pooled == null) pooled = novoCarro.AddComponent<PooledObject>();
        for (int i = 0; i < carPools.Length; i++)
        {
            if (carPools[i] == pool)
            {
                pooled.SetKey($"car/{i}");
                break;
            }
        }

        if (!filaDrive.TentarEntrarNaFila(novoCarro))
        {
            DevLog.Warn("[CarSpawner] Fila cheia segundo filaDrive, devolvendo carro ao pool.");
            pool.Release(carroCliente);
            return;
        }
        // Não mover novamente: FilaDriveThruManager já posiciona o carro ao entrar na fila.
    }

    public GameObject ObterCarroFrente()
    {
        DevLog.Info("[CarSpawner] ObterCarroFrente chamado.");
        return filaDrive != null ? filaDrive.ObterCarroPosicao1() : null;
    }

    /// <summary>
    /// Remove o carro da fila e atualiza ambas as listas (spawner e filaDrive).
    /// Deve ser chamado pelo NPC atendente após o carro ser atendido e pronto para sair.
    /// </summary>
    public void SairDaFila(GameObject carro)
    {
        DevLog.Info($"[CarSpawner] SairDaFila chamado para carro: {carro.name}");
        if (filaDrive == null)
        {
            DevLog.Warn("[CarSpawner] filaDrive não atribuída em SairDaFila.");
            return;
        }
        filaDrive.SairDaFila(carro);
    }

    public void AvancarFila()
    {
        DevLog.Info("[CarSpawner] AvancarFila chamado.");
        if (filaDrive != null)
            filaDrive.AvancarFila();
    }
}
