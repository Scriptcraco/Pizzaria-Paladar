using UnityEngine;

public abstract class EquipmentBase : MonoBehaviour, IUpgradable
{
    [SerializeField] private int level = 1;
    public int Level => level;

    [SerializeField] protected int capacity = 10;
    public int Capacity => capacity;

    public virtual void Upgrade()
    {
        level++;
        capacity = Mathf.RoundToInt(capacity * 1.2f);
    }
}
