using UnityEngine;

/// <summary>
/// 장애물 기본 클래스.
///
/// 컴포넌트 조합으로 장애물의 동작을 결정한다:
///   - IObstacleEffect: 충돌 시 효과 (LethalEffect, BlockEffect, KnockbackEffect 등)
///   - IObstacleMovement: 이동 패턴 (StationaryMovement, ApproachMovement 등)
///
/// isTrigger 설정:
///   - false (기본값): 물리적으로 막힘 + OnCollisionEnter로 효과 적용
///   - true: 통과하면서 OnTriggerEnter로 효과 적용 (향후 아이템, 버프존 등에 활용)
/// </summary>
[RequireComponent(typeof(Collider))]
public class ObstacleBase : MonoBehaviour, IPoolable
{
    Transform playerTransform;
    IObstacleEffect[] effects;
    IObstacleMovement[] movements;

    const float DespawnDistance = 20f;

    void Awake()
    {
        effects = GetComponents<IObstacleEffect>();
        movements = GetComponents<IObstacleMovement>();
    }

    void Start()
    {
        CachePlayer();
    }

    void Update()
    {
        if (playerTransform == null) return;

        for (int i = 0; i < movements.Length; i++)
        {
            movements[i].UpdateMovement(transform, playerTransform, Time.deltaTime);
        }

        if (transform.position.z < playerTransform.position.z - DespawnDistance)
        {
            ObstaclePool.Instance.Return(gameObject);
        }
    }

    // isTrigger=false (기본) — 물리 충돌
    void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision.gameObject);
    }

    // isTrigger=true — 통과형 (향후 확장: 아이템, 버프존, 함정 등)
    void OnTriggerEnter(Collider other)
    {
        HandleCollision(other.gameObject);
    }

    void HandleCollision(GameObject other)
    {
        var player = other.GetComponent<PlayerController>();
        if (player == null) return;

        for (int i = 0; i < effects.Length; i++)
        {
            effects[i].ApplyEffect(player);
        }
    }

    public void OnSpawnFromPool()
    {
        gameObject.SetActive(true);
        CachePlayer();
    }

    public void OnReturnToPool()
    {
        gameObject.SetActive(false);
    }

    void CachePlayer()
    {
        if (playerTransform == null)
            playerTransform = FindAnyObjectByType<PlayerController>()?.transform;
    }
}
