using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee01 : MonoBehaviour
{
    public LayerMask groundLayer; // 바닥 감지 레이어
    public float groundCheckDistance = 2f; // 바닥 감지 Raycast 거리
    private bool isGrounded = true; // 바닥 위에 있는지 여부
    private bool isFacingLeft = true; // 이동 방향

    private StateEnemyMelee01 enemyState; // 능력치 참조

    private void Start()
    {
        enemyState = GetComponent<StateEnemyMelee01>();
    }

    private void Update()
    {
        Move(); // 이동 실행
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
                // 낭떠러지 감지
                if (!Physics2D.Raycast(transform.position + new Vector3(-0.5f, 0, 0), Vector2.down, groundCheckDistance, groundLayer))
                {
                    Flip();
                }
            }
            else
            {
                transform.Translate(Vector2.right * enemyState.moveSpeed * Time.deltaTime);
                // 낭떠러지 감지
                if (!Physics2D.Raycast(transform.position + new Vector3(0.5f, 0, 0), Vector2.down, groundCheckDistance, groundLayer))
                {
                    Flip();
                }
            }
        }
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
