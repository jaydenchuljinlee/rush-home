using UnityEngine;

/// <summary>
/// 공중 장애물(AirObstacle) 전용 Y축 왕복 이동 컴포넌트.
/// Normal 난이도(30초) 이후부터 활성화되어 위아래로 왕복 이동한다.
/// Easy 구간에서는 스폰 Y 위치를 그대로 유지한다.
///
/// 사용법:
/// 1. AirObstacle 프리팹에 이 컴포넌트를 추가한다.
/// 2. Inspector에서 amplitude(이동 폭)와 frequency(왕복 빈도)를 조정한다.
/// </summary>
public class AirObstacleMover : MonoBehaviour
{
    [Header("이동 설정")]
    [Tooltip("Y축 왕복 이동 폭 (유닛 거리, 양쪽으로 ±amplitude)")]
    [SerializeField] private float amplitude = 1.5f;

    [Tooltip("초당 왕복 횟수 (Hz). 1이면 1초에 한 번 왕복")]
    [SerializeField] private float frequency = 1.5f;

    private float _spawnY;
    private float _phase;
    private bool _isMoving;

    private void OnEnable()
    {
        // 풀에서 꺼낼 때마다 현재 난이도를 기준으로 활성 여부 결정
        _phase = 0f;
    }

    private void Start()
    {
        _spawnY = transform.position.y;
        RefreshActiveState();
    }

    private void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.CurrentState != GameState.Playing) return;

        // 매 프레임 경과 시간으로 활성 상태 재평가
        RefreshActiveState();

        if (!_isMoving) return;

        _phase += Time.deltaTime;
        float yOffset = Mathf.Sin(_phase * frequency * Mathf.PI * 2f) * amplitude;

        Vector3 pos = transform.position;
        pos.y = _spawnY + yOffset;
        transform.position = pos;
    }

    /// <summary>
    /// 현재 경과 시간을 기준으로 이동 활성 여부를 갱신한다.
    /// Normal 이상(30초 이후)이면 활성화.
    /// </summary>
    private void RefreshActiveState()
    {
        if (GameManager.Instance == null)
        {
            _isMoving = false;
            return;
        }

        float elapsed = GameManager.Instance.ElapsedTime;
        bool shouldMove = elapsed >= 30f;

        if (!_isMoving && shouldMove)
        {
            // 활성화 전환 시 현재 Y를 기준점으로 재설정
            _spawnY = transform.position.y;
            _phase = 0f;
        }

        _isMoving = shouldMove;
    }

    /// <summary>
    /// 외부에서 강제로 이동 활성화 여부를 설정한다. (테스트용)
    /// </summary>
    public void SetMoving(bool active)
    {
        _isMoving = active;
        if (active)
        {
            _spawnY = transform.position.y;
            _phase = 0f;
        }
    }

    /// <summary>
    /// 이동 폭을 런타임에 변경한다. (테스트용)
    /// </summary>
    public void SetAmplitude(float value)
    {
        amplitude = value;
    }

    /// <summary>
    /// 왕복 빈도를 런타임에 변경한다. (테스트용)
    /// </summary>
    public void SetFrequency(float value)
    {
        frequency = value;
    }

    public float Amplitude => amplitude;
    public float Frequency => frequency;
    public bool IsMoving => _isMoving;
    public float SpawnY => _spawnY;
}
