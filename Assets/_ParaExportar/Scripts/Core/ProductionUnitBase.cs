using UnityEngine;

public abstract class ProductionUnitBase : EquipmentBase
{
    [SerializeField] private int stock;
    [SerializeField] protected float productionPerSecond = 1f;

    private float _accum;

    public int Stock => stock;

    protected virtual void Update()
    {
        Produce(Time.deltaTime);
    }

    protected virtual void Produce(float dt)
    {
        if (stock >= Capacity) return;
        _accum += dt * productionPerSecond;
        if (_accum >= 1f)
        {
            int units = Mathf.FloorToInt(_accum);
            _accum -= units;
            stock = Mathf.Min(stock + units, Capacity);
        }
    }

    public virtual int Take(int amount)
    {
        int taken = Mathf.Clamp(amount, 0, stock);
        stock -= taken;
        return taken;
    }

    public virtual void AddExternal(int amount)
    {
        stock = Mathf.Min(stock + Mathf.Max(amount, 0), Capacity);
    }
}
