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

    private StateEnemyMelee01 enemyState; // 능력치 참조
    private EnemyMelee01AnimationController enemyAnimationController; // 애니메이션 컨트롤러 참조

    private void Start()
    {
        enemyState = GetComponent<StateEnemyMelee01>();
        enemyAnimationController = GetComponent<EnemyMelee01AnimationController>(); // 애니메이션 컨트롤러 참조 추가

        isFacingLeft = false; // 기본 스프라이트가 오른쪽을 바라본다고 가정
    }

    private void Update()
    {
        if (!isAtEdge) // 낭떠러지에 도달하지 않았을 때만 이동
        {
            Move(); // 이동 실행
        }
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
}
