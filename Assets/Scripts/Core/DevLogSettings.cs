using UnityEngine;

// Drop this on any GameObject to control DevLog globally via Inspector
public class DevLogSettings : MonoBehaviour
{
    [Tooltip("Habilita logs informativos/aviso (erros sempre aparecem)")] public bool enableLogs = false;

    private void OnEnable()
    {
        DevLog.Enabled = enableLogs;
    }

    private void OnValidate()
    {
        DevLog.Enabled = enableLogs;
    }
}
