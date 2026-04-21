using UnityEngine;

/// <summary>
/// 공중 장애물(AirObstacle) 전용 Y축 왕복 이동 컴포넌트.
/// Normal 난이도(30초) 이후부터 활성화되어 위아래로 왕복 이동한다.
/// 지면 스크롤 속도에 비례하여 이동 폭과 빈도가 증가한다.
/// </summary>
public class AirObstacleMover : MonoBehaviour
{
    [Header("기본 이동 설정 (Easy 속도 기준)")]
    [Tooltip("기준 속도(Easy)에서의 Y축 왕복 폭")]
    [SerializeField] private float baseAmplitude = 1.2f;

    [Tooltip("기준 속도(Easy)에서의 초당 왕복 횟수")]
    [SerializeField] private float baseFrequency = 1.2f;

    [Tooltip("비례 계산 기준이 되는 지면 속도 (Easy 속도)")]
    [SerializeField] private float referenceSpeed = 6f;

    [Tooltip("Y 위치 하한선 (이 아래로 내려가지 않음 — Ground 영역 보호)")]
    [SerializeField] private float minY = 1.5f;

    [Tooltip("이동 시작 시 amplitude가 최대치에 도달하는 시간 (초)")]
    [SerializeField] private float rampUpDuration = 3f;

    private float _spawnY;
    private float _phase;
    private float _rampUpTimer;
    private bool _isMoving;
    private GroundScroller _groundScroller;

    public float Amplitude => GetScaledAmplitude();
    public float Frequency => GetScaledFrequency();
    public bool IsMoving => _isMoving;
    public float SpawnY => _spawnY;

    private void OnEnable()
    {
        _phase = 0f;
    }

    private void Start()
    {
        _spawnY = transform.position.y;
        _groundScroller = Object.FindFirstObjectByType<GroundScroller>();
        RefreshActiveState();
    }

    private void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.CurrentState != GameState.Playing) return;

        RefreshActiveState();

        if (!_isMoving) return;

        float freq = GetScaledFrequency();
        float amp = GetScaledAmplitude();

        // 점진적 amplitude 증가 (ease-in)
        _rampUpTimer += Time.deltaTime;
        float rampFactor = rampUpDuration > 0f ? Mathf.Clamp01(_rampUpTimer / rampUpDuration) : 1f;
        amp *= rampFactor;

        _phase += Time.deltaTime;
        float yOffset = Mathf.Sin(_phase * freq * Mathf.PI * 2f) * amp;

        Vector3 pos = transform.position;
        pos.y = Mathf.Max(_spawnY + yOffset, minY);
        transform.position = pos;
    }

    private float GetSpeedRatio()
    {
        if (_groundScroller == null || referenceSpeed <= 0f) return 1f;
        return _groundScroller.ScrollSpeed / referenceSpeed;
    }

    private float GetScaledAmplitude()
    {
        return baseAmplitude * GetSpeedRatio();
    }

    private float GetScaledFrequency()
    {
        return baseFrequency * GetSpeedRatio();
    }

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
            _spawnY = transform.position.y;
            _phase = 0f;
            _rampUpTimer = 0f;
        }

        _isMoving = shouldMove;
    }

    // --- 테스트/외부 설정용 ---

    public void SetMoving(bool active)
    {
        _isMoving = active;
        if (active)
        {
            _spawnY = transform.position.y;
            _phase = 0f;
        }
    }

    public void SetAmplitude(float value) { baseAmplitude = value; }
    public void SetFrequency(float value) { baseFrequency = value; }
    public void SetGroundScroller(GroundScroller scroller) { _groundScroller = scroller; }
}
