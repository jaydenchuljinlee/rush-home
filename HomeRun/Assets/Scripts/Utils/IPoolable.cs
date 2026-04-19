/// <summary>
/// 오브젝트 풀링을 지원하는 오브젝트가 구현하는 인터페이스.
/// </summary>
public interface IPoolable
{
    /// <summary>풀에서 꺼낼 때 호출. 초기화 및 활성화 처리.</summary>
    void OnSpawnFromPool();

    /// <summary>풀에 반환할 때 호출. 정리 및 비활성화 처리.</summary>
    void OnReturnToPool();
}
