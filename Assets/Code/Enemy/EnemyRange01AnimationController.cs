using UnityEngine;
using Spine.Unity;
using Spine;

public class EnemyRange01AnimationController : MonoBehaviour
{
    private SkeletonAnimation skeletonAnimation; // Spine 애니메이션 컴포넌트
    private string currentAnimation; // 현재 실행 중인 애니메이션 이름
    private bool isAnimationComplete = false; // 애니메이션 완료 여부

    private const string IdleAnimation = "Idle"; // Idle 애니메이션 이름
    private const string AttackAnimation = "attack"; // Attack 애니메이션 이름
    private const string HurtAnimation = "hurt"; // Hurt 애니메이션 이름

    void Start()
    {
        // SkeletonAnimation 컴포넌트 초기화
        skeletonAnimation = GetComponent<SkeletonAnimation>();

        // Spine 애니메이션 완료 이벤트를 구독하여 애니메이션 완료 시 호출
        skeletonAnimation.state.Complete += OnAnimationComplete;

        // 기본 애니메이션으로 Idle 애니메이션 실행
        PlayIdleAnimation();
    }

    // Idle
    public void PlayIdleAnimation()
    {
        SetAnimation(IdleAnimation, true); // Idle 애니메이션을 루프 실행
    }

    // Attack
    public void PlayAttackAnimation()
    {
        isAnimationComplete = false; // 애니메이션 완료 상태 초기화
        SetAnimation(AttackAnimation, false); // Attack 애니메이션을 한 번 실행
    }

    // Hurt
    public void PlayHurtAnimation()
    {
        isAnimationComplete = false; // 애니메이션 완료 상태 초기화
        SetAnimation(HurtAnimation, false); // Hurt 애니메이션을 한 번 실행
    }

    // Spine 애니메이션 완료 이벤트 핸들러
    private void OnAnimationComplete(TrackEntry trackEntry)
    {
        // Attack 또는 Hurt 애니메이션 완료 시 완료 상태를 true로 설정
        if (trackEntry.Animation.Name == AttackAnimation || trackEntry.Animation.Name == HurtAnimation)
        {
            isAnimationComplete = true;
        }
    }

    // Spine 애니메이션 실행 메서드
    private void SetAnimation(string animationName, bool loop)
    {
        // 현재 애니메이션이 실행 중이면 다시 실행하지 않음
        if (currentAnimation == animationName) return;

        // Spine 애니메이션 상태 업데이트 및 실행
        skeletonAnimation.state.SetAnimation(0, animationName, loop);
        currentAnimation = animationName; // 현재 애니메이션 이름 저장
    }

    // 애니메이션 완료 여부 반환
    public bool IsAnimationComplete()
    {
        return isAnimationComplete; // 애니메이션 완료 여부 반환
    }
}
