using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    void OnSpawned();
    void OnDespawned();
}

public class GenericPool<T> where T : MonoBehaviour, IPoolable
{
    private readonly T prefab;
    private readonly Transform parent;
    private readonly Queue<T> pool = new();

    public GenericPool(T prefab, int initialSize = 10, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;

        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Object.Instantiate(prefab.gameObject, parent);
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj.GetComponent<T>());
        }
    }

    public T Spawn(Vector3 position, Quaternion rotation)
    {
        T obj = pool.Count > 0 ? pool.Dequeue() : Object.Instantiate(prefab, parent);
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.gameObject.SetActive(true);
        return obj;
    }

    public void Despawn(T obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(parent);
        pool.Enqueue(obj);
    }
}