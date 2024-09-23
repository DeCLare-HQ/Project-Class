using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    [Header("공격 능력치 설정")]
    public float physicalAttackPower = 10.0f; // 물리 공격력
    public float magicAttackPower = 8.0f;     // 마법 공격력
    public float attackSpeed = 1.0f;          // 공격 속도
    public float defense = 5.0f;              // 방어력

    [Header("체력 능력치 설정")]
    public int maxHealth = 100;               // 최대 체력
    public int currentHealth;                 // 현재 체력
    public int potionCount = 3;               // 회복약 소지 개수

    [Header("이동 능력치 설정")]
    public float moveSpeed = 5.0f;            // 이동 속도
    public float decelerationRate = 10.0f;     // 감속 속도
    public float jumpPower = 14.0f;           // 점프 힘

    void Start()
    {
        // 현재 체력을 최대 체력으로 설정
        currentHealth = maxHealth;
    }

    // 체력 회복
    public void Heal(int amount)
    {
        //회복 시 최대 체력을 넘지 않음
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }
}
