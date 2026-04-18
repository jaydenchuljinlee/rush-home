using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 제네릭 오브젝트 풀. MonoBehaviour 기반 컴포넌트를 재사용한다.
/// IPoolable을 구현한 컴포넌트에 OnSpawnFromPool / OnReturnToPool 콜백을 자동 호출한다.
/// </summary>
public class ObjectPool<T> where T : Component
{
    private readonly T _prefab;
    private readonly Transform _parent;
    private readonly Stack<T> _pool = new Stack<T>();
    private readonly int _maxSize;

    public int CountInactive => _pool.Count;

    public ObjectPool(T prefab, int initialSize, int maxSize, Transform parent = null)
    {
        _prefab = prefab;
        _parent = parent;
        _maxSize = maxSize;

        for (int i = 0; i < initialSize; i++)
        {
            T instance = CreateInstance();
            instance.gameObject.SetActive(false);
            _pool.Push(instance);
        }
    }

    /// <summary>풀에서 오브젝트를 꺼낸다. 풀이 비어 있으면 새로 생성한다.</summary>
    public T Get()
    {
        T instance = _pool.Count > 0 ? _pool.Pop() : CreateInstance();
        instance.gameObject.SetActive(true);

        if (instance is IPoolable poolable)
            poolable.OnSpawnFromPool();

        return instance;
    }

    /// <summary>오브젝트를 풀로 반환한다. maxSize 초과 시 파괴한다.</summary>
    public void Return(T instance)
    {
        if (instance == null) return;

        if (instance is IPoolable poolable)
            poolable.OnReturnToPool();

        if (_pool.Count >= _maxSize)
        {
            UnityEngine.Object.Destroy(instance.gameObject);
            return;
        }

        instance.gameObject.SetActive(false);
        _pool.Push(instance);
    }

    private T CreateInstance()
    {
        GameObject go = UnityEngine.Object.Instantiate(_prefab.gameObject, _parent);
        return go.GetComponent<T>();
    }
}
