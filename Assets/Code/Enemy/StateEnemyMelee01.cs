using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateEnemyMelee01 : MonoBehaviour
{
    public float health = 100f; // 체력
    public float physicalDefense = 10f; // 물리 방어력
    public float magicDefense = 5f; // 마법 방어력
    public float attackSpeed = 1.5f; // 공격 속도
    public float attackPower = 20f; // 공격력
    public GameObject itemDrop; // 전리품

    // 공격 받음
    public void TakeDamage(float damage, bool isPhysical)
    {
        if (isPhysical)
        {
            damage -= physicalDefense; // 물리 방어력 계산
        }
        else
        {
            damage -= magicDefense; // 마법 방어력 계산
        }

        damage = Mathf.Max(damage, 0); // 데미지 0 이하 X

        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    // 죽음
    private void Die()
    {
        // 전리품 드롭
        if (itemDrop != null)
        {
            Instantiate(itemDrop, transform.position, Quaternion.identity);
        }
        Destroy(gameObject); // 적 제거
    }
}