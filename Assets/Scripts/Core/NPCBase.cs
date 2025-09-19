using UnityEngine;

public abstract class NPCBase : MonoBehaviour, IUpgradable
{
    [Header("Upgrades")] public NpcUpgradeConfig upgradeConfig;
    public NpcStats stats = new NpcStats();

    public int Level => stats.level;

    protected virtual void Awake()
    {
        stats.Recalculate(upgradeConfig);
    }

    public void Upgrade()
    {
        stats.level++;
        stats.Recalculate(upgradeConfig);
        OnUpgraded();
    }

    protected virtual void OnUpgraded() { }

    public abstract void Tick(float dt);
}
