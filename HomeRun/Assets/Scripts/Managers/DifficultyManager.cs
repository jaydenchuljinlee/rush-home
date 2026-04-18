using UnityEngine;

/// <summary>
/// 경과 시간에 따라 난이도(스크롤 속도)를 조절하는 싱글톤 매니저.
/// DifficultyData ScriptableObject에서 단계별 속도 값을 읽어 GroundScroller에 적용.
/// GroundScroller는 F-02에서 Inspector 할당.
/// </summary>
public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance { get; private set; }

    [SerializeField] private DifficultyData difficultyData;
    [SerializeField] private GroundScroller groundScroller;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.CurrentState != GameState.Playing) return;
        if (difficultyData == null) return;

        float elapsed = GameManager.Instance.ElapsedTime;
        float targetSpeed = difficultyData.GetSpeedForTime(elapsed);

        if (groundScroller != null)
        {
            groundScroller.ScrollSpeed = targetSpeed;
        }
    }

    private void HandleGameStateChanged(GameState state)
    {
        // 게임 시작 시 Easy 속도로 초기화
        if (state == GameState.Playing && difficultyData != null && groundScroller != null)
        {
            groundScroller.ScrollSpeed = difficultyData.EasySpeed;
        }
    }

    /// <summary>
    /// 현재 경과 시간에 해당하는 목표 속도를 반환한다. (테스트 및 외부 접근용)
    /// </summary>
    public float GetCurrentTargetSpeed()
    {
        if (difficultyData == null || GameManager.Instance == null) return 0f;
        return difficultyData.GetSpeedForTime(GameManager.Instance.ElapsedTime);
    }
}
