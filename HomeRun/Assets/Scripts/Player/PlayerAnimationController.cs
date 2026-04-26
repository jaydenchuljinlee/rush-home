using UnityEngine;

/// <summary>
/// 플레이어 애니메이션 상태를 관리하는 컴포넌트.
/// PlayerController의 상태를 읽어 Animator 파라미터를 갱신한다.
/// Animator가 스프라이트 스왑 애니메이션 클립을 재생한다.
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

    private Animator         _animator;
    private SpriteRenderer   _spriteRenderer;
    private PlayerController _playerController;

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
        UpdateAnimatorParams();
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

    private void HandlePlayerHit()
    {
        _animator.SetTrigger(ParamHit);
    }

    private void HandleGameStateChanged(GameState newState)
    {
        if (newState == GameState.Ready)
        {
            _spriteRenderer.color = Color.white;
        }
    }
}
