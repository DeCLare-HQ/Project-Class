using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Rigidbody2D  참조
    private new Rigidbody2D rigidbody2D;

    // PlayerState 참조
    private PlayerState playerState;

    // 점프
    public float groundCheckDistance; // 바닥을 감지할 거리
    public LayerMask platformLayer; // 점프할 수 있는 레이어
    public LayerMask platformCanDownLayer; // 하단 점프할 수 있는 레이어
    public float disableColliderTime; // Collider 비활성화 시간

    //2단 점프
    public int maxJumpCount = 2; // 최대 점프 횟수
    private int JumpCount = 0;   // 현재 남은 점프 횟수

    // 스프라이트 방향
    private float horizontalInput;     // 이동 입력값 저장
    private bool isFacingRight = true; // 스프라이트 방향 제어

    void Start()
    {
        // PlayerState 컴포넌트
        playerState = GetComponent<PlayerState>();

        // Rigidbody2D 컴포넌트
        rigidbody2D = GetComponent<Rigidbody2D>();
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
            JumpCount = maxJumpCount; // 2단 점프 횟수 초기화
        }
    }

    void FixedUpdate()
    {
        if (horizontalInput != 0)
        {
            // 플레이어가 입력을 받을 때 이동 처리
            Vector2 movement = new Vector2(horizontalInput * playerState.moveSpeed, rigidbody2D.velocity.y);
            rigidbody2D.velocity = movement;
        }
        else
        {
            // 입력이 없을 때 감속 적용
            ApplyDeceleration();
        }
    }

    // 스프라이트 방향 변경
    void FlipSprite()
    {
        // 왼쪽으로 이동
        if (horizontalInput < 0 && isFacingRight)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            isFacingRight = false;
        }
        // 오른쪽으로 이동
        else if (horizontalInput > 0 && !isFacingRight)
        {
            transform.localScale = new Vector3(1, 1, 1);
            isFacingRight = true;
        }
    }

    // 감속 적용
    void ApplyDeceleration()
    {
        // 현재 x축 속도를 서서히 감소
        Vector2 velocity = rigidbody2D.velocity;
        velocity.x = Mathf.Lerp(velocity.x, 0, playerState.decelerationRate * Time.fixedDeltaTime);
        rigidbody2D.velocity = velocity;
    }

    // 점프
    void Jump()
    {
        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0); // 기존 y 속도 초기화
        rigidbody2D.velocity += Vector2.up * playerState.jumpPower;  // 새로운 힘 적용
        JumpCount--; // 점프할 때마다 점프 횟수 감소
    }

    // 바닥에 있는지 확인
    bool IsGrounded()
    {
        // 레이어 종류 감지
        LayerMask combinedLayerMask = platformLayer | platformCanDownLayer;

        // 플레이어 발 밑으로 Raycast
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, combinedLayerMask);

        // Platform 레이어에 맞으면 true를 반환
        return hit.collider != null;
    }

    // Raycast 표시 (디버깅)
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }

    // 하단 점프 가능한 바닥 확인
    void CheckAndPassThroughPlatform()
    {
        // 플레이어 발 밑으로 Raycast
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, platformCanDownLayer);

        // PlatformCanDown 레이어에 닿았을 경우 Collider 비활성화
        if (hit.collider != null)
        {
            StartCoroutine(DisablePlatformCollider(hit.collider));
        }
    }

    // 일정 시간 Collider를 비활성화하여 플랫폼 통과
    IEnumerator DisablePlatformCollider(Collider2D platformCollider)
    {
        // Collider 비활성화
        platformCollider.enabled = false;

        // 일정 시간 기다림
        yield return new WaitForSeconds(disableColliderTime);

        // Collider 활성화
        platformCollider.enabled = true;
    }
}
