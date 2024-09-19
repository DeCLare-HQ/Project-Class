using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee01 : MonoBehaviour
{
    public float moveSpeed = 2f; // 이동 속도
    public float attackRange = 1f; // 공격 거리
    public float attackDelay = 1f; // 공격 지연 시간
    public LayerMask groundLayer; // 바닥 감지
    public float groundCheckDistance = 2f; // 바닥 감지 Raycast 거리
    private bool isGrounded = true; // 바닥에 있는지 여부
    private bool isFacingLeft = true; // 이동 방향
    private bool isAttacking = false; // 공격 중 여부

    private Transform player; // 플레이어 위치 참조
    private Animator animator; // 애니메이션 컨트롤러

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // 플레이어 탐지
        animator = GetComponent<Animator>(); // 애니메이터 설정
    }

    private void Update()
    {
        if (!isAttacking)
        {
            Move();
            CheckAttack();
        }
    }

    // 이동 로직
    private void Move()
    {
        // 바닥 감지
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        
        if (isGrounded)
        {
            // 좌/우로 계속 이동
            if (isFacingLeft)
            {
                transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
                // 앞쪽 낭떠러지 감지
                if (!Physics2D.Raycast(transform.position + new Vector3(-0.5f, 0, 0), Vector2.down, groundCheckDistance, groundLayer))
                {
                    Flip();
                }
            }
            else
            {
                transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
                // 앞쪽 낭떠러지 감지
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

    // 공격 거리 진입 체크 후 공격 시도
    private void CheckAttack()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            StartCoroutine(Attack());
        }
    }

    // 공격 루틴
    private IEnumerator Attack()
    {
        isAttacking = true;
        animator.SetTrigger("Attack"); // 공격 애니메이션 트리거
        yield return new WaitForSeconds(attackDelay); // 공격 지연
        // 실제 공격 로직
        isAttacking = false;
    }
}