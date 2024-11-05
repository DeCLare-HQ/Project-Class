using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerState : MonoBehaviour
{
    public static PlayerState instance;

    [Header("UI 설정")]
    public Image selectedItemImage;  // 선택된 아이템을 표시할 UI 이미지

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
    private int selectedSlot = -1;  // 현재 선택된 슬롯 번호 (-1이면 선택되지 않은 상태)
    public int inventorySlot = 5;           // 인벤토리 칸 개수
    public int maxInventorySlot = 5; // 인벤토리 최대 슬롯
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
        }
        else if (instance != this)
        {
            Destroy(gameObject); // 이미 존재하는 인스턴스가 있으면 새로 생성된 것은 삭제
        }
    }

    private void Start()
    {
        // 인벤토리 리스트를 고정된 슬롯 수에 맞게 초기화 (빈 슬롯은 null)
        for (int i = 0; i < inventorySlot; i++)
        {
            inventory.Add(null);  // 빈 슬롯을 null로 채움
        }

        // 선택된 아이템 UI 초기화
        UpdateSelectedItemUI();
    }

    private void Update()
    {
        // W 버튼을 눌렀을 때 선택된 슬롯 전환
        if (Input.GetKeyDown(KeyCode.W))
        {
            ChangeSelectedSlot();
        }

        // Q 버튼을 눌렀을 때 선택된 슬롯의 아이템 삭제
        if (Input.GetKeyDown(KeyCode.Q))
        {
            RemoveSelectedItem();
        }
    }

    // W 버튼으로 슬롯 전환 기능 (빈 슬롯은 건너뜀)
    private void ChangeSelectedSlot()
    {
        // 인벤토리가 비어 있으면 슬롯을 전환하지 않음
        if (inventory.Count == 0)
        {
            Debug.LogWarning("Inventory is empty. No slot to select.");
            return;
        }

        int startSlot = selectedSlot;
        bool validSlotFound = false;

        // 빈 슬롯을 건너뛰고 유효한 슬롯을 찾는 루프
        do
        {
            selectedSlot = (selectedSlot + 1) % inventorySlot;  // 슬롯을 순환

            // 유효한 슬롯을 찾으면 종료
            if (selectedSlot < inventory.Count && inventory[selectedSlot] != null)
            {
                validSlotFound = true;
                break;
            }
        }
        while (selectedSlot != startSlot);  // 다시 처음 선택한 슬롯으로 돌아오면 루프 종료

        if (validSlotFound)
        {
            Debug.Log($"Selected Slot: {selectedSlot + 1}");
            UpdateSelectedItemUI();
        }
        else
        {
            // 유효한 슬롯을 찾지 못했을 때
            Debug.LogWarning("No valid item found in the inventory slots.");
            selectedItemImage.enabled = false;
        }
    }

    // Q 버튼으로 선택된 슬롯의 아이템 삭제 기능
    private void RemoveSelectedItem()
    {
        if (selectedSlot >= 0 && selectedSlot < inventorySlot && inventory[selectedSlot] != null)
        {
            Item itemToRemove = inventory[selectedSlot];

            RemoveItemEffect(itemToRemove);

            inventory[selectedSlot] = null;  // 슬롯에서 아이템 삭제
            Debug.Log($"{itemToRemove.itemName} 아이템이 삭제되었습니다.");

            // 가장 가까운 아이템이 있는 슬롯으로 자동 선택
            SelectClosestSlot();
        }
        else
        {
            Debug.LogWarning("선택된 슬롯에 아이템이 없습니다.");
        }
    }

    // 선택된 아이템을 UI에 업데이트하는 함수
    private void UpdateSelectedItemUI()
    {
        // 현재 선택된 슬롯의 아이템이 있으면
        if (selectedSlot >= 0 && selectedSlot < inventory.Count && inventory[selectedSlot] != null)
        {
            Item selectedItem = inventory[selectedSlot];
            selectedItemImage.sprite = selectedItem.itemSprite;
            selectedItemImage.enabled = true;  // 이미지 활성화
        }
        else
        {
            // 선택된 슬롯에 아이템이 없으면 이미지를 비활성화
            selectedItemImage.enabled = false;
        }
    }

    // 인벤토리에 아이템 추가 메서드
    public bool AddItemToInventory(Item newItem)
    {
        // 빈 슬롯을 찾아서 아이템을 추가
        for (int i = 0; i < inventorySlot; i++)
        {
            if (inventory[i] == null)  // 빈 슬롯 발견
            {
                inventory[i] = newItem;
                Debug.Log($"{newItem.itemName} 아이템이 인벤토리 {i + 1}번째 슬롯에 추가되었습니다.");

                ApplyItemEffect(newItem);

                // 인벤토리가 비어있을 때 자동 선택
                if (selectedSlot == -1)
                {
                    selectedSlot = i;
                    UpdateSelectedItemUI();
                }

                return true;
            }
        }

        Debug.LogWarning("인벤토리가 가득 찼습니다.");
        return false;
    }

    // 가장 가까운 아이템이 있는 슬롯으로 선택
    private void SelectClosestSlot()
    {
        if (inventorySlot == 0)
        {
            selectedSlot = -1;  // 인벤토리가 비었을 때는 선택된 슬롯을 해제
            UpdateSelectedItemUI();
            return;
        }

        // 현재 슬롯부터 시작해서 가까운 슬롯을 찾음
        int startSlot = selectedSlot;
        bool validSlotFound = false;

        do
        {
            // 슬롯을 순환하면서 아이템이 있는 슬롯을 찾음
            selectedSlot = (selectedSlot + 1) % inventorySlot;

            // 유효한 슬롯을 찾았는지 확인
            if (selectedSlot < inventory.Count && inventory[selectedSlot] != null)
            {
                validSlotFound = true;
                break;
            }
        }
        while (selectedSlot != startSlot);  // 다시 처음 선택한 슬롯으로 돌아오면 루프 종료

        if (validSlotFound)
        {
            Debug.Log($"Closest item found in slot {selectedSlot + 1}.");
            UpdateSelectedItemUI();
        }
        else
        {
            Debug.LogWarning("No valid item found in the inventory slots.");
            selectedItemImage.enabled = false;  // 유효한 아이템이 없으면 이미지 비활성화
        }
    }

    // 인벤토리에서 아이템 제거 메서드
    public bool RemoveItemFromInventory(Item itemToRemove)
    {
        if (inventory.Contains(itemToRemove))
        {
            RemoveItemEffect(itemToRemove); // 아이템 효과 제거
            inventory.Remove(itemToRemove);
            Debug.Log($"{itemToRemove.itemName} 아이템 인벤토리에서 제거.");
            return true;
        }
        else
        {
            Debug.LogWarning("아이템을 인벤토리에서 찾을 수 없습니다.");
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