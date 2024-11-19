using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    public static Inventory instance; // 싱글톤 패턴
    public GameObject inventoryUI;    // 인벤토리 UI 오브젝트 참조

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
        else
        {
            Debug.LogWarning("Inventory UI가 설정되지 않았습니다.");
        }

        UpdateInventoryUI();
    }

    // 인벤토리 UI를 열거나 닫는 함수
    public void ToggleInventory()
    {
        if (inventoryUI != null)
        {
            // 상점이 열려 있으면 인벤토리를 열지 않음
            if (ShopIsOpen()) return;

            bool isOpen = !inventoryUI.activeSelf;
            inventoryUI.SetActive(isOpen);
            Time.timeScale = isOpen ? 0 : 1; // 인벤토리가 열려 있으면 게임 일시정지
            Debug.Log("인벤토리 " + (isOpen ? "열림" : "닫힘"));
        }
        else
        {
            Debug.LogWarning("Inventory UI가 설정되지 않아 동작할 수 없습니다.");
        }

        if (inventoryUI.activeSelf)
        {
            UpdateInventoryUI(); // 열릴 때 최신 상태로 업데이트
        }
    }

    // 인벤토리 UI 업데이트 함수
    public void UpdateInventoryUI()
    {
        if (slotImages == null || slotTexts == null)
        {
            Debug.LogError("슬롯 UI가 제대로 설정되지 않았습니다.");
            return;
        }

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
        }
    }

    // 상점 상태 확인 메서드
    private bool ShopIsOpen()
    {
        return Shop.instance != null && Shop.instance.shopUI.activeSelf;
    }
}