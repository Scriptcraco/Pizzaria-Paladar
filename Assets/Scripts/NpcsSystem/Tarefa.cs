using UnityEngine;

public class Tarefa
{
    public TipoTarefa tipo;
    public EstadoTarefa estado;
    public int quantidade;
    public Transform origem;
    public Transform destino;
    public bool exclusiva;

    public Tarefa(TipoTarefa tipo, int quantidade, Transform origem, Transform destino, bool exclusiva = false)
    {
        this.tipo = tipo;
        this.quantidade = quantidade;
        this.origem = origem;
        this.destino = destino;
        this.exclusiva = exclusiva;
        this.estado = EstadoTarefa.IdaParaOrigem;
    }

    public virtual bool ExecutarAcaoOrigem(int quantidadeParaPegar) { return true; }
    public virtual bool ExecutarAcaoDestino(int quantidadeParaEntregar) { return true; }
}
