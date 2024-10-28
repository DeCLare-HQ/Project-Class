using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateEnemyRange01 : MonoBehaviour
{
    [Header("능력치 설정")]
    public float health = 20f; // 체력
    public float physicalDefense = 1.0f; // 물리 방어력
    public float magicDefense = 1.0f; // 마법 방어력
    public float attackPower = 10f; // 공격력
    public float moveSpeed = 1f; // 이동 속도
}
