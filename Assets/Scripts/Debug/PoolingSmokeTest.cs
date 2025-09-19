using System.Collections;
using UnityEngine;

public class PoolingSmokeTest : MonoBehaviour
{
    [Header("Opcional: Teste de Clientes")]
    public ClientSpawner clientSpawner;
    public int clientesParaSpawnar = 20;
    public float intervaloEntreClientes = 0.2f;

    [Header("Opcional: Teste de Carros")] 
    public CarSpawner carSpawner;
    public int carrosParaSpawnar = 10;
    public float intervaloEntreCarros = 0.4f;

    [Header("Execução")] public bool autoRun = false;

    private void Start()
    {
        if (autoRun)
        {
            if (clientSpawner != null) StartCoroutine(SpawnClientes());
            if (carSpawner != null) StartCoroutine(SpawnCarros());
        }
    }

    private IEnumerator SpawnClientes()
    {
        for (int i = 0; i < clientesParaSpawnar; i++)
        {
            clientSpawner.SpawnManual();
            yield return new WaitForSeconds(intervaloEntreClientes);
        }
    }

    private IEnumerator SpawnCarros()
    {
        for (int i = 0; i < carrosParaSpawnar; i++)
        {
            carSpawner.TentarSpawnarCarro();
            yield return new WaitForSeconds(intervaloEntreCarros);
        }
    }
}
