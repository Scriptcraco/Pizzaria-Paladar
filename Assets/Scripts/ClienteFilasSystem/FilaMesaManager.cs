using System.Collections.Generic;
using UnityEngine;

public class FilaMesaManager : MonoBehaviour
{
    private List<Transform> cadeiras = new List<Transform>();
    private Queue<Client> filaDeEspera = new Queue<Client>();
    private HashSet<Transform> cadeirasOcupadas = new HashSet<Transform>();

    private void Update()
    {
        LimparClientesInvalidos();
    }

    public bool TentarReservarCadeiraDisponivel(Client cliente)
    {
        Transform cadeiraLivre = EncontrarCadeiraLivre();
        if (cadeiraLivre != null)
        {
            cadeirasOcupadas.Add(cadeiraLivre);
            cliente.cadeiraOcupada = cadeiraLivre;
            cliente.MoverPara(cadeiraLivre.position);
            cliente.focoOlharMesa = cadeiraLivre.parent;
            return true;
        }
        return false;
    }

    public void EnfileirarClienteParaMesa(Client cliente)
    {
        if (!filaDeEspera.Contains(cliente))
        {
            filaDeEspera.Enqueue(cliente);
        }
    }

    public void LiberarCadeira(Transform cadeira)
    {
        if (cadeirasOcupadas.Contains(cadeira))
        {
            cadeirasOcupadas.Remove(cadeira);
        }

        if (filaDeEspera.Count > 0)
        {
            Client proximo = filaDeEspera.Dequeue();
            cadeirasOcupadas.Add(cadeira);
            proximo.cadeiraOcupada = cadeira;
            proximo.MoverPara(cadeira.position);
            proximo.focoOlharMesa = cadeira.parent;
            proximo.ComecarSequencia();
        }
    }

    private void LimparClientesInvalidos()
    {
        if (filaDeEspera.Count == 0) return;

        int antes = filaDeEspera.Count;
        var novaFila = new Queue<Client>(antes);
        while (filaDeEspera.Count > 0)
        {
            var c = filaDeEspera.Dequeue();
            if (c != null && c.gameObject.activeInHierarchy)
            {
                novaFila.Enqueue(c);
            }
        }
        if (novaFila.Count != antes)
        {
        DevLog.Info($"[FilaMesaManager] Removidos {antes - novaFila.Count} clientes inválidos da fila de espera.");
        }
        filaDeEspera = novaFila;
    }

    private Transform EncontrarCadeiraLivre()
    {
        foreach (Transform cadeira in cadeiras)
        {
            if (!cadeirasOcupadas.Contains(cadeira)
                && cadeira.gameObject.activeInHierarchy
                && cadeira.parent.gameObject.activeInHierarchy)
            {
                return cadeira;
            }
        }
        return null;
    }

    // Adiciona cadeiras manualmente, usado no TriggerCompraMesa
    public void AdicionarCadeiras(List<Transform> novasCadeiras)
    {
    DevLog.Info($"Adicionando {novasCadeiras.Count} cadeiras ao gerenciador");
        cadeiras.AddRange(novasCadeiras);
    }

    // Atualiza a lista de cadeiras para conter somente as cadeiras das mesas ativas na cena
    public void AtualizarCadeirasAtivas()
    {
        cadeiras.Clear();

        // Supondo que todas as mesas ativas têm tag "Mesa"
        GameObject[] mesasAtivas = GameObject.FindGameObjectsWithTag("Mesa");

        foreach (GameObject mesa in mesasAtivas)
        {
            if (!mesa.activeInHierarchy)
                continue;

            foreach (Transform child in mesa.transform)
            {
                if (child.CompareTag("Cadeira"))
                {
                    cadeiras.Add(child);
                }
            }
        }
    }

    // Método para debug das cadeiras no gerenciador
    public void LogCadeiras()
    {
        DevLog.Info($"Total cadeiras no gerenciador: {cadeiras.Count}");
        foreach (var c in cadeiras)
            DevLog.Info($"Cadeira: {c.name}, Ativa? {c.gameObject.activeInHierarchy}");
    }
}
