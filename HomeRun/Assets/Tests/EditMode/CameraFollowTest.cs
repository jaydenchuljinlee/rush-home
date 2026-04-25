using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class CameraFollowTest
{
    [Test]
    public void 성공_SnapToTarget_플레이어상승시_카메라Y상승()
    {
        // Arrange
        GameObject cameraGo = new GameObject("Camera");
        cameraGo.transform.position = new Vector3(0f, 1f, -10f);
        GameObject playerGo = new GameObject("Player");
        playerGo.transform.position = Vector3.zero;

        CameraFollow follow = cameraGo.AddComponent<CameraFollow>();
        SetTarget(follow, playerGo.transform);

        // Act
        InvokeAwake(follow);
        playerGo.transform.position = new Vector3(0f, 2f, 0f);
        follow.SnapToTarget();

        // Assert
        Assert.Greater(cameraGo.transform.position.y, 1f);
        Assert.AreEqual(0f, cameraGo.transform.position.x, 0.001f);
        Assert.AreEqual(-10f, cameraGo.transform.position.z, 0.001f);

        // Cleanup
        Object.DestroyImmediate(cameraGo);
        Object.DestroyImmediate(playerGo);
    }

    [Test]
    public void 성공_SnapToTarget_플레이어하강시_초기Y아래로_내려가지않음()
    {
        // Arrange
        GameObject cameraGo = new GameObject("Camera");
        cameraGo.transform.position = new Vector3(0f, 1f, -10f);
        GameObject playerGo = new GameObject("Player");
        playerGo.transform.position = Vector3.zero;

        CameraFollow follow = cameraGo.AddComponent<CameraFollow>();
        SetTarget(follow, playerGo.transform);

        // Act
        InvokeAwake(follow);
        playerGo.transform.position = new Vector3(0f, -4f, 0f);
        follow.SnapToTarget();

        // Assert
        Assert.AreEqual(1f, cameraGo.transform.position.y, 0.001f);

        // Cleanup
        Object.DestroyImmediate(cameraGo);
        Object.DestroyImmediate(playerGo);
    }

    private static void SetTarget(CameraFollow follow, Transform target)
    {
        typeof(CameraFollow)
            .GetField("target", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(follow, target);
    }

    private static void InvokeAwake(CameraFollow follow)
    {
        typeof(CameraFollow)
            .GetMethod("Awake", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .Invoke(follow, null);
    }
}
