using UnityEngine;

/// <summary>
/// 플레이어 자동 달리기, 점프, 슬라이딩을 처리하는 컨트롤러.
/// 러너 게임에서 플레이어는 수평 고정, 월드가 스크롤됨.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Jump")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Header("Slide")]
    [SerializeField] private float slideDuration = 0.5f;

    private Rigidbody2D _rigidbody;
    private BoxCollider2D _collider;
    private Vector2 _normalColliderSize;
    private Vector2 _normalColliderOffset;
    private bool _isGrounded;
    private bool _isSliding;
    private float _slideTimer;

    // 스와이프 감지
    private Vector2 _touchStartPos;
    private bool _isTouching;
    private const float SwipeThreshold = 50f;

    public bool IsGrounded => _isGrounded;
    public bool IsSliding => _isSliding;

    public static event System.Action OnPlayerHit;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();

        _normalColliderSize = _collider.size;
        _normalColliderOffset = _collider.offset;
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Playing)
            return;

        CheckGround();
        HandleInput();
        UpdateSlide();
    }

    private void CheckGround()
    {
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
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
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            TryJump();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            TrySlide();
        }
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount <= 0) return;

        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                _touchStartPos = touch.position;
                _isTouching = true;
                break;

            case TouchPhase.Ended:
                if (!_isTouching) break;
                _isTouching = false;

                Vector2 swipeDelta = touch.position - _touchStartPos;

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
                break;
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

        // 콜라이더를 절반 높이로 줄임
        _collider.size = new Vector2(_normalColliderSize.x, _normalColliderSize.y * 0.5f);
        _collider.offset = new Vector2(_normalColliderOffset.x, _normalColliderOffset.y - _normalColliderSize.y * 0.25f);
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
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            OnPlayerHit?.Invoke();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
