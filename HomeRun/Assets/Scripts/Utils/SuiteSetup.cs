using UnityEngine;

public class SuiteSetup
{
    public static void Execute()
    {
        var obstacles = Object.FindObjectsByType<Obstacle>(FindObjectsSortMode.None);
        Debug.Log($"[Suite] Active obstacles: {obstacles.Length}");
        foreach (var obs in obstacles)
        {
            if (!obs.gameObject.activeInHierarchy) continue;
            var pos = obs.transform.position;
            var mover = obs.GetComponent<AirObstacleMover>();
            string moverInfo = mover != null ? $" [Mover: moving={mover.IsMoving}]" : "";
            Debug.Log($"[Suite] {obs.name} type={obs.ObstacleType} X={pos.x:F1} Y={pos.y:F1}{moverInfo}");
        }

        var gm = Object.FindFirstObjectByType<GameManager>();
        if (gm != null) Debug.Log($"[Suite] ElapsedTime={gm.ElapsedTime:F1}");

        var gs = Object.FindFirstObjectByType<GroundScroller>();
        if (gs != null) Debug.Log($"[Suite] ScrollSpeed={gs.ScrollSpeed:F1}");
    }
}
