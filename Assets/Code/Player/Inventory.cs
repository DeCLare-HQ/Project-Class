using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    public static Inventory instance; // 싱글톤 패턴
    public GameObject inventoryUI;    // 인벤토리 UI 오브젝트 참조
    public TMP_Text moneyText;        // MText 태그를 가진 머니 텍스트
    public TMP_Text descriptionText; // 설명 텍스트를 표시할 UI
    public int selectedSlot = -1;     // 현재 선택된 슬롯 번호 (-1: 선택되지 않음)
    public Image[] slotHighlights; // 슬롯 선택 상태를 표시할 Highlight 이미지 배열

    // UI 슬롯 연결
    public Image[] slotImages; // 슬롯 이미지 배열 (5개 슬롯)
    public TMP_Text[] slotTexts; // 슬롯 이름 텍스트 배열 (5개 슬롯)

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
        if (inventoryUI != null)
        {
            inventoryUI.SetActive(false); // 시작 시 인벤토리 UI를 숨김
        }

        if (slotHighlights != null)
        {
            foreach (var highlight in slotHighlights)
            {
                highlight.enabled = false; // 모든 하이라이트 비활성화
            }
        }
        
        UpdateInventoryUI();
        UpdateMoneyUI();
    }

    // 인벤토리 UI를 열거나 닫는 함수
    public void ToggleInventory()
    {
        if (inventoryUI != null)
        {
            // 상점이 열려 있으면 인벤토리를 열지 않음
            if (Shop.instance != null && Shop.instance.shopUI.activeSelf) return;

            bool isOpen = !inventoryUI.activeSelf;
            inventoryUI.SetActive(isOpen);

            if (isOpen)
            {
                UpdateMoneyUI(); // 인벤토리 열릴 때 머니 텍스트 업데이트
            }

            Time.timeScale = isOpen ? 0 : 1; // 인벤토리가 열려 있으면 게임 일시정지
        }

        if (inventoryUI.activeSelf)
        {
            UpdateInventoryUI(); // 열릴 때 최신 상태로 업데이트
        }
    }

    // 인벤토리 UI 업데이트 함수
    public void UpdateInventoryUI()
    {
        var inventory = PlayerState.instance.inventory;

        for (int i = 0; i < slotImages.Length; i++)
        {
            if (i < inventory.Count && inventory[i] != null)
            {
                // 슬롯에 아이템 데이터 바인딩
                slotImages[i].sprite = inventory[i].itemSprite;
                slotImages[i].color = Color.white; // 아이템이 있는 경우 투명도 제거
                slotTexts[i].text = inventory[i].itemName;
                slotTexts[i].enabled = true; // 텍스트 활성화
            }
            else
            {
                // 빈 슬롯 처리: 이미지와 텍스트 숨김
                slotImages[i].sprite = null; // 이미지 제거
                slotImages[i].color = new Color(1, 1, 1, 0); // 완전 투명
                slotTexts[i].text = ""; // 텍스트 제거
                slotTexts[i].enabled = false; // 텍스트 비활성화
            }
            // 슬롯 하이라이트 설정
            if (slotHighlights != null && i < slotHighlights.Length)
            {
                slotHighlights[i].enabled = (i == selectedSlot); // 선택된 슬롯만 활성화
            }
        }

        if (selectedSlot >= 0 && inventory[selectedSlot] != null)
        {
            DisplayItemDescription(inventory[selectedSlot]);
        }
        else
        {
            descriptionText.text = "";
        }
    }

    // 상점 상태 확인 메서드
    private bool ShopIsOpen()
    {
        return Shop.instance != null && Shop.instance.shopUI.activeSelf;
    }

    // 슬롯 선택 및 설명 표시
    public void SelectSlot(int slotIndex)
    {
        var inventory = PlayerState.instance.inventory;

        if (slotIndex >= 0 && slotIndex < inventory.Count && inventory[slotIndex] != null)
        {
            selectedSlot = slotIndex;
            DisplayItemDescription(inventory[slotIndex]);
            Debug.Log($"슬롯 {slotIndex + 1} 선택됨: {inventory[slotIndex].itemName}");
        }
        else
        {
            selectedSlot = -1;
            descriptionText.text = "아이템을 선택하세요.";
        }
    }

    // 머니 UI 업데이트 함수
    public void UpdateMoneyUI()
    {
        if (moneyText != null)
        {
            moneyText.text = PlayerState.instance.money.ToString();
        }
    }

    // 다른 클래스에서 머니를 변경한 후 UI를 업데이트하기 위해 호출
    public void UpdateMoney(int amount)
    {
        PlayerState.instance.money += amount;
        UpdateMoneyUI(); // UI 동기화
    }

    public void DisplayItemDescription(Item item)
    {
        if (item != null)
        {
            descriptionText.text = item.itemDescription;
        }
        else
        {
            descriptionText.text = "아이템을 선택하세요.";
        }
    }

    public void SelectPreviousSlot()
    {
        var inventory = PlayerState.instance.inventory;
        int slotCount = PlayerState.instance.inventorySlot;

        if (inventory.TrueForAll(slot => slot == null))
        {
            Debug.LogWarning("인벤토리가 비어 있습니다.");
            return;
        }

        do
        {
            selectedSlot = (selectedSlot - 1 + slotCount) % slotCount; // 이전 슬롯
        } while (inventory[selectedSlot] == null);

        UpdateInventoryUI(); // UI 업데이트와 하이라이트 갱신
    }

    public void SelectNextSlot()
    {
        var inventory = PlayerState.instance.inventory;
        int slotCount = PlayerState.instance.inventorySlot;

        if (inventory.TrueForAll(slot => slot == null))
        {
            Debug.LogWarning("인벤토리가 비어 있습니다.");
            return;
        }

        do
        {
            selectedSlot = (selectedSlot + 1) % slotCount; // 다음 슬롯
        } while (inventory[selectedSlot] == null);

        UpdateInventoryUI(); // UI 업데이트와 하이라이트 갱신
    }

    public void RemoveSelectedItem()
    {
        var inventory = PlayerState.instance.inventory;

        if (selectedSlot >= 0 && inventory[selectedSlot] != null)
        {
            Debug.Log($"{inventory[selectedSlot].itemName} 아이템을 삭제합니다.");
            inventory[selectedSlot] = null; // 아이템 삭제
            UpdateInventoryUI(); // UI 업데이트
            descriptionText.text = ""; // 설명 초기화
        }
        else
        {
            Debug.LogWarning("선택된 슬롯에 아이템이 없습니다.");
        }
    }
}