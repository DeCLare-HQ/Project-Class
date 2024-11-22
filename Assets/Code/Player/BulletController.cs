using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    void Start()
    {
        if (GetComponent<Collider2D>() == null)
        {
            gameObject.AddComponent<CircleCollider2D>().isTrigger = false;
        }
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
            Destroy(gameObject); // 플랫폼 충돌 시 투사체 파괴
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Hit Enemy");

            // 근접 적일 경우 처리
            if (collision.TryGetComponent(out EnemyMelee01 meleeEnemy))
            {
                float damage = Mathf.Max(0, FindObjectOfType<PlayerState>().magicAttackPower - meleeEnemy.GetComponent<StateEnemyMelee01>().magicDefense);
                meleeEnemy.TakeDamageFromProjectile(damage); // 원거리 공격 피격 처리
            }

            // 원거리 적일 경우 처리
            else if (collision.TryGetComponent(out EnemyRange01 rangedEnemy))
            {
                float damage = Mathf.Max(0, FindObjectOfType<PlayerState>().magicAttackPower - rangedEnemy.GetComponent<StateEnemyRange01>().magicDefense);
                rangedEnemy.TakeDamageFromProjectile(damage); // 원거리 공격 피격 처리
            }

            Destroy(gameObject); // 적과 충돌 시 투사체 파괴
        }
    }
}
