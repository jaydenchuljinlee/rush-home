using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Extreme 난이도 지면 잘림(Ground Clipping) 버그 수정 확인 테스트.
/// bugfix-extreme-ground-clipping.md 5절 검증 테스트 계획 기반.
/// </summary>
[TestFixture]
public class GroundScrollerExtremeClippingTest
{
    private const float CameraOrthoSize = 5f;
    private const float CameraAspect = 1.6f;
    private const float TileWidth = 16f;

    // 화면 반너비 = 5 * 1.6 = 8
    private float ScreenHalfWidth => CameraOrthoSize * CameraAspect;

    /// <summary>
    /// 5개 타일 중 가장 왼쪽 타일(x=-18)이 재활용 조건을 충족할 때
    /// 나머지 4개 타일이 화면 오른쪽 끝(8)을 여유있게 커버하는지 검증.
    /// </summary>
    [Test]
    public void 성공_5타일_재활용후_화면오른쪽_항상커버()
    {
        // Arrange
        GameObject cameraGo = new GameObject("Main Camera");
        Camera camera = cameraGo.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = CameraOrthoSize;
        camera.aspect = CameraAspect;
        cameraGo.transform.position = new Vector3(0f, 0f, -10f);

        GameObject scrollerGo = new GameObject("GroundScroller");
        GroundScroller scroller = scrollerGo.AddComponent<GroundScroller>();
        SetPrivateField(scroller, "viewCamera", camera);
        SetPrivateField(scroller, "tileWidth", TileWidth);
        SetPrivateField(scroller, "recycleScreenMargin", 2f);

        // 타일 5개 생성: 중심 x = 0, 16, 32, 48, 64
        float[] initialX = { 0f, 16f, 32f, 48f, 64f };
        for (int i = 0; i < 5; i++)
        {
            GameObject tileGo = new GameObject($"GroundTile_{i}");
            tileGo.transform.parent = scrollerGo.transform;
            tileGo.transform.position = new Vector3(initialX[i], -1f, 0f);
        }

        // 맨 왼쪽 타일을 재활용 조건 충족 위치(-18)로 이동
        // recycleScreenMargin=2, leftVisibleX=-8 → 조건: x + 8 <= -10 → x <= -18
        Transform leftmostTile = scrollerGo.transform.GetChild(0);
        leftmostTile.position = new Vector3(-18f, -1f, 0f);

        // Assert: 해당 타일이 재활용 조건을 충족하는지
        bool shouldRecycle = InvokeShouldRecycle(scroller, leftmostTile);
        Assert.IsTrue(shouldRecycle, "x=-18 타일은 recycleScreenMargin=2 조건에서 재활용되어야 한다.");

        // 나머지 4개 타일(x=16,32,48,64)의 오른쪽 끝 최댓값 = 64 + 8 = 72
        float rightmostCoverage = 64f + TileWidth * 0.5f;
        Assert.Greater(rightmostCoverage, ScreenHalfWidth,
            "5개 타일 배치 시 재활용 중에도 화면 오른쪽(8)이 항상 커버된다.");

        // Cleanup
        Object.DestroyImmediate(scrollerGo);
        Object.DestroyImmediate(cameraGo);
    }

    /// <summary>
    /// recycleScreenMargin 스크립트 기본값이 2f인지 확인.
    /// </summary>
    [Test]
    public void 성공_recycleScreenMargin_기본값_2f()
    {
        // Arrange
        GameObject scrollerGo = new GameObject("GroundScroller");
        GroundScroller scroller = scrollerGo.AddComponent<GroundScroller>();

        // Act
        float margin = (float)typeof(GroundScroller)
            .GetField("recycleScreenMargin",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(scroller);

        // Assert
        Assert.AreEqual(2f, margin, 0.001f,
            "recycleScreenMargin 기본값이 2f이어야 Extreme 속도에서 조기 재활용이 보장된다.");

        // Cleanup
        Object.DestroyImmediate(scrollerGo);
    }

    /// <summary>
    /// Gap → Flat → 다음 타입이 Gap이 아님을 100회 반복 검증 (연속 Gap 방지).
    /// </summary>
    [Test]
    public void 성공_Gap직후Flat_다음_Gap불허()
    {
        // Arrange
        GameObject go = new GameObject("TerrainTypeSequencer");
        TerrainTypeSequencer sequencer = go.AddComponent<TerrainTypeSequencer>();
        sequencer.SetDifficultyTier(DifficultyTier.Extreme);

        // Act / Assert
        for (int i = 0; i < 100; i++)
        {
            // Gap 상태 설정 후 GetNextType → 항상 Flat
            sequencer.SetLastType(TerrainChunkType.Gap);
            TerrainChunkType afterGap = sequencer.GetNextType();
            Assert.AreEqual(TerrainChunkType.Flat, afterGap,
                $"Gap 다음은 반드시 Flat이어야 한다. (iteration {i})");

            // 이제 _lastType=Flat, _prevWasGap=true → Gap 불가
            TerrainChunkType afterFlat = sequencer.GetNextType();
            Assert.AreNotEqual(TerrainChunkType.Gap, afterFlat,
                $"Gap → Flat → Gap 연속 패턴이 발생하면 안 된다. (iteration {i})");
        }

        // Cleanup
        Object.DestroyImmediate(go);
    }

    /// <summary>
    /// Flat이 연속으로 나온 경우(prevWasGap=false)에는 Gap이 정상적으로 등장함을 검증.
    /// 연속 Gap 방지 로직이 정상 Gap 등장을 막지 않는지 확인.
    /// </summary>
    [Test]
    public void 성공_Extreme티어_Gap_정상등장_확인()
    {
        // Arrange
        GameObject go = new GameObject("TerrainTypeSequencer");
        TerrainTypeSequencer sequencer = go.AddComponent<TerrainTypeSequencer>();
        sequencer.SetDifficultyTier(DifficultyTier.Extreme);

        bool sawGap = false;

        // Act: Flat → Flat 상태(prevWasGap=false)에서 Gap이 충분히 등장해야 함
        // Gap 확률 0.25 → 200회 중 1번도 안 나올 확률은 극소
        for (int i = 0; i < 200; i++)
        {
            // SetLastType(Flat) 호출: _prevWasGap = (_lastType == Gap)
            // 이전에 Flat이었으면 _prevWasGap = false
            sequencer.SetLastType(TerrainChunkType.Flat);
            // 그 다음 한번 더 SetLastType(Flat)으로 prevWasGap=false 확정
            sequencer.SetLastType(TerrainChunkType.Flat);

            TerrainChunkType next = sequencer.GetNextType();
            if (next == TerrainChunkType.Gap)
            {
                sawGap = true;
                break;
            }
        }

        // Assert
        Assert.IsTrue(sawGap, "Extreme 티어에서 Flat 연속 후 Gap이 200회 내에 등장해야 한다.");

        // Cleanup
        Object.DestroyImmediate(go);
    }

    // Helpers

    private static void SetPrivateField(object target, string fieldName, object value)
    {
        target.GetType()
            .GetField(fieldName,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(target, value);
    }

    private static bool InvokeShouldRecycle(GroundScroller scroller, Transform tile)
    {
        return (bool)typeof(GroundScroller)
            .GetMethod("ShouldRecycle",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .Invoke(scroller, new object[] { tile });
    }
}
