using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private float attackPower; // 공격력 저장

    void Start()
    {
        Invoke("DestroyTime", 2.0f); // 투사체 자동 파괴 시간
    }

    // 투사체의 방향 설정 메서드
    public void SetDirection(Vector2 direction)
{
    Rigidbody2D rb = GetComponent<Rigidbody2D>();
    if (rb != null)
    {
        rb.velocity = direction * 5.0f; // 투사체 속도 설정
    }
    else
    {
        Debug.LogError("Rigidbody2D가 설정되지 않았습니다.");
    }
}

    // 공격력 설정
    public void SetAttackPower(float power)
    {
        attackPower = power; // 공격력 설정
    }

    void DestroyTime()
    {
        Destroy(gameObject); // 투사체 파괴
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            Debug.Log("Hit Platform");
            Destroy(gameObject); // 플랫폼 충돌 시 파괴
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hit Player");

            // 플레이어가 투사체에 맞았을 때, 데미지 계산 후 처리
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                // 적 공격력에서 플레이어 방어력을 뺀 데미지 계산
                float damage = Mathf.Max(0, attackPower - player.GetComponent<PlayerState>().defense);
                player.TakeDamage(damage); // 플레이어에게 데미지 전달
            }

            Destroy(gameObject); // 플레이어와 충돌 시 투사체 파괴
        }
    }
}
