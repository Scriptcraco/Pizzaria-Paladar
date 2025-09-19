using UnityEngine;

// Simple debug logger with a global flag to reduce log spam under high load
public static class DevLog
{
    // Set to true to enable info/warn logs; errors always log
    public static bool Enabled = false;

    public static void Info(string message)
    {
        if (Enabled) Debug.Log(message);
    }

    public static void Warn(string message)
    {
        if (Enabled) Debug.LogWarning(message);
    }

    public static void Error(string message)
    {
        // Errors are important even when disabled
        Debug.LogError(message);
    }
}
