using System.Collections;
using UnityEngine;
using Spine.Unity;

public class EnemyRange01 : MonoBehaviour
{
    public Transform detectionPoint; // 플레이어 감지 기준점
    public Transform firePoint; // 발사체 생성 위치
    public LayerMask playerLayer; // 플레이어 감지 레이어
    public Vector2 detectionBoxSize = new Vector2(15, 5f); // 감지 박스 크기
    public GameObject bulletPrefab; // 발사체 프리팹
    public float attackDelay = 0.5f; // 공격 대기 시간
    public float throwDelay = 0.7f; // 투사체 대기 시간
    private EnemyRange01AnimationController animationController; // Spine 애니메이션 컨트롤러
    private bool isFacingLeft = true; // 기본 방향
    private bool isAttacking = false; // 현재 공격 상태 여부
    private bool isHurt = false; // 피격 상태 여부
    private float currentHealth; // 현재 체력
    private Color originalColor; // 기본 색상
    private SkeletonAnimation skeletonAnimation; // Spine 애니메이션
    private EnemyItemDrop itemDrop; // 아이템 드롭 관리 스크립트

    private StateEnemyRange01 state; // 능력치 관리 스크립트

    private void Start()
    {
        // 컴포넌트 초기화
        animationController = GetComponent<EnemyRange01AnimationController>();
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        itemDrop = GetComponent<EnemyItemDrop>();
        state = GetComponent<StateEnemyRange01>();

        // 초기 상태 설정
        currentHealth = state.health; // 초기 체력을 설정
        originalColor = skeletonAnimation.skeleton.GetColor(); // Spine 애니메이션 기본 색상 저장
    }

    private void Update()
    {
        // 피격 상태가 아닌 경우에만 동작
        if (!isHurt)
        {
            bool playerInRange = DetectPlayer(); // 플레이어 감지 여부 확인
            if (playerInRange && !isAttacking) // 플레이어가 감지되었고 공격 중이 아닐 경우
            {
                StartCoroutine(HandleAttack()); // 공격 실행
            }
        }
    }

    // 플레이어 감지
    private bool DetectPlayer()
    {
        // 감지 박스를 통해 충돌한 오브젝트 확인
        Collider2D[] hits = Physics2D.OverlapBoxAll(detectionPoint.position, detectionBoxSize, 0f, playerLayer);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player")) // 감지된 충돌체가 플레이어인지 확인
            {
                FlipSprite(hit.transform.position.x); // 적이 플레이어를 바라보도록 방향 전환
                return true; // 플레이어 감지
            }
        }
        return false; // 플레이어 미감지
    }

    // 스프라이트 방향 전환
    private void FlipSprite(float playerX)
    {
        bool shouldFaceLeft = playerX > transform.position.x; // 플레이어 위치에 따라 방향 결정

        if (isFacingLeft != shouldFaceLeft) // 방향이 현재와 다를 경우 반전
        {
            isFacingLeft = shouldFaceLeft;

            // 스프라이트의 x 스케일 값을 변경하여 반전
            Vector3 theScale = transform.localScale;
            theScale.x = Mathf.Abs(theScale.x) * (isFacingLeft ? 1 : -1);
            transform.localScale = theScale;
        }
    }

    // 공격 처리
    private IEnumerator HandleAttack()
    {
        isAttacking = true; // 공격 상태 설정

        animationController.PlayIdleAnimation(); // Idle 애니메이션 실행
        yield return new WaitForSeconds(attackDelay); // 공격 대기 시간 동안 대기

        animationController.PlayAttackAnimation(); // 공격 애니메이션 실행

        yield return new WaitForSeconds(throwDelay); // 공격 대기 시간 동안 대기
            FireProjectile(); // 투사체 발사

        yield return new WaitUntil(() => animationController.IsAnimationComplete()); // 애니메이션 종료 대기
        isAttacking = false; // 공격 상태 해제
    }

    // 투사체 발사
    private void FireProjectile()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogError("Bullet Prefab 또는 FirePoint가 설정되지 않았습니다.");
            return;
        }

        // 발사체 생성
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // 발사 방향 설정
        float direction = isFacingLeft ? 1f : -1f;

        // 발사체 크기와 방향 설정
        bullet.transform.localScale = new Vector3(
            bullet.transform.localScale.x * direction,
            bullet.transform.localScale.y,
            bullet.transform.localScale.z
        );

        // Rigidbody2D로 힘을 가해 발사
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            bulletRb.AddForce(new Vector2(direction, 0) * 10f, ForceMode2D.Impulse);
        }

        // 공격력 설정
        EnemyBullet enemyBullet = bullet.GetComponent<EnemyBullet>();
        if (enemyBullet != null)
        {
            enemyBullet.SetDirection(new Vector2(direction, 0));
            enemyBullet.SetAttackPower(state.attackPower);
        }
    }

    // 물리 공격에 맞았을 때 호출
    public void TakeDamage(float physicalDamage)
    {
        float damage = Mathf.Max(0, physicalDamage - state.physicalDefense); // 방어력을 고려한 데미지 계산
        StartCoroutine(HurtRoutine(Color.red, damage)); // 피격 처리 코루틴 실행 (빨간색 표시)
    }

    // 마법 공격에 맞았을 때 호출
    public void TakeDamageFromProjectile(float magicDamage)
    {
        float damage = Mathf.Max(0, magicDamage - state.magicDefense); // 방어력을 고려한 데미지 계산
        StartCoroutine(HurtRoutine(Color.blue, damage)); // 피격 처리 코루틴 실행 (파란색 표시)
    }

    // 피격 처리 코루틴
    private IEnumerator HurtRoutine(Color hurtColor, float damage)
    {
        isHurt = true; // 피격 상태 설정

        skeletonAnimation.skeleton.SetColor(hurtColor); // 피격 상태 색상 변경
        currentHealth -= damage; // 체력 감소

        Debug.Log($"적이 받은 데미지: {damage}, 남은 체력: {currentHealth}");

        if (currentHealth <= 0) // 체력이 0 이하이면 사망 처리
        {
            Debug.Log("적 사망");
            itemDrop.HandleEnemyDeath(); // 아이템 드롭 처리
            Destroy(gameObject); // 적 제거
            yield break;
        }

        animationController.PlayHurtAnimation(); // 피격 애니메이션 실행
        yield return new WaitUntil(() => animationController.IsAnimationComplete()); // 애니메이션 완료 대기

        skeletonAnimation.skeleton.SetColor(originalColor); // 색상 복구
        isHurt = false; // 피격 상태 해제
    }

    // 감지 박스 시각화
    private void OnDrawGizmosSelected()
    {
        if (detectionPoint != null)
        {
            Gizmos.color = Color.red; // 감지 박스 색상 설정
            Gizmos.DrawWireCube(detectionPoint.position, detectionBoxSize); // 감지 박스 그리기
        }
        // FirePoint 시각화
        if (firePoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(firePoint.position, 0.2f);
        }
    }
}