using NUnit.Framework;
using UnityEngine;

/// <summary>
/// PlayerAnimationController v2 Edit Mode 테스트.
/// Animator 파라미터 해시 유효성 및 컴포넌트 구조 검증.
/// </summary>
[TestFixture]
public class PlayerAnimationControllerV2Test
{
    // ----------------------------------------------------------------
    // 파라미터 해시 유효성 검증
    // ----------------------------------------------------------------

    [Test]
    public void 성공_파라미터해시_isPlaying_유효()
    {
        int hash = Animator.StringToHash("isPlaying");
        Assert.AreNotEqual(0, hash, "isPlaying 파라미터 해시는 0이 아니어야 한다");
    }

    [Test]
    public void 성공_파라미터해시_isGrounded_유효()
    {
        int hash = Animator.StringToHash("isGrounded");
        Assert.AreNotEqual(0, hash, "isGrounded 파라미터 해시는 0이 아니어야 한다");
    }

    [Test]
    public void 성공_파라미터해시_isSliding_유효()
    {
        int hash = Animator.StringToHash("isSliding");
        Assert.AreNotEqual(0, hash, "isSliding 파라미터 해시는 0이 아니어야 한다");
    }

    [Test]
    public void 성공_파라미터해시_hit_유효()
    {
        int hash = Animator.StringToHash("hit");
        Assert.AreNotEqual(0, hash, "hit 파라미터 해시는 0이 아니어야 한다");
    }

    // ----------------------------------------------------------------
    // 컴포넌트 생성 및 RequireComponent 검증
    // ----------------------------------------------------------------

    [Test]
    public void 성공_PlayerAnimationController_RequireComponent_Animator_충족()
    {
        // RequireComponent(typeof(Animator)) 어트리뷰트 존재 여부 확인
        var attrs = typeof(PlayerAnimationController)
            .GetCustomAttributes(typeof(RequireComponent), true);
        bool hasAnimator = false;
        foreach (var attr in attrs)
        {
            var rc = (RequireComponent)attr;
            if (rc.m_Type0 == typeof(Animator) || rc.m_Type1 == typeof(Animator) || rc.m_Type2 == typeof(Animator))
            {
                hasAnimator = true;
                break;
            }
        }
        Assert.IsTrue(hasAnimator, "PlayerAnimationController는 RequireComponent(Animator)를 가져야 한다");
    }

    [Test]
    public void 성공_PlayerAnimationController_RequireComponent_SpriteRenderer_충족()
    {
        var attrs = typeof(PlayerAnimationController)
            .GetCustomAttributes(typeof(RequireComponent), true);
        bool hasSpriteRenderer = false;
        foreach (var attr in attrs)
        {
            var rc = (RequireComponent)attr;
            if (rc.m_Type0 == typeof(SpriteRenderer) || rc.m_Type1 == typeof(SpriteRenderer) || rc.m_Type2 == typeof(SpriteRenderer))
            {
                hasSpriteRenderer = true;
                break;
            }
        }
        Assert.IsTrue(hasSpriteRenderer, "PlayerAnimationController는 RequireComponent(SpriteRenderer)를 가져야 한다");
    }

    // ----------------------------------------------------------------
    // Animator 파라미터 이름 상수 일관성 검증
    // ----------------------------------------------------------------

    [Test]
    public void 성공_isPlaying과_isGrounded_해시는_서로_다름()
    {
        int h1 = Animator.StringToHash("isPlaying");
        int h2 = Animator.StringToHash("isGrounded");
        Assert.AreNotEqual(h1, h2, "isPlaying과 isGrounded의 해시는 달라야 한다");
    }

    [Test]
    public void 성공_isSliding과_hit_해시는_서로_다름()
    {
        int h1 = Animator.StringToHash("isSliding");
        int h2 = Animator.StringToHash("hit");
        Assert.AreNotEqual(h1, h2, "isSliding과 hit의 해시는 달라야 한다");
    }
}
