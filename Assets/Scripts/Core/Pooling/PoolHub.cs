using System.Collections.Generic;
using UnityEngine;

public interface IPool
{
    Component Get();
    void Release(Component item);
}

public class GenericPool<T> : IPool where T : Component
{
    private readonly ObjectPool<T> _pool;
    public GenericPool(ObjectPool<T> pool)
    {
        _pool = pool;
    }

    public Component Get() => _pool.Get();
    public void Release(Component item)
    {
        if (item is T t)
            _pool.Release(t);
        else
            Debug.LogError($"[PoolHub] Tried to release wrong type {item?.GetType().Name} into pool of {typeof(T).Name}");
    }
}

public class PoolHub : MonoBehaviour
{
    public static PoolHub Instance { get; private set; }

    private readonly Dictionary<string, IPool> _pools = new Dictionary<string, IPool>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Register<T>(string key, ObjectPool<T> pool) where T : Component
    {
        _pools[key] = new GenericPool<T>(pool);
    }

    public T Get<T>(string key) where T : Component
    {
        if (_pools.TryGetValue(key, out var pool))
        {
            return pool.Get() as T;
        }
        Debug.LogError($"[PoolHub] Pool not found for key '{key}'");
        return null;
    }

    public void Release(string key, Component item)
    {
        if (_pools.TryGetValue(key, out var pool))
        {
            pool.Release(item);
            return;
        }
        Debug.LogError($"[PoolHub] Pool not found for key '{key}'");
    }
}
