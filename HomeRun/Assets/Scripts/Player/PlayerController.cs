using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어 자동 달리기, 점프, 슬라이딩을 처리하는 컨트롤러.
/// 러너 게임에서 플레이어는 수평 고정, 월드가 스크롤됨.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Jump")]
    [SerializeField] private float jumpForce = 9f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Header("Slide")]
    [SerializeField] private float slideDuration = 0.5f;

    private Rigidbody2D _rigidbody;
    private BoxCollider2D _collider;
    private SpriteRenderer _spriteRenderer;
    private Vector2 _normalColliderSize;
    private Vector2 _normalColliderOffset;
    private Vector3 _normalScale;
    private bool _isGrounded;
    private bool _isSliding;
    private float _slideTimer;

    // 스와이프 감지
    private Vector2 _touchStartPos;
    private bool _isTouching;
    private const float SwipeThreshold = 50f;

    public bool IsGrounded => _isGrounded;
    public bool IsSliding => _isSliding;
    public float JumpForce => jumpForce;

    public static event System.Action OnPlayerHit;

#if UNITY_EDITOR
    /// <summary>디버그 무적 모드. true면 OnPlayerHit을 발동하지 않는다.</summary>
    public static bool DebugInvincible { get; set; } = false;
#endif

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _normalColliderSize = _collider.size;
        _normalColliderOffset = _collider.offset;
        _normalScale = transform.localScale;
    }

    private void Update()
    {
        if (!GameManager.IsPlaying) return;

        CheckGround();
        HandleInput();
        UpdateSlide();
        CheckFallDeath();
    }

    private void CheckGround()
    {
        if (groundCheck == null)
        {
            _isGrounded = false;
            return;
        }
        // BoxCast로 콜라이더 너비 전체를 아래 방향으로 감지 — 경사면 PolygonCollider2D도 안정적으로 감지
        float castWidth = _collider != null ? _collider.size.x * 0.8f : 0.5f;
        _isGrounded = Physics2D.BoxCast(
            groundCheck.position,
            new Vector2(castWidth, 0.05f),
            0f,
            Vector2.down,
            groundCheckRadius,
            groundLayer
        );
    }

    private void HandleInput()
    {
#if UNITY_EDITOR
        HandleKeyboardInput();
#endif
        HandleTouchInput();
    }

    private void HandleKeyboardInput()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            TryJump();
        }

        if (Keyboard.current.downArrowKey.wasPressedThisFrame || Keyboard.current.sKey.wasPressedThisFrame)
        {
            TrySlide();
        }
    }

    private void HandleTouchInput()
    {
        if (Touchscreen.current == null) return;

        var touch = Touchscreen.current.primaryTouch;

        if (touch.press.wasPressedThisFrame)
        {
            _touchStartPos = touch.position.ReadValue();
            _isTouching = true;
        }
        else if (touch.press.wasReleasedThisFrame && _isTouching)
        {
            _isTouching = false;
            Vector2 touchEndPos = touch.position.ReadValue();
            Vector2 swipeDelta = touchEndPos - _touchStartPos;

            if (swipeDelta.magnitude < SwipeThreshold)
            {
                // 탭 = 점프
                TryJump();
            }
            else if (swipeDelta.y < -SwipeThreshold && Mathf.Abs(swipeDelta.y) > Mathf.Abs(swipeDelta.x))
            {
                // 아래 스와이프 = 슬라이딩
                TrySlide();
            }
        }
    }

    private void TryJump()
    {
        if (!_isGrounded || _isSliding) return;

        _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, 0f);
        _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void TrySlide()
    {
        if (!_isGrounded || _isSliding) return;

        _isSliding = true;
        _slideTimer = slideDuration;

        // 콜라이더를 줄임 — 빨간 박스를 아슬아슬하게 피할 수 있는 높이
        float slideRatio = 0.4f;
        _collider.size = new Vector2(_normalColliderSize.x, _normalColliderSize.y * slideRatio);
        _collider.offset = new Vector2(_normalColliderOffset.x, _normalColliderOffset.y - _normalColliderSize.y * (1f - slideRatio) * 0.5f);
    }

    private void UpdateSlide()
    {
        if (!_isSliding) return;

        _slideTimer -= Time.deltaTime;
        if (_slideTimer <= 0f)
        {
            EndSlide();
        }
    }

    private void EndSlide()
    {
        _isSliding = false;
        _collider.size = _normalColliderSize;
        _collider.offset = _normalColliderOffset;

        // 스프라이트 스케일 복구
        transform.localScale = _normalScale;
    }

    private void CheckFallDeath()
    {
        if (transform.position.y < -2f && _rigidbody.linearVelocity.y < -1f)
        {
            // 떨어지기 시작할 때 바로 기록
            var gs = FindAnyObjectByType<GroundScroller>();
            if (gs != null)
            {
                for (int i = 0; i < gs.transform.childCount; i++)
                {
                    var child = gs.transform.GetChild(i);
                    var tt = child.GetComponent<TerrainTile>();
                    if (tt != null)
                    {
                        float dist = Mathf.Abs(child.position.x - transform.position.x);
                        if (dist < 20f)
                            Debug.LogError($"[FALL] Tile '{child.name}' X={child.position.x:F1} type={tt.CurrentType} L={tt.LeftTopYOffset:F2} R={tt.RightTopYOffset:F2} hasGround={tt.HasGround}");
                    }
                }
                Debug.LogError($"[FALL] Player X={transform.position.x:F1} Y={transform.position.y:F1} velY={_rigidbody.linearVelocity.y:F1}");
            }
            OnPlayerHit?.Invoke();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
#if UNITY_EDITOR
            if (DebugInvincible) return;
#endif
            OnPlayerHit?.Invoke();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        // BoxCast 범위 시각화
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        float castWidth = col != null ? col.size.x * 0.8f : 0.5f;
        Gizmos.DrawWireCube(
            groundCheck.position + Vector3.down * groundCheckRadius * 0.5f,
            new Vector3(castWidth, groundCheckRadius, 0f)
        );
    }
}
