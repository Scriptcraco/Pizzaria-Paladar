using UnityEngine;
using System.Collections.Generic;

public class MesaManager : MonoBehaviour
{
    public static MesaManager Instance;

    [Header("Lista de mesas na ordem de desbloqueio")]
    public List<TriggerCompraMesa> mesasEmOrdem;
    public List<TriggerCompraMesa> todasAsMesas;


    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        for (int i = 0; i < mesasEmOrdem.Count; i++)
        {
            if (i == 0)
                mesasEmOrdem[i].SetDisponivelParaCompra(true); // só a primeira liberada
            else
                mesasEmOrdem[i].SetDisponivelParaCompra(false); // bloqueadas
        }
    }

    public void MesaComprada(TriggerCompraMesa mesa)
    {
        int index = mesasEmOrdem.IndexOf(mesa);
        if (index >= 0 && index < mesasEmOrdem.Count - 1)
        {
            mesasEmOrdem[index + 1].SetDisponivelParaCompra(true); // libera a próxima
        }
    }
public int GetQuantidadeMesasCompradas()
{
    int quantidade = 0;
    foreach (TriggerCompraMesa mesa in todasAsMesas)
    {
        if (mesa != null && mesa.mesaParaAtivar != null && mesa.mesaParaAtivar.activeSelf)
        {
            quantidade++;
        }
    }
    return quantidade;
}

}
