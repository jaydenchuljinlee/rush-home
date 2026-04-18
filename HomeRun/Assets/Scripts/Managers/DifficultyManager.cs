using UnityEngine;

/// <summary>
/// 게임 경과 시간을 감지하여 GroundScroller 속도를 실시간으로 갱신한다.
/// DifficultyData를 기준으로 현재 단계의 스크롤 속도를 적용한다.
/// </summary>
public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance { get; private set; }

    [SerializeField] private DifficultyData _difficultyData;
    [SerializeField] private GroundScroller _groundScroller;

    public DifficultyData DifficultyData => _difficultyData;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Playing)
            return;

        if (_difficultyData == null || _groundScroller == null) return;

        float elapsed = GameManager.Instance.ElapsedTime;
        float targetSpeed = _difficultyData.GetScrollSpeed(elapsed);

        // 변경이 있을 때만 갱신
        if (!Mathf.Approximately(_groundScroller.ScrollSpeed, targetSpeed))
        {
            _groundScroller.ScrollSpeed = targetSpeed;
        }
    }
}
