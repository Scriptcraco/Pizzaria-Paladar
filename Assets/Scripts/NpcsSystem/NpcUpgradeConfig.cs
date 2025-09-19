using UnityEngine;

[CreateAssetMenu(menuName = "Paladar/NPC Upgrade Config", fileName = "NpcUpgradeConfig")]
public class NpcUpgradeConfig : ScriptableObject
{
    [Header("Crescimento por nível")]
    public AnimationCurve speedMultiplierByLevel = AnimationCurve.Linear(1, 1f, 10, 2f);
    public AnimationCurve capacityMultiplierByLevel = AnimationCurve.Linear(1, 1f, 10, 2f);
    public AnimationCurve taskEfficiencyByLevel = AnimationCurve.Linear(1, 1f, 10, 2f);

    public float GetSpeedMult(int level) => speedMultiplierByLevel.Evaluate(level);
    public float GetCapacityMult(int level) => capacityMultiplierByLevel.Evaluate(level);
    public float GetEfficiencyMult(int level) => taskEfficiencyByLevel.Evaluate(level);
}
