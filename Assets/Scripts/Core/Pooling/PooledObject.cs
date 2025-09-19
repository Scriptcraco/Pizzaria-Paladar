using UnityEngine;

// Attach to pooled prefabs to make it easy to return to pool by key
public class PooledObject : MonoBehaviour
{
    [SerializeField] private string poolKey;

    public string PoolKey => poolKey;

    public void SetKey(string key) => poolKey = key;

    public void ReturnToPool()
    {
        if (!string.IsNullOrEmpty(poolKey))
        {
            PoolHub.Instance?.Release(poolKey, this);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
