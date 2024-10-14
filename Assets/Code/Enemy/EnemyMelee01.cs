using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class EnemyMelee01 : MonoBehaviour
{
    public LayerMask groundLayer; // 바닥 감지 레이어
    public float groundCheckDistance = 2f; // 바닥 감지 Raycast 거리
    private bool isGrounded = true; // 바닥 위에 있는지 여부
    private bool isFacingLeft = true; // 스프라이트 기본 방향이 왼쪽이면 true로 설정
    private bool isAtEdge = false; // 낭떠러지에 도달했는지 여부

    private bool isHurt = false; // 피격 상태 여부 확인
    private bool playerInRange = false; // 플레이어가 감지 범위 안에 있는지 여부 확인

    public Vector2 detectionBoxSize = new Vector2(2f, 2f); // 감지 범위 크기 (가로, 세로)
    public Transform detectionPoint; // 감지 범위의 중심이 될 빈 오브젝트
    public LayerMask playerLayer; // 플레이어 감지 레이어

    private StateEnemyMelee01 enemyState; // 능력치 참조
    private EnemyMelee01AnimationController enemyAnimationController; // 애니메이션 컨트롤러 참조
    private SkeletonAnimation skeletonAnimation; // SkeletonAnimation 참조

    private Color originalColor; // 원래 색상 저장
    private float currentHealth; // 현재 체력


    private void Start()
    {
        enemyState = GetComponent<StateEnemyMelee01>();
        enemyAnimationController = GetComponent<EnemyMelee01AnimationController>();
        skeletonAnimation = GetComponent<SkeletonAnimation>(); // SkeletonAnimation 컴포넌트 참조 추가

        // 기본 색상 저장
        originalColor = skeletonAnimation.skeleton.GetColor(); // Spine의 기본 색상 저장

        isFacingLeft = false; // 기본 스프라이트가 오른쪽을 바라본다고 가정
        currentHealth = enemyState.health; // 최대 체력으로 초기화
    }

    private void Update()
    {
        if (!isHurt && !isAtEdge) // 피격 상태나 낭떠러지 상태가 아닐 때만 이동
        {
            Move(); // 이동 실행
        }

        // 플레이어 감지 체크
        playerInRange = DetectPlayer();

        if (playerInRange)
        {
            Debug.Log("플레이어 감지됨!");
        }
    }

    // 플레이어 감지
    private bool DetectPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(detectionPoint.position, detectionBoxSize, 0f, playerLayer);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                return true; // 플레이어가 감지됨
            }
        }
        return false; // 플레이어 감지되지 않음
    }

    // 디버깅용 감지 범위 시각화
    private void OnDrawGizmosSelected()
    {
        if (detectionPoint == null) return;

        // 감지 범위를 시각적으로 표시
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(detectionPoint.position, detectionBoxSize);
    }

    // 이동
    private void Move()
    {
        // 바닥 감지
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);

        if (isGrounded)
        {
            // 좌/우 계속 이동
            if (isFacingLeft)
            {
                transform.Translate(Vector2.left * enemyState.moveSpeed * Time.deltaTime);
                enemyAnimationController.PlayWalkAnimation(); // 걷기 애니메이션 실행

                // 낭떠러지 감지
                if (!Physics2D.Raycast(transform.position + new Vector3(-0.5f, 0, 0), Vector2.down, groundCheckDistance, groundLayer))
                {
                    StartCoroutine(HandleEdgeDetection()); // 낭떠러지에 도달했을 때 처리
                }
            }
            else
            {
                transform.Translate(Vector2.right * enemyState.moveSpeed * Time.deltaTime);
                enemyAnimationController.PlayWalkAnimation(); // 걷기 애니메이션 실행

                // 낭떠러지 감지
                if (!Physics2D.Raycast(transform.position + new Vector3(0.5f, 0, 0), Vector2.down, groundCheckDistance, groundLayer))
                {
                    StartCoroutine(HandleEdgeDetection()); // 낭떠러지에 도달했을 때 처리
                }
            }
        }
        else
        {
            // 만약 바닥이 아닌 경우, Idle 애니메이션 실행
            enemyAnimationController.PlayIdleAnimation();
        }
    }

    // 낭떠러지 감지 시 처리
    private IEnumerator HandleEdgeDetection()
    {
        isAtEdge = true; // 낭떠러지 감지
        enemyAnimationController.PlayIdleAnimation(); // Idle 애니메이션 실행

        // Idle 애니메이션이 끝나기 전까지 잠시 대기
        yield return new WaitForSeconds(1.0f); // 1초 대기 후 방향 전환

        Flip(); // 방향 전환
        isAtEdge = false; // 다시 이동 가능 상태로 설정
    }

    // 방향 전환
    private void Flip()
    {
        isFacingLeft = !isFacingLeft;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    // 근접 공격에 맞았을 때 호출 (물리 공격력 - 물리 방어력)
    public void TakeDamage(float physicalDamage)
    {
        StartCoroutine(HurtRoutine(Color.red, physicalDamage)); // 빨간색으로 변경
    }

    // 원거리 공격에 맞았을 때 호출 (마법 공격력 - 마법 방어력)
    public void TakeDamageFromProjectile(float magicDamage)
    {
        StartCoroutine(HurtRoutine(Color.blue, magicDamage)); // 파란색으로 변경
    }

    // 피격 처리 코루틴
    private IEnumerator HurtRoutine(Color hurtColor, float damage)
    {
        isHurt = true; // 피격 상태 설정

        // 색상 변경
        skeletonAnimation.skeleton.SetColor(hurtColor);

        // 데미지 계산 및 체력 감소
        currentHealth -= damage;
        Debug.Log("적이 받은 데미지: " + damage);
        Debug.Log("적의 남은 체력: " + currentHealth);

        // 체력이 0 이하이면 죽음 처리
        if (currentHealth <= 0)
        {
            Debug.Log("죽음");
            Destroy(gameObject);
            yield break; // 죽음 시 코루틴 종료
        }

        // hurt 애니메이션 재생
        enemyAnimationController.PlayHurtAnimation();

        // 애니메이션이 끝날 때까지 대기
        yield return new WaitUntil(() => enemyAnimationController.IsAnimationComplete());

        // 원래 색으로 복원 (SkeletonAnimation의 색상 복원)
        skeletonAnimation.skeleton.SetColor(originalColor);

        isHurt = false; // 피격 상태 해제
    }
}
