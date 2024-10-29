using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public static PlayerState instance;

    [Header("근접 공격 능력치 설정")]
    public float physicalAttackPower = 10.0f; // 물리 공격력
    public float meleeAttackCoolTime = 1.0f;  // 근접 공격 간격

    [Header("원거리 공격 능력치 설정")]
    public float magicAttackPower = 8.0f;     // 마법 공격력
    public float bulletFirePower = 15.0f;     // 투사체 발사 힘
    public float bulletFireDelay = 1.0f;      // 투사체 발사 딜레이

    [Header("방어 능력치 설정")]
    public float defense = 5.0f;              // 방어력

    [Header("체력 능력치 설정")]
    public int maxHealth = 50;               // 최대 체력
    public int currentHealth = 50;           // 현재 체력


    [Header("이동 능력치 설정")]
    public float moveSpeed = 5.0f;            // 이동 속도
    public float decelerationRate = 10.0f;    // 감속 속도
    public float jumpPower = 15.0f;           // 점프 힘 (중력5일 때)

    [Header("인벤토리 설정")]
    public int potionCount = 3;             // 회복약 소지 개수
    public int maxPotionCount = 3;             // 회복약 최대 소지 개수
    public int healAmount = 20;              // 회복약 1개의 체력 회복량
    public List<Item> inventory = new List<Item>(); // 인벤토리 리스트
    public int inventorySlot = 5;           // 인벤토리 칸 개수
    public int skillSlot = 5;               // 스킬 칸 개수
    public int money;                       // 돈
    public int skillPoint;                  // 스킬 포인트

    private void Awake()
    {
        // Singleton 적용: 씬이 전환될 때 이 인스턴스를 유지하고, 중복된 인스턴스 제거
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 이 오브젝트는 씬이 전환되어도 파괴되지 않음
            Debug.Log("PlayerState initialized.");
        }
        else if (instance != this)
        {
            Debug.Log("Duplicate PlayerState destroyed.");
            Destroy(gameObject); // 이미 존재하는 인스턴스가 있으면 새로 생성된 것은 삭제
        }
    }
    // 인벤토리에 아이템 추가 메서드
    public bool AddItemToInventory(Item newItem)
    {
        if (inventory.Count < inventorySlot)
        {
            inventory.Add(newItem);
            ApplyItemEffect(newItem); // 아이템 효과 적용
            Debug.Log($"{newItem.itemName} added to inventory and effect applied.");
            return true;
        }
        else
        {
            Debug.LogWarning("Inventory is full.");
            return false;
        }
    }

    // 인벤토리에서 아이템 제거 메서드
    public bool RemoveItemFromInventory(Item itemToRemove)
    {
        if (inventory.Contains(itemToRemove))
        {
            RemoveItemEffect(itemToRemove); // 아이템 효과 제거
            inventory.Remove(itemToRemove);
            Debug.Log($"{itemToRemove.itemName} removed from inventory and effect removed.");
            return true;
        }
        else
        {
            Debug.LogWarning("Item not found in inventory.");
            return false;
        }
    }

    // 아이템 효과 적용 메서드
    private void ApplyItemEffect(Item item)
    {
        switch (item.itemType)
        {
            case ItemType.HealthMax:
                maxHealth += (int)item.effectAmount;
                currentHealth = Mathf.Min(currentHealth, maxHealth);
                Debug.Log($"{item.itemName} applied: Max Health increased by {item.effectAmount}");
                break;
            case ItemType.MeleeAttackBoost:
                physicalAttackPower += item.effectAmount;
                Debug.Log($"{item.itemName} applied: Physical Attack Power increased by {item.effectAmount}");
                break;
            case ItemType.MagicAttackBoost:
                magicAttackPower += item.effectAmount;
                Debug.Log($"{item.itemName} applied: Magic Attack Power increased by {item.effectAmount}");
                break;
            case ItemType.DefenseBoost:
                defense += item.effectAmount;
                Debug.Log($"{item.itemName} applied: Defense increased by {item.effectAmount}");
                break;
        }
    }

    // 아이템 효과 제거 메서드
    private void RemoveItemEffect(Item item)
    {
        switch (item.itemType)
        {
            case ItemType.HealthMax:
                maxHealth -= (int)item.effectAmount;
                currentHealth = Mathf.Min(currentHealth, maxHealth);
                Debug.Log($"{item.itemName} removed: Max Health decreased by {item.effectAmount}");
                break;
            case ItemType.MeleeAttackBoost:
                physicalAttackPower -= item.effectAmount;
                Debug.Log($"{item.itemName} removed: Physical Attack Power decreased by {item.effectAmount}");
                break;
            case ItemType.MagicAttackBoost:
                magicAttackPower -= item.effectAmount;
                Debug.Log($"{item.itemName} removed: Magic Attack Power decreased by {item.effectAmount}");
                break;
            case ItemType.DefenseBoost:
                defense -= item.effectAmount;
                Debug.Log($"{item.itemName} removed: Defense decreased by {item.effectAmount}");
                break;
        }
    }
}