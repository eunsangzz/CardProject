using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private readonly Dictionary<GameObject, Queue<GameObject>> _pools = new();
    private readonly Dictionary<GameObject, GameObject> _instanceToPrefab = new();

    public GameObject Get(GameObject prefab, Transform parent = null)
    {
        if(!_pools.TryGetValue(prefab, out var q))
        {
            q = new Queue<GameObject>();
            _pools[prefab] = q;
        }

        GameObject obj;
        if (q.Count > 0)
        {
            obj = q.Dequeue();
        }
        else
        {
            obj = Instantiate(prefab);
            _instanceToPrefab[obj] = prefab;
        }

        if (parent) obj.transform.SetParent(parent, false);
        obj.SetActive(true);

        if (obj.TryGetComponent<IPoolable>(out var poolable))
            poolable.OnSpawned();

        return obj;
    }

    public void Release(GameObject obj)
    {
        if (obj == null) return;

        if (obj.TryGetComponent<IPoolable>(out var poolable))
            poolable.OnDespawned();

        obj.SetActive(false);
        obj.transform.SetParent(transform, false);

        if (_instanceToPrefab.TryGetValue(obj, out var prefab))
        {
            _pools[prefab].Enqueue(obj);
        }
        else { }
    }
}

public interface IPoolable
{
    void OnSpawned();
    void OnDespawned();
}
