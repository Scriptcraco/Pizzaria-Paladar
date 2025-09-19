using UnityEngine;
using System.Collections.Generic;


public class CaixaDriveTaskGenerator : MonoBehaviour
{
    public CaixaDoDrive caixaDrive;

    public List<Tarefa> GerarTarefas()
    {
        List<Tarefa> tarefas = new List<Tarefa>();

        
            tarefas.Add(new TarefaAtenderCaixaDrive(caixaDrive));
        

        return tarefas;
    }
}