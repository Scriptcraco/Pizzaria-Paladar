using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
    private readonly T _prefab;
    private readonly Transform _parent;
    private readonly Queue<T> _pool = new Queue<T>();

    public ObjectPool(T prefab, int initialSize = 0, Transform parent = null)
    {
        _prefab = prefab;
        _parent = parent;
        for (int i = 0; i < initialSize; i++)
        {
            var inst = GameObject.Instantiate(_prefab, _parent);
            inst.gameObject.SetActive(false);
            _pool.Enqueue(inst);
        }
    }

    public T Get()
    {
        T item = _pool.Count > 0 ? _pool.Dequeue() : GameObject.Instantiate(_prefab, _parent);
        item.gameObject.SetActive(true);
        return item;
    }

    public void Release(T item)
    {
        item.gameObject.SetActive(false);
        _pool.Enqueue(item);
    }
}
