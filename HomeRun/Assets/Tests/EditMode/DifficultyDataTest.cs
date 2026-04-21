using NUnit.Framework;
using UnityEngine;

/// <summary>
/// DifficultyData ScriptableObject의 속도 계산 및 스폰 간격 로직 검증.
/// Edit Mode 테스트 - Unity 에디터 실행 불필요.
///
/// 난이도 임계값: Easy(0~30초), Normal(30~60초), Hard(60~120초), Extreme(120초~)
/// </summary>
[TestFixture]
public class DifficultyDataTest
{
    private DifficultyData _data;

    [SetUp]
    public void SetUp()
    {
        _data = ScriptableObject.CreateInstance<DifficultyData>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_data);
    }

    // ===== 속도 테스트 =====

    [Test]
    public void 성공_0초에서_Easy속도_반환()
    {
        float speed = _data.GetSpeedForTime(0f);
        Assert.AreEqual(_data.EasySpeed, speed, "0초에서는 EasySpeed를 반환해야 한다.");
    }

    [Test]
    public void 성공_30초에서_Normal속도_반환()
    {
        float speed = _data.GetSpeedForTime(30f);
        Assert.AreEqual(_data.NormalSpeed, speed, "30초(NormalThreshold)에서는 NormalSpeed를 반환해야 한다.");
    }

    [Test]
    public void 성공_60초에서_Hard속도_반환()
    {
        float speed = _data.GetSpeedForTime(60f);
        Assert.AreEqual(_data.HardSpeed, speed, "60초(HardThreshold)에서는 HardSpeed를 반환해야 한다.");
    }

    [Test]
    public void 성공_90초에서_Hard속도_반환()
    {
        // 90초는 Hard 구간 (60초 이상, 120초 미만)
        float speed = _data.GetSpeedForTime(90f);
        Assert.AreEqual(_data.HardSpeed, speed, "90초는 Hard 구간이므로 HardSpeed를 반환해야 한다.");
    }

    [Test]
    public void 성공_120초에서_Extreme속도_반환()
    {
        float speed = _data.GetSpeedForTime(120f);
        Assert.AreEqual(_data.ExtremeSpeed, speed, "120초(ExtremeThreshold)에서는 ExtremeSpeed를 반환해야 한다.");
    }

    [Test]
    public void 성공_기본값_Easy속도가_0보다_큼()
    {
        Assert.Greater(_data.EasySpeed, 0f, "EasySpeed는 0보다 커야 한다.");
    }

    [Test]
    public void 성공_속도_단계적_증가_Easy_Normal_Hard_Extreme()
    {
        Assert.Less(_data.EasySpeed, _data.NormalSpeed, "EasySpeed < NormalSpeed");
        Assert.Less(_data.NormalSpeed, _data.HardSpeed, "NormalSpeed < HardSpeed");
        Assert.Less(_data.HardSpeed, _data.ExtremeSpeed, "HardSpeed < ExtremeSpeed");
    }

    [Test]
    public void 성공_임계값_이전_시간은_이전_단계_속도_반환()
    {
        // 29초는 Easy 구간
        float speedAt29 = _data.GetSpeedForTime(29f);
        Assert.AreEqual(_data.EasySpeed, speedAt29, "29초는 Easy 속도여야 한다.");
    }

    [Test]
    public void 성공_매우_큰_시간에서_Extreme속도_반환()
    {
        float speed = _data.GetSpeedForTime(9999f);
        Assert.AreEqual(_data.ExtremeSpeed, speed, "매우 긴 시간에서는 ExtremeSpeed를 반환해야 한다.");
    }

    [Test]
    public void 성공_ExtremeThreshold가_120초()
    {
        Assert.AreEqual(120f, _data.ExtremeThreshold, "ExtremeThreshold는 120초여야 한다.");
    }

    // ===== 스폰 간격 테스트 =====

    [Test]
    public void 성공_0초에서_Easy스폰간격_반환()
    {
        var (min, max) = _data.GetSpawnIntervalForTime(0f);
        Assert.AreEqual(_data.EasySpawnMin, min, "0초에서 min은 EasySpawnMin이어야 한다.");
        Assert.AreEqual(_data.EasySpawnMax, max, "0초에서 max는 EasySpawnMax이어야 한다.");
    }

    [Test]
    public void 성공_30초에서_Normal스폰간격_반환()
    {
        var (min, max) = _data.GetSpawnIntervalForTime(30f);
        Assert.AreEqual(_data.NormalSpawnMin, min, "30초에서 min은 NormalSpawnMin이어야 한다.");
        Assert.AreEqual(_data.NormalSpawnMax, max, "30초에서 max는 NormalSpawnMax이어야 한다.");
    }

    [Test]
    public void 성공_60초에서_Hard스폰간격_반환()
    {
        var (min, max) = _data.GetSpawnIntervalForTime(60f);
        Assert.AreEqual(_data.HardSpawnMin, min, "60초에서 min은 HardSpawnMin이어야 한다.");
        Assert.AreEqual(_data.HardSpawnMax, max, "60초에서 max는 HardSpawnMax이어야 한다.");
    }

    [Test]
    public void 성공_120초에서_Extreme스폰간격_반환()
    {
        var (min, max) = _data.GetSpawnIntervalForTime(120f);
        Assert.AreEqual(_data.ExtremeSpawnMin, min, "120초에서 min은 ExtremeSpawnMin이어야 한다.");
        Assert.AreEqual(_data.ExtremeSpawnMax, max, "120초에서 max는 ExtremeSpawnMax이어야 한다.");
    }

    [Test]
    public void 성공_스폰간격_단계별_감소()
    {
        // 단계가 올라갈수록 간격이 줄어야(더 자주 스폰) 한다
        Assert.Greater(_data.EasySpawnMin, _data.NormalSpawnMin, "EasySpawnMin > NormalSpawnMin");
        Assert.Greater(_data.NormalSpawnMin, _data.HardSpawnMin, "NormalSpawnMin > HardSpawnMin");
        Assert.Greater(_data.HardSpawnMin, _data.ExtremeSpawnMin, "HardSpawnMin > ExtremeSpawnMin");

        Assert.Greater(_data.EasySpawnMax, _data.NormalSpawnMax, "EasySpawnMax > NormalSpawnMax");
        Assert.Greater(_data.NormalSpawnMax, _data.HardSpawnMax, "NormalSpawnMax > HardSpawnMax");
        Assert.Greater(_data.HardSpawnMax, _data.ExtremeSpawnMax, "HardSpawnMax > ExtremeSpawnMax");
    }

    [Test]
    public void 성공_스폰간격_min이_max보다_작거나_같음()
    {
        Assert.LessOrEqual(_data.EasySpawnMin, _data.EasySpawnMax, "Easy: min <= max");
        Assert.LessOrEqual(_data.NormalSpawnMin, _data.NormalSpawnMax, "Normal: min <= max");
        Assert.LessOrEqual(_data.HardSpawnMin, _data.HardSpawnMax, "Hard: min <= max");
        Assert.LessOrEqual(_data.ExtremeSpawnMin, _data.ExtremeSpawnMax, "Extreme: min <= max");
    }
}
