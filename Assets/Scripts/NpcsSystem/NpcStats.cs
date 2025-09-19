using UnityEngine;

[System.Serializable]
public class NpcStats
{
    [Header("Base Stats")] public float baseMoveSpeed = 3.5f;
    public int baseCapacity = 1;

    [Header("Level")] public int level = 1;
    [Header("Dynamic")] public float currentMoveSpeed;
    public int currentCapacity;

    public void Recalculate(NpcUpgradeConfig config)
    {
        float speedMult = config != null ? config.GetSpeedMult(level) : 1f;
        float capacityMult = config != null ? config.GetCapacityMult(level) : 1f;

        currentMoveSpeed = baseMoveSpeed * speedMult;
        currentCapacity = Mathf.Max(1, Mathf.RoundToInt(baseCapacity * capacityMult));
    }
}
