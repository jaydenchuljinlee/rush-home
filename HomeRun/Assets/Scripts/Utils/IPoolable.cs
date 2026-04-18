/// <summary>
/// 오브젝트 풀링에 참여하는 오브젝트가 구현해야 하는 인터페이스.
/// 풀에서 꺼낼 때와 반환할 때 초기화/정리 로직을 분리한다.
/// </summary>
public interface IPoolable
{
    /// <summary>풀에서 꺼내 활성화될 때 호출.</summary>
    void OnSpawnFromPool();

    /// <summary>풀로 반환되어 비활성화될 때 호출.</summary>
    void OnReturnToPool();
}
