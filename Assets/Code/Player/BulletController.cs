using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    void Start()
    {
        Invoke("DestroyTime", 2.0f); // 투사체 자동 파괴 시간
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
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Hit Enemy");

            // 적이 투사체에 맞았을 때, 데미지 계산 후 처리
            EnemyMelee01 enemy = collision.GetComponent<EnemyMelee01>();
            if (enemy != null)
            {
                // 플레이어의 마법 공격력에서 적의 마법 방어력을 뺀 데미지 계산
                float damage = Mathf.Max(0, FindObjectOfType<PlayerState>().magicAttackPower - enemy.GetComponent<StateEnemyMelee01>().magicDefense);
                enemy.TakeDamageFromProjectile(damage); // 원거리 공격 피격 처리
            }

            Destroy(gameObject); // 적과 충돌 시 투사체 파괴
        }
    }
}
