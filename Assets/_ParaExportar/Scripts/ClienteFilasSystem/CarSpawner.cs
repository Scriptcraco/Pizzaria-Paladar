using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class CarSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject[] prefabsCarro;

    [Header("Configuração")]
    [SerializeField] private float intervaloSpawn = 5f;
    [SerializeField] private float maxDistanciaNavMesh = 2f;
    [SerializeField] private bool spawnAutomatico = true;

    [Header("Referência à fila")]
    public FilaDriveThruManager filaDrive;

    private List<GameObject> carrosNaFila = new List<GameObject>();
    private Coroutine spawnCoroutine;

    private void Start()
    {
        Debug.Log("[CarSpawner] Start iniciado.");
        if (spawnAutomatico)
        {
            spawnCoroutine = StartCoroutine(SpawnLoop());
        }
    }

    private IEnumerator SpawnLoop()
    {
        Debug.Log("[CarSpawner] SpawnLoop iniciado.");
        while (true)
        {
            if (filaDrive == null)
            {
                Debug.LogError("[CarSpawner] filaDrive não atribuída. Parando SpawnLoop.");
                yield break;
            }

            if (carrosNaFila.Count < filaDrive.CapacidadeMaxima())
            {
                Debug.Log("[CarSpawner] Espaço disponível na fila, tentando spawnar carro.");
                TentarSpawnarCarro();
            }
            else
            {
                Debug.Log("[CarSpawner] Fila cheia, aguardando espaço.");
            }

            yield return new WaitForSeconds(intervaloSpawn);
        }
    }

    public void TentarSpawnarCarro()
    {
        Debug.Log("[CarSpawner] TentarSpawnarCarro chamado.");

        if (prefabsCarro.Length == 0)
        {
            Debug.LogWarning("[CarSpawner] Nenhum prefab de carro atribuído.");
            return;
        }

        if (filaDrive == null)
        {
            Debug.LogError("[CarSpawner] Referência a filaDrive não atribuída.");
            return;
        }

        if (carrosNaFila.Count >= filaDrive.CapacidadeMaxima())
        {
            Debug.Log("[CarSpawner] Fila cheia, abortando spawn.");
            return;
        }

        Vector3 pos = transform.position;
        if (!NavMesh.SamplePosition(pos, out NavMeshHit hit, maxDistanciaNavMesh, NavMesh.AllAreas))
        {
            Debug.LogWarning("[CarSpawner] Posição de spawn fora da NavMesh.");
            return;
        }

        int index = Random.Range(0, prefabsCarro.Length);
        GameObject novoCarro = Instantiate(prefabsCarro[index], hit.position, Quaternion.identity);
        Debug.Log($"[CarSpawner] Novo carro instanciado: {novoCarro.name}");

        CarroCliente cliente = novoCarro.GetComponent<CarroCliente>();
        if (cliente == null)
        {
            Debug.LogError("[CarSpawner] Prefab não possui CarroCliente, destruindo objeto.");
            Destroy(novoCarro);
            return;
        }

        carrosNaFila.Add(novoCarro);
        Debug.Log($"[CarSpawner] Carro adicionado a carrosNaFila. Total agora: {carrosNaFila.Count}");

        if (!filaDrive.TentarEntrarNaFila(novoCarro))
        {
            Debug.LogWarning("[CarSpawner] Fila cheia segundo filaDrive, destruindo carro spawnado.");
            carrosNaFila.Remove(novoCarro);
            Destroy(novoCarro);
            return;
        }

        cliente.MoverPara(filaDrive.ObterPosicaoFila(carrosNaFila.Count - 1));
        Debug.Log($"[CarSpawner] Carro {novoCarro.name} movido para posição na fila {carrosNaFila.Count - 1}.");
    }

    public GameObject ObterCarroFrente()
    {
        Debug.Log($"[CarSpawner] ObterCarroFrente chamado. Total carros na fila: {carrosNaFila.Count}");
        return carrosNaFila.Count > 0 ? carrosNaFila[0] : null;
    }

    /// <summary>
    /// Remove o carro da fila e atualiza ambas as listas (spawner e filaDrive).
    /// Deve ser chamado pelo NPC atendente após o carro ser atendido e pronto para sair.
    /// </summary>
    public void SairDaFila(GameObject carro)
    {
        Debug.Log($"[CarSpawner] SairDaFila chamado para carro: {carro.name}");

        int index = carrosNaFila.IndexOf(carro);
        if (index != -1)
        {
            carrosNaFila.RemoveAt(index);
            Debug.Log($"[CarSpawner] Carro removido de carrosNaFila. Total agora: {carrosNaFila.Count}");

            filaDrive.SairDaFila(carro);
            Debug.Log($"[CarSpawner] Carro removido da filaDrive.");

            AvancarFila();
        }
        else
        {
            Debug.LogWarning($"[CarSpawner] Tentativa de remover carro que não está na lista: {carro.name}");
        }
    }

    public void AvancarFila()
    {
        Debug.Log("[CarSpawner] AvancarFila chamado.");

        for (int i = 0; i < carrosNaFila.Count; i++)
        {
            CarroCliente cliente = carrosNaFila[i].GetComponent<CarroCliente>();
            if (cliente != null)
            {
                Debug.Log($"[CarSpawner] Movendo carro {carrosNaFila[i].name} para posição {i}.");
                cliente.MoverPara(filaDrive.ObterPosicaoFila(i));
            }
            else
            {
                Debug.LogWarning($"[CarSpawner] Carro {carrosNaFila[i].name} não possui componente CarroCliente.");
            }
        }
    }
}
