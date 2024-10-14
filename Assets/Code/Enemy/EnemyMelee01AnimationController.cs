using UnityEngine;
using Spine.Unity;
using Spine;

public class EnemyMelee01AnimationController : MonoBehaviour
{
    private SkeletonAnimation skeletonAnimation; // Spine 애니메이션 컴포넌트 참조
    private string currentAnimation; // 현재 실행 중인 애니메이션 추적

    private const string IdleAnimation = "Idle"; // Idle 애니메이션 이름
    private const string WalkAnimation = "walk"; // Walk 애니메이션 이름
    private const string HurtAnimation = "hurt"; // hurt 애니메이션 이름
    private const string AttackAnimation = "attack"; // attack 애니메이션 이름


    private bool isAnimationComplete = false; // 애니메이션 완료 여부 확인

    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();

        if (skeletonAnimation == null)
        {
            Debug.LogError("SkeletonAnimation 컴포넌트가 없습니다. 확인해주세요.");
        }

        // Spine 애니메이션 완료 이벤트 구독
        skeletonAnimation.state.Complete += OnAnimationComplete;

        PlayIdleAnimation(); // Idle 애니메이션으로 시작
    }

    // Idle 애니메이션 실행
    public void PlayIdleAnimation()
    {
        SetAnimation(IdleAnimation, true); // Idle 애니메이션을 루프 실행
    }

    // Walk 애니메이션 실행
    public void PlayWalkAnimation()
    {
        SetAnimation(WalkAnimation, true); // Walk 애니메이션을 루프 실행
    }

    // Hurt 애니메이션 실행
    public void PlayHurtAnimation()
    {
        isAnimationComplete = false; // 애니메이션 완료 여부 초기화
        SetAnimation(HurtAnimation, false); // Hurt 애니메이션을 한 번 실행
    }

    // Attack 애니메이션 실행
    public void PlayAttackAnimation()
    {
        isAnimationComplete = false; // 애니메이션 완료 여부 초기화
        SetAnimation(AttackAnimation, false); // Attack 애니메이션을 한 번 실행
    }

    // 애니메이션이 완료되었을 때 호출되는 메서드
    private void OnAnimationComplete(TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == HurtAnimation) // Hurt 애니메이션이 끝났을 때만
        {
            isAnimationComplete = true; // 애니메이션이 끝났음을 표시
        }
    }

    // 애니메이션을 설정하는 메서드
    private void SetAnimation(string animationName, bool loop)
    {
        if (currentAnimation == animationName) return; // 동일 애니메이션 중복 실행 방지

        skeletonAnimation.state.SetAnimation(0, animationName, loop); // 트랙 0에서 애니메이션 실행
        currentAnimation = animationName; // 현재 애니메이션 상태 업데이트
    }

    // 애니메이션 완료 여부 반환
    public bool IsAnimationComplete()
    {
        return isAnimationComplete; // 애니메이션 완료 여부 반환
    }
}
