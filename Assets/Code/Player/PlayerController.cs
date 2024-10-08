using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Rigidbody2D 참조
    private new Rigidbody2D rigidbody2D;

    // PlayerState 참조
    private PlayerState playerState;

    // PlayerAnimationController 참조
    public PlayerAnimationController animationController;  // 애니메이션 컨트롤러 참조 추가

    // 점프
    public float groundCheckDistance;
    public LayerMask platformLayer;
    public LayerMask platformCanDownLayer;
    public float disableColliderTime = 0.5f;

    // 2단 점프
    public int maxJumpCount = 2;
    private int JumpCount = 0;

    // 스프라이트 방향
    public float horizontalInput;
    private bool isFacingRight = true;

    // 근접 공격
    private float curTime;
    public Transform pos;
    public Vector2 boxSize;
    public bool isAttacking = false;  // 공격 중인지 확인

    // 원거리 공격
    [SerializeField] GameObject bulletPref;
    private float lastFireTime = 0;
    [SerializeField] Transform firePoint;  // 투사체 발사 위치
    [SerializeField] Vector3 bulletScale = new Vector3(0.5f, 0.5f, 0.5f); // 투사체 크기


    void Start()
    {
        // PlayerState 컴포넌트
        playerState = GetComponent<PlayerState>();

        // Rigidbody2D 컴포넌트
        rigidbody2D = GetComponent<Rigidbody2D>();

        // PlayerAnimationController 컴포넌트
        animationController = GetComponent<PlayerAnimationController>();
    }

    void Update()
    {
        // 방향키 입력 (좌우)
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // 스프라이트 방향 변경
        FlipSprite();

        // 하단 점프
        if (Input.GetKey(KeyCode.DownArrow) && Input.GetKeyDown(KeyCode.Space))
        {
            CheckAndPassThroughPlatform();
        }

        // 점프
        else if (Input.GetKeyDown(KeyCode.Space) && JumpCount > 0)
        {
            Jump();
        }

        // 바닥에 닿았는지 확인, 점프 횟수 초기화
        if (IsGrounded())
        {
            JumpCount = maxJumpCount;
        }

        // 버튼 입력시 쿨타임이 없으면 근접 공격
        if (curTime <= 0)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                MeleeAttack();
            }
        }
        else
        {
            curTime -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Fire();
        }
    }

    void FixedUpdate()
    {
        // 공격 중에도 이동 가능
        if (horizontalInput != 0)
        {
            Vector2 movement = new Vector2(horizontalInput * playerState.moveSpeed, rigidbody2D.velocity.y);
            rigidbody2D.velocity = movement;
        }
        else
        {
            ApplyDeceleration();
        }
    }

    // 스프라이트 방향 변경
    void FlipSprite()
    {
        if (horizontalInput < 0 && isFacingRight)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            isFacingRight = false;
        }
        else if (horizontalInput > 0 && !isFacingRight)
        {
            transform.localScale = new Vector3(1, 1, 1);
            isFacingRight = true;
        }
    }

    // 감속 적용
    void ApplyDeceleration()
    {
        Vector2 velocity = rigidbody2D.velocity;
        velocity.x = Mathf.Lerp(velocity.x, 0, playerState.decelerationRate * Time.fixedDeltaTime);
        rigidbody2D.velocity = velocity;
    }

    // 점프
    void Jump()
    {
        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);
        rigidbody2D.velocity += Vector2.up * playerState.jumpPower;
        JumpCount--;
    }

    // 바닥에 있는지 확인
    bool IsGrounded()
    {
        LayerMask combinedLayerMask = platformLayer | platformCanDownLayer;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, combinedLayerMask);
        return hit.collider != null;
    }

    void CheckAndPassThroughPlatform()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, platformCanDownLayer);

        if (hit.collider != null)
        {
            StartCoroutine(DisablePlatformCollider(hit.collider));
        }
    }

    IEnumerator DisablePlatformCollider(Collider2D platformCollider)
    {
        platformCollider.enabled = false;
        yield return new WaitForSeconds(disableColliderTime);
        platformCollider.enabled = true;
    }

    // 근접 공격
    void MeleeAttack()
    {
        isAttacking = true;

        // 상체에 근접 공격 애니메이션 실행 (트랙 1)
        animationController.PlayMeleeAttackAnimation();

        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(pos.position, boxSize, 0);
        foreach (Collider2D collider in collider2Ds)
        {
            if (collider.tag == "Enemy")
                Debug.Log("MeleeAttack Hit: " + collider.name);
        }

        curTime = playerState.meleeAttackCoolTime;
    }

    // 원거리 공격
    void Fire()
    {
        if (Time.time - lastFireTime >= playerState.bulletFireDelay)
        {
            isAttacking = true;

            // 원거리 공격 애니메이션 실행
            animationController.PlayRangedAttackAnimation();

            // firePoint 위치에서 발사
            Vector3 direction = transform.right * transform.localScale.x;
            GameObject bullet = Instantiate(bulletPref, firePoint.position, Quaternion.identity);

            float bulletDirection = transform.localScale.x > 0 ? 1f : -1f;

            // 플레이어 방향에 따라 반전
            bullet.transform.localScale = new Vector3(bulletScale.x * bulletDirection, bulletScale.y, bulletScale.z);

            bullet.GetComponent<Rigidbody2D>().AddForce(direction * playerState.bulletFirePower, ForceMode2D.Impulse);

            lastFireTime = Time.time;
        }
    }

    // Raycast 표시 (디버깅)
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(pos.position, boxSize);
    }
}
