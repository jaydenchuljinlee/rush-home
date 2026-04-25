using UnityEngine;

/// <summary>
/// 플레이어 애니메이션 상태를 관리하는 컴포넌트.
/// PlayerController의 상태를 읽어 Animator 파라미터를 갱신한다.
/// 스프라이트 시트가 없는 경우 SpriteRenderer 색상으로 상태를 시각화한다.
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerAnimationController : MonoBehaviour
{
    // Animator 파라미터 이름 상수
    private static readonly int ParamIsPlaying  = Animator.StringToHash("isPlaying");
    private static readonly int ParamIsGrounded = Animator.StringToHash("isGrounded");
    private static readonly int ParamIsSliding  = Animator.StringToHash("isSliding");
    private static readonly int ParamHit        = Animator.StringToHash("hit");

    // Placeholder 색상 정의
    private static readonly Color ColorIdle  = Color.white;
    private static readonly Color ColorRun   = new Color(0.2f, 0.8f, 0.2f, 1f);
    private static readonly Color ColorJump  = new Color(0.2f, 0.4f, 1f,   1f);
    private static readonly Color ColorSlide = new Color(1f,   0.9f, 0f,   1f);
    private static readonly Color ColorHit   = new Color(1f,   0.2f, 0.2f, 1f);

    private Animator         _animator;
    private SpriteRenderer   _spriteRenderer;
    private PlayerController _playerController;

    private bool _isHit;
    private float _hitTimer;
    private const float HitDisplayDuration = 0.3f;

    private void Awake()
    {
        _animator        = GetComponent<Animator>();
        _spriteRenderer  = GetComponent<SpriteRenderer>();
        _playerController = GetComponent<PlayerController>();

        if (_playerController == null)
        {
            Debug.LogError("[PlayerAnimationController] PlayerController 컴포넌트를 찾을 수 없습니다.", this);
        }
    }

    private void OnEnable()
    {
        PlayerController.OnPlayerHit += HandlePlayerHit;
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        PlayerController.OnPlayerHit -= HandlePlayerHit;
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void Update()
    {
        UpdateHitTimer();
        UpdateAnimatorParams();
        UpdatePlaceholderColor();
    }

    private void UpdateHitTimer()
    {
        if (!_isHit) return;

        _hitTimer -= Time.deltaTime;
        if (_hitTimer <= 0f)
        {
            _isHit = false;
        }
    }

    private void UpdateAnimatorParams()
    {
        if (_playerController == null) return;

        bool isPlaying  = GameManager.IsPlaying;
        bool isGrounded = _playerController.IsGrounded;
        bool isSliding  = _playerController.IsSliding;

        _animator.SetBool(ParamIsPlaying,  isPlaying);
        _animator.SetBool(ParamIsGrounded, isGrounded);
        _animator.SetBool(ParamIsSliding,  isSliding);
    }

    private void UpdatePlaceholderColor()
    {
        if (_isHit)
        {
            _spriteRenderer.color = ColorHit;
            return;
        }

        if (!GameManager.IsPlaying)
        {
            _spriteRenderer.color = ColorIdle;
            return;
        }

        if (_playerController == null) return;

        if (_playerController.IsSliding)
        {
            _spriteRenderer.color = ColorSlide;
        }
        else if (!_playerController.IsGrounded)
        {
            _spriteRenderer.color = ColorJump;
        }
        else
        {
            _spriteRenderer.color = ColorRun;
        }
    }

    private void HandlePlayerHit()
    {
        _isHit = true;
        _hitTimer = HitDisplayDuration;
        _animator.SetTrigger(ParamHit);
    }

    private void HandleGameStateChanged(GameState newState)
    {
        if (newState == GameState.Ready)
        {
            _isHit = false;
            _spriteRenderer.color = ColorIdle;
        }
    }
}
