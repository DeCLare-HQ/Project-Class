using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerState : MonoBehaviour
{
    public static PlayerState instance;

    [Header("UI 설정")]
    public Image selectedItemImage;  // 선택된 아이템을 표시할 UI 이미지
    public TMP_Text moneyText; // 소지금 UI 표시
    private bool isRetry; // 리트라이 여부를 확인하는 플래그

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
    public int potionCount = 3;               // 회복약 소지 개수
    public int maxPotionCount = 3;            // 회복약 최대 소지 개수
    public int healAmount = 20;               // 회복약 1개의 체력 회복량
    public List<Item> inventory = new List<Item>(); // 인벤토리 리스트
    private int selectedSlot = -1;            // 현재 선택된 슬롯 번호 (-1이면 선택되지 않은 상태)
    public int inventorySlot = 5;             // 인벤토리 칸 개수
    public int maxInventorySlot = 5;          // 인벤토리 최대 슬롯
    public int skillSlot = 5;                 // 스킬 칸 개수
    public int money;                         // 돈
    public int skillPoint;                    // 스킬 포인트

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 싱글톤 유지
            isRetry = PlayerPrefs.GetInt("IsRetry", 0) == 1; // IsRetry 값을 로드
            LoadMoney(); // 게임 시작 시 money 값 불러오기
        }
        else if (instance != this)
        {
            Destroy(gameObject); // 중복 인스턴스 방지
        }
    }

    private void Start()
    {
        // 인벤토리 리스트를 고정된 슬롯 수에 맞게 초기화 (빈 슬롯은 null로 유지)
        inventory = new List<Item>(new Item[inventorySlot]);

        UpdateSelectedItemUI();
        FindMoneyText();
        UpdateMoneyUI();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            ChangeSelectedSlot();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            RemoveSelectedItem();
        }
    }

    private void OnApplicationQuit()
    {
        SaveMoney(); // 게임 종료 시 money 값 저장
    }

    private void OnDestroy()
    {
        SaveMoney(); // 씬 이동 시 money 저장
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // money 값을 PlayerPrefs에 저장
    public void SaveMoney()
    {
        PlayerPrefs.SetInt("PlayerMoney", money);
        PlayerPrefs.Save(); // 값 저장
        Debug.Log($"Money saved: {money}");
    }

    // money 값을 PlayerPrefs에서 불러오기
    private void LoadMoney()
    {
        if (isRetry)
        {
            money = PlayerPrefs.GetInt("PlayerMoney", 0); // 리트라이 시 저장된 돈 불러오기
        }
        else
        {
            money = 0; // 새 게임 시작 시 돈 초기화
        }
        Debug.Log($"Money loaded: {money}");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindMoneyText();  // moneyText를 새 씬에서 다시 참조
        UpdateMoneyUI();  // money 값을 UI와 동기화
        SelectClosestSlot();  // 인벤토리 슬롯 갱신
        
        // Lobby 씬의 UI 동기화 강제
        if (scene.name == "Lobby")
        {
            FindMoneyText();
            UpdateMoneyUI();
            Debug.Log("Lobby scene loaded. PlayerState synced.");
        }
    }

    public void InitializeAfterSpawn(GameObject player)
    {
        FindMoneyText();  // UI 텍스트를 다시 참조
        UpdateMoneyUI();  // UI와 상태를 동기화
        Debug.Log($"Player initialized after spawn with money: {money}");
    }

    // **W 버튼으로 슬롯 전환 기능 (빈 슬롯은 건너뜀)**
    private void ChangeSelectedSlot()
    {
        if (inventory.TrueForAll(slot => slot == null))
        {
            Debug.LogWarning("Inventory is empty. No slot to select.");
            selectedSlot = -1;
            selectedItemImage.enabled = false;
            return;
        }

        int startSlot = selectedSlot;
        bool validSlotFound = false;

        do
        {
            selectedSlot = (selectedSlot + 1) % inventorySlot;

            if (selectedSlot < inventory.Count)
            {
                Item currentItem = inventory[selectedSlot];
                Debug.Log($"W key pressed: Slot {selectedSlot}, Item: {(currentItem != null ? currentItem.itemName : "Empty")}");
            }

            if (selectedSlot < inventory.Count && inventory[selectedSlot] != null)
            {
                validSlotFound = true;
                break;
            }
        }
        while (selectedSlot != startSlot);

        if (validSlotFound)
        {
            UpdateSelectedItemUI();
        }
        else
        {
            selectedSlot = -1;
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
            inventory[selectedSlot] = null;
            Debug.Log($"{itemToRemove.itemName} 아이템이 삭제되었습니다.");

            SelectClosestSlot(); // 빈 슬롯이 선택되지 않도록 가장 가까운 아이템 있는 슬롯으로 자동 선택
        }
        else
        {
            Debug.LogWarning("선택된 슬롯에 아이템이 없습니다.");
        }
    }

    // **선택된 아이템을 UI에 업데이트하는 함수**
    private void UpdateSelectedItemUI()
    {
        // selectedItemImage가 null이거나 파괴된 경우, 태그를 통해 다시 할당
        if (selectedItemImage == null)
        {
            GameObject imageObject = GameObject.FindWithTag("SelectedItemImage");
            if (imageObject != null)
            {
                selectedItemImage = imageObject.GetComponent<Image>();
            }
            else
            {
                Debug.LogWarning("SelectedItemImage with the specified tag not found.");
                return; // UI 오브젝트가 없으면 더 이상 진행하지 않음
            }
        }

        // **현재 선택된 슬롯이 유효한지 확인**
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

    // MoneyText UI 텍스트를 자동으로 찾는 메서드
    private void FindMoneyText()
    {
        GameObject moneyTextObject = GameObject.Find("MoneyText"); // "MoneyText" 이름의 오브젝트 검색
        if (moneyTextObject != null)
        {
            moneyText = moneyTextObject.GetComponent<TMP_Text>(); // UI 텍스트 컴포넌트 연결
        }
    }

    // 돈 값이 변할 때마다 UI를 갱신하는 메서드
    public void UpdateMoneyUI()
    {
        if (moneyText != null)
        {
            moneyText.text = "Money: " + money.ToString();
        }
        else
        {
            FindMoneyText(); // UI 참조가 누락된 경우 다시 검색

            if (moneyText != null)
            {
                moneyText.text = "Money: " + money.ToString();
            }
        }
        // 현재 money 값을 PlayerPrefs에 저장
        PlayerPrefs.SetInt("PlayerMoney", money);
    }

    // 돈을 증가시킬 때 UI 갱신
    public void AddMoney(int amount)
    {
        money += amount;
        UpdateMoneyUI();  // 돈 값이 변할 때마다 UI를 업데이트
    }

    // 인벤토리에 아이템 추가 메서드
    public bool AddItemToInventory(Item newItem)
    {
        if (newItem.itemType == ItemType.Money)
        {
            // 돈 아이템일 경우 인벤토리에 추가하지 않고 money 값만 증가
            money += (int)newItem.effectAmount;
            UpdateMoneyUI(); // UI에 소지 금액 갱신
            return true;
        }

        // 일반 아이템일 경우 인벤토리에 추가
        for (int i = 0; i < inventorySlot; i++)
        {
            if (inventory[i] == null)
            {
                inventory[i] = newItem;
                Debug.Log($"{newItem.itemName} 아이템이 인벤토리 {i + 1}번째 슬롯에 추가되었습니다.");

                ApplyItemEffect(newItem);

                if (selectedSlot == -1)  // 선택된 슬롯이 없다면 추가한 슬롯을 선택
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

    // 리트라이 시 호출할 메서드
    public void RetryGame()
    {
        isRetry = true; // 리트라이 플래그 설정
        PlayerPrefs.SetInt("IsRetry", 1); // 리트라이 상태 저장
        SaveMoney();    // 현재 money 저장
        Debug.Log("Retry game triggered.");
    }

    // 게임을 완전히 새로 시작할 때 호출할 메서드
    public void StartNewGame()
    {
        isRetry = false; // 새 게임 시작으로 리트라이 플래그 해제
        PlayerPrefs.SetInt("IsRetry", 0); // IsRetry 값을 0으로 설정
        money = 0; // 돈 초기화
        PlayerPrefs.SetInt("PlayerMoney", money); // PlayerPrefs에 초기화 값 저장
        PlayerPrefs.Save(); // 값 저장 보장
        UpdateMoneyUI(); // UI 업데이트
        Debug.Log("New game started. Money reset to 0.");
    }

    // 가장 가까운 아이템이 있는 슬롯으로 선택
    public void SelectClosestSlot()
    {
        // 인벤토리가 비어있는 경우 선택된 슬롯을 해제하고 UI를 업데이트
        if (inventory.TrueForAll(slot => slot == null))
        {
            selectedSlot = -1;
            UpdateSelectedItemUI();
            return;
        }

        int startSlot = selectedSlot;
        bool validSlotFound = false;

        do
        {
            // 다음 슬롯으로 이동하면서 유효한 슬롯인지 확인
            selectedSlot = (selectedSlot + 1) % inventorySlot;

            if (selectedSlot < inventory.Count && inventory[selectedSlot] != null)
            {
                validSlotFound = true;
                break;
            }
        }
        while (selectedSlot != startSlot);  // 다시 처음 슬롯으로 돌아오면 종료

        if (validSlotFound)
        {
            UpdateSelectedItemUI();
        }
        else
        {
            selectedSlot = -1;  // 유효한 슬롯이 없을 때 선택 해제
            selectedItemImage.enabled = false;
        }
    }

    // 아이템 효과 적용 메서드
    private void ApplyItemEffect(Item item)
    {
        switch (item.itemType)
        {
            case ItemType.Money:
                money += (int)item.effectAmount;
                Debug.Log($"{item.itemName} applied: Money increased by {item.effectAmount}");
                break;
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