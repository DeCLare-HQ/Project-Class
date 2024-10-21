using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public PlayerState playerState;   // PlayerState 스크립트 참조
    public Image healthBarImage;      // 체력바 이미지
    public Image potionBarImage;      // 회복약 개수 이미지

    private void Start()
    {
        if (playerState == null)
        {
            playerState = FindObjectOfType<PlayerState>();  // PlayerState 자동 검색
        }

        // 처음 시작할 때 UI 업데이트
        UpdateHealthBar();
        UpdatePotionBar();
    }

    private void Update()
    {
        // 매 프레임마다 UI 업데이트
        UpdateHealthBar();
        UpdatePotionBar();
    }

    // 체력바 UI를 업데이트하는 메서드
    private void UpdateHealthBar()
    {
        if (healthBarImage != null)
        {
            // Fill Amount를 현재 체력 / 최대 체력 비율로 설정 (0 ~ 1 사이)
            healthBarImage.fillAmount = (float)playerState.currentHealth / playerState.maxHealth;
        }
    }

    // 회복약 개수 UI를 업데이트하는 메서드
    private void UpdatePotionBar()
    {
        if (potionBarImage != null)
        {
            // Fill Amount를 현재 개수 / 최대 개수 비율로 설정 (0 ~ 1 사이)
            potionBarImage.fillAmount = (float)playerState.potionCount / playerState.maxPotionCount;
        }
    }
}
