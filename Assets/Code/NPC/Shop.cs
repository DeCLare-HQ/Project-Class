using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour
{
    public static Shop instance; // 싱글톤 패턴

    public GameObject shopUI; // 상점 UI 오브젝트
    public Image[] slotImages; // 상점 슬롯의 이미지 배열
    public TMP_Text[] slotTexts; // 상점 슬롯의 텍스트 배열
    public Button[] buyButtons; // 구매 버튼 배열

    private Item[] shopItems; // 상점에 표시될 아이템 배열
    private bool isPlayerInRange; // 플레이어가 상점 범위에 있는지 여부
    private PlayerController player; // 플레이어 컨트롤러 참조

    public ItemManager itemManager; // 아이템 매니저 참조 (아이템 데이터 생성)

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (shopUI != null)
            shopUI.SetActive(false); // 초기에는 상점 UI를 비활성화

        shopItems = new Item[3]; // 3개의 슬롯에 사용할 아이템 배열 초기화
    }

    void Update()
    {
        // 상점 닫기
        if (shopUI.activeSelf && Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleShopUI(false);
        }

        // 상점 열기
        if (isPlayerInRange && !shopUI.activeSelf && !Inventory.instance.inventoryUI.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                ToggleShopUI(true);
                GenerateRandomItems(); // 상점 열릴 때 랜덤 아이템 생성
                UpdateShopUI(); // 상점 UI 업데이트
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어가 상점 범위에 들어올 경우
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            player = other.GetComponent<PlayerController>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 플레이어가 상점 범위를 떠날 경우
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            player = null;
        }
    }

    // 상점 UI 토글 메서드
    public void ToggleShopUI(bool isOpen)
    {
        if (shopUI != null)
        {
            shopUI.SetActive(isOpen);
            Time.timeScale = isOpen ? 0 : 1; // 상점이 열려 있으면 게임 일시정지
            Debug.Log("상점 " + (isOpen ? "열림" : "닫힘"));
        }
    }

    // 랜덤 아이템 생성
    private void GenerateRandomItems()
    {
        var items = new Item[]
        {
            itemManager.money1, itemManager.money5, itemManager.money10,
            itemManager.testMelee, itemManager.testMagic,
            itemManager.testHealth, itemManager.testDefense
        };

        for (int i = 0; i < shopItems.Length; i++)
        {
            shopItems[i] = items[Random.Range(0, items.Length)];
        }
    }

    // 상점 UI 업데이트
    private void UpdateShopUI()
    {
        for (int i = 0; i < shopItems.Length; i++)
        {
            if (shopItems[i] != null)
            {
                slotImages[i].sprite = shopItems[i].itemSprite;
                slotImages[i].color = Color.white; // 아이템이 있는 경우 투명도 제거
                slotTexts[i].text = shopItems[i].itemName;
                buyButtons[i].interactable = true; // 구매 버튼 활성화
                int index = i; // 버튼에 현재 슬롯 인덱스를 전달
                buyButtons[i].onClick.RemoveAllListeners(); // 기존 리스너 제거
                buyButtons[i].onClick.AddListener(() => BuyItem(index));
            }
            else
            {
                slotImages[i].sprite = null;
                slotImages[i].color = new Color(1, 1, 1, 0); // 빈 슬롯 투명
                slotTexts[i].text = "";
                buyButtons[i].interactable = false; // 버튼 비활성화
            }
        }
    }

    // 아이템 구매
    private void BuyItem(int slotIndex)
    {
        var selectedItem = shopItems[slotIndex];
        if (selectedItem != null)
        {
            if (PlayerState.instance.money >= selectedItem.effectAmount)
            {
                PlayerState.instance.AddItemToInventory(selectedItem);
                PlayerState.instance.money -= (int)selectedItem.effectAmount;
                Debug.Log($"{selectedItem.itemName} 구매 완료!");
                UpdateShopUI(); // UI 갱신
            }
            else
            {
                Debug.Log("돈이 부족합니다!");
            }
        }
    }
}