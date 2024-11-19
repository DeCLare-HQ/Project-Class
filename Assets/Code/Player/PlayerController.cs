using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Rigidbody2D 참조
    private new Rigidbody2D rigidbody2D;

    // PlayerState 참조
    private PlayerState playerState;

    // PlayerAnimationController 참조
    public PlayerAnimationController animationController;

    // PlayerEffect 참조
    private PlayerEffect playerEffect;

    // Gameover 스크립트 참조
    private Gameover gameOverManager;

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
    public bool isAttacking = false;
    public float meleeAttackDelay = 0.1f;  // 공격 딜레이


    // 원거리 공격
    [SerializeField] GameObject bulletPref;
    private float lastFireTime = 0;
    [SerializeField] Transform firePoint;  // 투사체 발사 위치
    [SerializeField] Vector3 bulletScale = new Vector3(0.5f, 0.5f, 0.5f); // 투사체 크기
    public float rangeAttackDelay = 0.3f;  // 공격 딜레이

    // 무적 상태
    private bool isInvincible = false; // 무적 상태 여부
    private float invincibilityDuration = 1.0f; // 무적 상태 지속 시간
    private Color originalColor; // 원래 색상 저장
    private SkeletonAnimation skeletonAnimation; // Spine의 SkeletonAnimation 참조

    // 충돌 감지
    public LayerMask enemyLayerMask;  // 적의 레이어 마스크
    public string enemyTag = "Enemy"; // 적 태그
    public Transform detectionBoxTransform; // 충돌 감지를 위한 Empty Object의 Transform
    public Vector2 detectionBoxSize = new Vector2(1.0f, 2.0f); // 사각형 범위 크기

    void Start()
    {
        // PlayerState 컴포넌트
        playerState = GetComponent<PlayerState>();

        // Rigidbody2D 컴포넌트
        rigidbody2D = GetComponent<Rigidbody2D>();

        // PlayerAnimationController 컴포넌트
        animationController = GetComponent<PlayerAnimationController>();

        // PlayerEffect 컴포넌트
        playerEffect = GetComponent<PlayerEffect>();

        // SkeletonAnimation 컴포넌트
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        
        // Spine 캐릭터의 원본 색상 저장
        originalColor = skeletonAnimation.skeleton.GetColor();

        playerState = PlayerState.instance;  // PlayerState Singleton 참조
        InitializePlayerData();

        SceneManager.sceneLoaded += OnSceneLoaded; // 씬 로드 시 호출될 메서드 등록
        FindGameOverManager(); // 현재 씬에서 Gameover 매니저 찾기
    }

    void InitializePlayerData()
    {
        // PlayerState의 데이터를 기반으로 PlayerController의 데이터 동기화
        // 체력 및 회복약 수를 PlayerState에서 가져와 반영
        int currentHealth = playerState.currentHealth;
        int potionCount = playerState.potionCount;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // 씬 언로드 시 이벤트 해제
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Inventory.instance?.ToggleInventory(); // 인벤토리 토글
        }

        if (Inventory.instance != null && Inventory.instance.inventoryUI.activeSelf)
        {
            // 이전 슬롯 선택
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Inventory.instance.SelectPreviousSlot();
            }

            // 다음 슬롯 선택
            if (Input.GetKeyDown(KeyCode.W))
            {
                Inventory.instance.SelectNextSlot();
            }

            // E 키로 아이템 삭제
            if (Input.GetKeyDown(KeyCode.E))
            {
                Inventory.instance.RemoveSelectedItem();
            }
        }

        // 좌우 이동
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

        // 적과의 충돌 감지
        CheckForEnemyCollision();

        // 회복약 사용
        if (Input.GetKeyDown(KeyCode.E))
        {
            UsePotion();
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
        // 점프 이펙트 생성
        playerEffect.SpawnJumpEffect();

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

    // Raycast 바닥 확인
    void CheckAndPassThroughPlatform()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, platformCanDownLayer);

        if (hit.collider != null)
        {
            StartCoroutine(DisablePlatformCollider(hit.collider));
        }
    }

    // 하단 점프 콜라이더 해제
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

        // 근접 공격 이펙트 생성
        playerEffect.SpawnMeleeEffect();

        // 적에게 데미지를 줄 때 딜레이 적용
        StartCoroutine(DealDamageWithDelay(meleeAttackDelay));

        curTime = playerState.meleeAttackCoolTime; // 쿨타임 설정
    }

    // 근접 공격이 적에게 데미지를 주는 코드
    private IEnumerator DealDamageWithDelay(float delay)
    {
        // 딜레이 시간만큼 대기
        yield return new WaitForSeconds(delay);

        // 딜레이 후에 데미지를 적용
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(pos.position, boxSize, 0);
        foreach (Collider2D collider in collider2Ds)
        {
            if (collider.tag == "Enemy")
            {
                Debug.Log("MeleeAttack Hit: " + collider.name);

                // 적에게 데미지 전달
                EnemyMelee01 enemy = collider.GetComponent<EnemyMelee01>();
                if (enemy != null)
                {
                    // 플레이어의 물리 공격력에서 적의 물리 방어력을 뺀 데미지 계산
                    float damage = Mathf.Max(0, playerState.physicalAttackPower - enemy.GetComponent<StateEnemyMelee01>().physicalDefense);
                    enemy.TakeDamage(damage); // 적에게 데미지 전달
                }
            }
        }
    }

    // 원거리 공격
    void Fire()
    {
        if (Time.time - lastFireTime >= playerState.bulletFireDelay)
        {
            isAttacking = true;

            // 원거리 공격 애니메이션 실행
            animationController.PlayRangedAttackAnimation();

            // 원거리 공격 이펙트 생성
            playerEffect.SpawnRangedEffect();

            // 투사체 발사를 딜레이 후에 실행
            StartCoroutine(FireProjectileWithDelay(rangeAttackDelay));

            lastFireTime = Time.time;
        }
    }

    // 투사체 발사를 딜레이 후에 실행하는 코루틴
    IEnumerator FireProjectileWithDelay(float delay)
    {
        // 지정된 딜레이 시간만큼 대기
        yield return new WaitForSeconds(delay);

        // firePoint 위치에서 발사
        Vector3 direction = transform.right * transform.localScale.x;
        GameObject bullet = Instantiate(bulletPref, firePoint.position, Quaternion.identity);

        float bulletDirection = transform.localScale.x > 0 ? 1f : -1f;

        // 플레이어 방향에 따라 반전
        bullet.transform.localScale = new Vector3(bulletScale.x * bulletDirection, bulletScale.y, bulletScale.z);

        // 투사체에 힘을 가해 발사
        bullet.GetComponent<Rigidbody2D>().AddForce(direction * playerState.bulletFirePower, ForceMode2D.Impulse);
    }

    // 적과 충돌 감지
    void CheckForEnemyCollision()
    {
        if (!isInvincible)
        {
            // 사각형 충돌 감지 범위 안에 적이 있는지 확인
            Collider2D[] colliders = Physics2D.OverlapBoxAll(detectionBoxTransform.position, detectionBoxSize, 0, enemyLayerMask);
            foreach (Collider2D collider in colliders)
            {
                if (collider.tag == enemyTag)  // 적이 적 태그를 가지고 있는지 확인
                {
                    HandleEnemyCollision(collider.GetComponent<EnemyMelee01>());
                    break;
                }
            }
        }
    }

    // 적과의 충돌 처리 함수
    void HandleEnemyCollision(EnemyMelee01 enemy)
    {
        if (enemy != null)
        {
            // 적 공격력 - 플레이어 방어력 데미지 공식
            float damage = Mathf.Max(0, enemy.GetComponent<StateEnemyMelee01>().attackPower - playerState.defense);

            // 데미지 적용
            TakeDamage(damage);
        }
    }

    // 데미지를 받는 함수
    public void TakeDamage(float damage)
    {
        if (isInvincible) return; // 무적 상태일 경우 데미지를 받지 않음

        // 데미지 처리: 플레이어의 체력을 감소시킴
        playerState.currentHealth -= (int)damage;
        Debug.Log("플레이어가 받은 데미지: " + damage);
        Debug.Log("플레이어의 남은 체력: " + playerState.currentHealth);

        // 체력이 0 이하이면 게임 오버 처리
        if (playerState.currentHealth <= 0)
        {
            if (gameOverManager == null)
            {
                FindGameOverManager(); // gameOverManager가 없으면 다시 찾기
            }

            if (gameOverManager != null)
            {
                gameOverManager.ShowGameOverScreen();
                Destroy(gameObject); // 플레이어 오브젝트 삭제
            }
            else
            {
                Debug.LogError("Gameover manager could not be found for game over handling.");
            }
        }
        else
        {
            // 피격 시 무적 상태 및 깜빡이는 효과 적용
            StartCoroutine(BlinkAndBecomeInvincible());
        }
    }

    // 무적 상태로 전환하고 깜빡이는 효과 처리
    private IEnumerator BlinkAndBecomeInvincible()
    {
        isInvincible = true; // 무적 상태 시작
        skeletonAnimation.skeleton.SetColor(Color.black); // 플레이어를 검정색으로 변경

        // 0.1초 후 다시 원래 색으로 복원
        yield return new WaitForSeconds(0.1f);
        skeletonAnimation.skeleton.SetColor(originalColor);

        // 무적 상태 지속 시간 동안 추가 데미지를 받지 않음
        yield return new WaitForSeconds(invincibilityDuration);

        isInvincible = false; // 무적 상태 해제
    }

    // 충돌 감지 범위 시각화
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(pos.position, boxSize);

        // 충돌 감지 범위를 사각형으로 표시
        Gizmos.color = Color.green;  // 충돌 감지 범위를 초록색으로 표시
        Gizmos.DrawWireCube(detectionBoxTransform.position, detectionBoxSize);
    }

    // 회복약 사용
    private void UsePotion()
    {
        if (playerState.potionCount > 0)
        {
            // 회복약 소모
            playerState.potionCount--;

            // 체력 회복
            playerState.currentHealth += playerState.healAmount;

            // 체력 최대치 초과 방지
            playerState.currentHealth = Mathf.Min(playerState.currentHealth, playerState.maxHealth);

            Debug.Log("회복약 사용: 현재 체력 = " + playerState.currentHealth + ", 남은 회복약 = " + playerState.potionCount);
        }
        else
        {
            Debug.Log("회복약 없음");
        }
    }

    // 씬 로드 완료 후 호출되는 메서드
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (PlayerState.instance != null && PlayerState.instance.inventory != null)
        {
            for (int i = 0; i < PlayerState.instance.inventory.Count; i++)
            {
                if (PlayerState.instance.inventory[i] == null || string.IsNullOrEmpty(PlayerState.instance.inventory[i]?.itemName))
                {
                    PlayerState.instance.inventory[i] = null;  // 빈 슬롯은 null로 강제 유지
                }
            }
        }

        FindGameOverManager(); // 새로운 씬에서 Gameover 매니저 찾기

        GameObject spawnPoint = GameObject.FindWithTag("SpawnPoint");
        if (spawnPoint != null)
        {
            transform.position = spawnPoint.transform.position;
            Debug.Log($"스폰 포인트로 이동: {scene.name}");
        }
        else
        {
            Debug.LogWarning($"스폰 포인트를 찾을 수 없음 씬: {scene.name}");
        }
    }

    private void FindGameOverManager()
    {
        gameOverManager = FindObjectOfType<Gameover>();

        if (gameOverManager == null)
        {
            Debug.LogError("Gameover manager not found in the current scene.");
        }
    }
}
