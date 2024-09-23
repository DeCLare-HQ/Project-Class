using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateEnemyMelee01 : MonoBehaviour
{
    [Header("능력치 설정")]
    public float health = 100f; // 체력
    public float physicalDefense = 10f; // 물리 방어력
    public float magicDefense = 5f; // 마법 방어력
    public float attackSpeed = 1.5f; // 공격 속도
    public float attackPower = 20f; // 공격력
    public float moveSpeed = 2f; // 이동 속도
}