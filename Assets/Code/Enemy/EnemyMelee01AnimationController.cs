using UnityEngine;
using Spine.Unity;

public class EnemyMelee01AnimationController : MonoBehaviour
{
    private SkeletonAnimation skeletonAnimation; // Spine 애니메이션 컴포넌트 참조
    private string currentAnimation; // 현재 실행 중인 애니메이션 추적

    private const string IdleAnimation = "Idle"; // Idle 애니메이션 이름
    private const string WalkAnimation = "walk"; // Walk 애니메이션 이름

    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();

        if (skeletonAnimation == null)
        {
            Debug.LogError("SkeletonAnimation 컴포넌트가 없습니다. 확인해주세요.");
        }

        PlayIdleAnimation(); // Idle 애니메이션으로 시작
    }

    // Idle 애니메이션 실행
    public void PlayIdleAnimation()
    {
        SetAnimation(IdleAnimation, false); // Idle 애니메이션을 한 번 실행
    }

    // Walk 애니메이션 실행
    public void PlayWalkAnimation()
    {
        SetAnimation(WalkAnimation, true); // Walk 애니메이션을 루프 실행
    }

    // 애니메이션을 설정하는 메서드
    private void SetAnimation(string animationName, bool loop)
    {
        if (currentAnimation == animationName) return; // 동일 애니메이션 중복 실행 방지

        skeletonAnimation.state.SetAnimation(0, animationName, loop); // 트랙 0에서 애니메이션 실행
        currentAnimation = animationName; // 현재 애니메이션 상태 업데이트
    }
}
