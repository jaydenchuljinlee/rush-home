using System.Collections.Generic;
using UnityEngine;

public class ObstaclePool : MonoBehaviour
{
    public static ObstaclePool Instance { get; private set; }

    readonly Dictionary<GameObject, Queue<GameObject>> pools = new();
    readonly HashSet<GameObject> activeObjects = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public GameObject Get(GameObject prefab)
    {
        if (!pools.ContainsKey(prefab))
            pools[prefab] = new Queue<GameObject>();

        GameObject obj;
        if (pools[prefab].Count > 0)
        {
            obj = pools[prefab].Dequeue();
        }
        else
        {
            obj = Instantiate(prefab, transform);
            obj.GetComponent<IPoolable>()?.OnReturnToPool();
        }

        activeObjects.Add(obj);
        obj.GetComponent<IPoolable>()?.OnSpawnFromPool();
        return obj;
    }

    public void Return(GameObject obj)
    {
        if (!activeObjects.Remove(obj)) return;

        obj.GetComponent<IPoolable>()?.OnReturnToPool();

        // prefab 키를 찾기 위해 이름 기반 매칭
        foreach (var kvp in pools)
        {
            if (obj.name.StartsWith(kvp.Key.name))
            {
                kvp.Value.Enqueue(obj);
                return;
            }
        }

        // 매칭 실패 시 새 풀 생성
        var newPool = new Queue<GameObject>();
        newPool.Enqueue(obj);
        // 이름 기반 임시 키로 저장하지 않고 비활성화만 처리
        obj.SetActive(false);
    }

    public void ReturnAll()
    {
        foreach (var obj in new List<GameObject>(activeObjects))
        {
            Return(obj);
        }
    }
}
