using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    
    [Header("공격 능력치 설정")]
    public float physicalAttackPower = 10.0f; // 물리 공격력
    public float meleeAttackCoolTime = 1.0f;  // 근접 공격 간격
    public float magicAttackPower = 8.0f;     // 마법 공격력
    public float bulletFirePower = 15.0f;          // 투사체 발사 힘
    public float bulletFireDelay = 0.5f;           // 투사체 발사 딜레이
    public float defense = 5.0f;              // 방어력

    [Header("체력 능력치 설정")]
    public int maxHealth = 100;               // 최대 체력

    [Header("이동 능력치 설정")]
    public float moveSpeed = 5.0f;            // 이동 속도
    public float decelerationRate = 10.0f;    // 감속 속도
    public float jumpPower = 14.0f;           // 점프 힘

    [Header("인벤토리 설정")]
    public int potionCount = 3;             // 회복약 소지 개수
    public int inventorySlot = 5;           // 인벤토리 칸 개수
    public int skillSlot = 5;               // 스킬 칸 개수
    public int money;                       // 돈
    public int skillPoint;                  // 스킬 포인트
}
