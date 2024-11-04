using UnityEngine;
using Spine.Unity;

public class PlayerAnimationController : MonoBehaviour
{
    private SkeletonAnimation skeletonAnimation;
    private PlayerState playerState;
    private PlayerController playerController;

    private string currentLowerBodyAnimation;  // 하체 애니메이션 추적
    private string currentUpperBodyAnimation;  // 상체 애니메이션 추적

    private const string WalkAnimation = "walk";
    private const string IdleAnimation = "idle";
    private const string MeleeAttackAnimation = "attack_melee";
    private const string RangedAttackAnimation = "attack_range";

    private bool isAttacking = false;

    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        playerState = GetComponent<PlayerState>();
        playerController = GetComponent<PlayerController>();

        if (skeletonAnimation == null)
        {
            Debug.LogError("SkeletonAnimation 컴포넌트가 없습니다. 확인해주세요.");
        }

        // 애니메이션 완료될 때 호출되는 이벤트 처리
        skeletonAnimation.state.Complete += OnAnimationComplete;

        // Idle 애니메이션으로 시작
        SetLowerBodyAnimation(IdleAnimation, true);
    }

    void Update()
    {
        if (!isAttacking)  // 공격 중이 아닐 때만 하체 애니메이션 전환
        {
            UpdateMovementAnimation();
        }
    }

    void UpdateMovementAnimation()
    {
        // 하체 트랙 (트랙 0)에 Walk 또는 Idle 애니메이션 실행
        if (Mathf.Abs(playerController.horizontalInput) > 0)
        {
            SetLowerBodyAnimation(WalkAnimation, true);  // 하체 애니메이션을 트랙 0에 실행
        }
        else
        {
            SetLowerBodyAnimation(IdleAnimation, true);  // 하체 애니메이션을 트랙 0에 실행
        }
    }

    // 하체 애니메이션 실행
    public void SetLowerBodyAnimation(string animationName, bool loop)
    {
        if (currentLowerBodyAnimation == animationName) return;  // 동일 애니메이션 중복 실행 방지

        skeletonAnimation.state.SetAnimation(0, animationName, loop);  // 트랙 0에서 실행 (하체)
        currentLowerBodyAnimation = animationName;
    }

    // 근접 공격 애니메이션 실행 (트랙 1)
    public void PlayMeleeAttackAnimation()
    {
        isAttacking = true;
        SetUpperBodyAnimation(MeleeAttackAnimation, false);  // 트랙 1에서 실행 (상체)
    }

    // 원거리 공격 애니메이션 실행 (트랙 1)
    public void PlayRangedAttackAnimation()
    {
        isAttacking = true;
        SetUpperBodyAnimation(RangedAttackAnimation, false);  // 트랙 1에서 실행 (상체)
    }

    // 상체 애니메이션 실행
    public void SetUpperBodyAnimation(string animationName, bool loop)
    {
        skeletonAnimation.state.SetAnimation(1, animationName, loop);  // 트랙 1에서 실행 (상체)
        currentUpperBodyAnimation = animationName;
    }

    // 애니메이션 완료 이벤트 처리
    private void OnAnimationComplete(Spine.TrackEntry trackEntry)
    {
        // 상체 공격 애니메이션 완료 시 다시 하체 애니메이션과 동기화
        if (trackEntry.TrackIndex == 1)  // 상체 트랙(트랙 1)에서 공격 애니메이션 완료 시
        {
            isAttacking = false;

            // 상체 트랙(트랙 1)을 비우고, 하체 애니메이션과 동기화
            skeletonAnimation.state.SetEmptyAnimation(1, 0);
        }
    }
}
