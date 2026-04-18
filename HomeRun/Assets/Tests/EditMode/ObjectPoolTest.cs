using NUnit.Framework;
using UnityEngine;

/// <summary>
/// ObjectPool 순수 로직 테스트.
/// 더미 컴포넌트를 사용하여 풀 동작을 검증한다.
/// </summary>
[TestFixture]
public class ObjectPoolTest
{
    /// <summary>테스트용 더미 IPoolable 컴포넌트.</summary>
    private class DummyPoolable : MonoBehaviour, IPoolable
    {
        public int SpawnCount { get; private set; }
        public int ReturnCount { get; private set; }

        public void OnSpawnFromPool() => SpawnCount++;
        public void OnReturnToPool() => ReturnCount++;
    }

    private GameObject _prefabGo;
    private DummyPoolable _prefab;

    [SetUp]
    public void SetUp()
    {
        _prefabGo = new GameObject("DummyPrefab");
        _prefab = _prefabGo.AddComponent<DummyPoolable>();
    }

    [TearDown]
    public void TearDown()
    {
        // 씬에 남은 오브젝트 정리
        foreach (var obj in Object.FindObjectsByType<DummyPoolable>(FindObjectsSortMode.None))
        {
            Object.DestroyImmediate(obj.gameObject);
        }
    }

    // ---- 초기 풀 크기 ----

    [Test]
    public void 성공_초기화시_지정한크기만큼_비활성오브젝트_생성()
    {
        // Arrange & Act
        var pool = new ObjectPool<DummyPoolable>(_prefab, initialSize: 3, maxSize: 10);

        // Assert
        Assert.AreEqual(3, pool.CountInactive, "초기 풀 크기가 3이어야 한다.");
    }

    // ---- Get ----

    [Test]
    public void 성공_Get호출시_활성화된_오브젝트_반환()
    {
        // Arrange
        var pool = new ObjectPool<DummyPoolable>(_prefab, initialSize: 1, maxSize: 5);

        // Act
        DummyPoolable instance = pool.Get();

        // Assert
        Assert.IsNotNull(instance);
        Assert.IsTrue(instance.gameObject.activeSelf, "Get된 오브젝트는 활성화되어야 한다.");
    }

    [Test]
    public void 성공_Get호출시_OnSpawnFromPool_콜백_호출()
    {
        // Arrange
        var pool = new ObjectPool<DummyPoolable>(_prefab, initialSize: 1, maxSize: 5);

        // Act
        DummyPoolable instance = pool.Get();

        // Assert
        Assert.AreEqual(1, instance.SpawnCount, "OnSpawnFromPool이 한 번 호출되어야 한다.");
    }

    [Test]
    public void 성공_풀이_비어있을때_새_인스턴스_생성()
    {
        // Arrange
        var pool = new ObjectPool<DummyPoolable>(_prefab, initialSize: 0, maxSize: 5);
        Assert.AreEqual(0, pool.CountInactive);

        // Act
        DummyPoolable instance = pool.Get();

        // Assert
        Assert.IsNotNull(instance, "풀이 비어있어도 새 인스턴스를 생성해야 한다.");
    }

    // ---- Return ----

    [Test]
    public void 성공_Return호출시_오브젝트_비활성화()
    {
        // Arrange
        var pool = new ObjectPool<DummyPoolable>(_prefab, initialSize: 1, maxSize: 5);
        DummyPoolable instance = pool.Get();

        // Act
        pool.Return(instance);

        // Assert
        Assert.IsFalse(instance.gameObject.activeSelf, "Return된 오브젝트는 비활성화되어야 한다.");
    }

    [Test]
    public void 성공_Return호출시_풀_크기_증가()
    {
        // Arrange
        var pool = new ObjectPool<DummyPoolable>(_prefab, initialSize: 0, maxSize: 5);
        DummyPoolable instance = pool.Get();
        Assert.AreEqual(0, pool.CountInactive);

        // Act
        pool.Return(instance);

        // Assert
        Assert.AreEqual(1, pool.CountInactive, "반환 후 풀 크기가 1이어야 한다.");
    }

    [Test]
    public void 성공_Return호출시_OnReturnToPool_콜백_호출()
    {
        // Arrange
        var pool = new ObjectPool<DummyPoolable>(_prefab, initialSize: 1, maxSize: 5);
        DummyPoolable instance = pool.Get();

        // Act
        pool.Return(instance);

        // Assert
        Assert.AreEqual(1, instance.ReturnCount, "OnReturnToPool이 한 번 호출되어야 한다.");
    }

    [Test]
    public void 성공_Get_Return_재사용_사이클_정상작동()
    {
        // Arrange
        var pool = new ObjectPool<DummyPoolable>(_prefab, initialSize: 1, maxSize: 5);

        // Act
        DummyPoolable first = pool.Get();
        pool.Return(first);
        DummyPoolable second = pool.Get();

        // Assert — 같은 인스턴스를 재사용해야 한다
        Assert.AreSame(first, second, "반환 후 다시 Get하면 같은 인스턴스를 재사용해야 한다.");
        Assert.AreEqual(2, second.SpawnCount, "두 번 꺼냈으므로 SpawnCount가 2여야 한다.");
    }

    // ---- MaxSize 초과 ----

    [Test]
    public void 성공_maxSize초과시_반환오브젝트_파괴()
    {
        // Arrange — maxSize=1, 인스턴스 2개 꺼낸 후 반환
        var pool = new ObjectPool<DummyPoolable>(_prefab, initialSize: 0, maxSize: 1);
        DummyPoolable inst1 = pool.Get();
        DummyPoolable inst2 = pool.Get();

        // Act
        pool.Return(inst1); // 풀 크기: 1 (= maxSize)
        pool.Return(inst2); // 초과 -> 파괴

        // Assert
        Assert.AreEqual(1, pool.CountInactive,
            "maxSize 초과 반환 시 풀 크기는 maxSize를 넘지 않아야 한다.");
        Assert.IsTrue(inst2 == null || !inst2.gameObject.activeSelf,
            "maxSize 초과된 오브젝트는 파괴되어야 한다.");
    }

    // ---- Null 안전성 ----

    [Test]
    public void 성공_null반환시_예외없이_처리()
    {
        // Arrange
        var pool = new ObjectPool<DummyPoolable>(_prefab, initialSize: 1, maxSize: 5);

        // Act & Assert — 예외 없이 처리되어야 한다
        Assert.DoesNotThrow(() => pool.Return(null));
    }
}
