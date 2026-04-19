using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 장애물 오브젝트 풀링 시스템.
/// 프리팹 단위로 Queue를 관리하여 Instantiate/Destroy 비용을 줄인다.
/// </summary>
public class ObstaclePool : MonoBehaviour
{
    [SerializeField] private int initialPoolSizePerPrefab = 3;

    // 프리팹 GameObject를 키로 Queue를 관리
    private readonly Dictionary<GameObject, Queue<Obstacle>> _pools = new Dictionary<GameObject, Queue<Obstacle>>();
    // 인스턴스 -> 원본 프리팹 매핑 (반환 시 어떤 풀로 돌아갈지 결정)
    private readonly Dictionary<Obstacle, GameObject> _prefabLookup = new Dictionary<Obstacle, GameObject>();

    /// <summary>
    /// 풀에서 장애물을 꺼낸다. 풀이 비어 있으면 신규 생성.
    /// </summary>
    public Obstacle Get(GameObject prefab, Vector3 position)
    {
        if (!_pools.ContainsKey(prefab))
        {
            _pools[prefab] = new Queue<Obstacle>();
        }

        Obstacle obstacle;
        Queue<Obstacle> pool = _pools[prefab];

        if (pool.Count > 0)
        {
            obstacle = pool.Dequeue();
            obstacle.transform.position = position;
        }
        else
        {
            obstacle = CreateNew(prefab, position);
        }

        obstacle.OnSpawnFromPool();
        return obstacle;
    }

    /// <summary>
    /// 장애물을 풀에 반환한다.
    /// </summary>
    public void Return(Obstacle obstacle)
    {
        obstacle.OnReturnToPool();

        if (_prefabLookup.TryGetValue(obstacle, out GameObject prefab))
        {
            if (!_pools.ContainsKey(prefab))
            {
                _pools[prefab] = new Queue<Obstacle>();
            }
            _pools[prefab].Enqueue(obstacle);
        }
        else
        {
            // 풀 정보가 없으면 제거
            Destroy(obstacle.gameObject);
        }
    }

    private Obstacle CreateNew(GameObject prefab, Vector3 position)
    {
        GameObject go = Instantiate(prefab, position, Quaternion.identity, transform);
        Obstacle obstacle = go.GetComponent<Obstacle>();

        if (obstacle == null)
        {
            Debug.LogError($"[ObstaclePool] 프리팹 '{prefab.name}'에 Obstacle 컴포넌트가 없습니다.");
            Destroy(go);
            return null;
        }

        _prefabLookup[obstacle] = prefab;
        return obstacle;
    }

    /// <summary>
    /// 지정 프리팹의 풀을 사전에 워밍업한다 (선택).
    /// </summary>
    public void Prewarm(GameObject prefab, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Obstacle obstacle = CreateNew(prefab, Vector3.zero);
            if (obstacle != null)
            {
                Return(obstacle);
            }
        }
    }
}
