using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public PlayerState playerState;   // PlayerState 스크립트 참조
    public Image healthBarImage;      // 체력바 이미지
    public Image potionBarImage;      // 회복약 개수 이미지
    private bool isInitialized = false; // 초기화 확인용 플래그

    private void Update()
    {
        // playerState가 준비되었는지 확인하고, 처음 준비되면 UI 초기화
        if (playerState == null)
        {
            playerState = FindObjectOfType<PlayerState>();
            if (playerState != null && !isInitialized)
            {
                isInitialized = true; // 초기화 완료
                UpdateHealthBar();
                UpdatePotionBar();
            }
        }
        else
        {
            // playerState가 준비된 후 매 프레임마다 UI 업데이트
            UpdateHealthBar();
            UpdatePotionBar();
        }
    }

    // 체력바 UI를 업데이트하는 메서드
    private void UpdateHealthBar()
    {
        if (healthBarImage != null && playerState != null)
        {
            healthBarImage.fillAmount = (float)playerState.currentHealth / playerState.maxHealth;
        }
    }

    // 회복약 개수 UI를 업데이트하는 메서드
    private void UpdatePotionBar()
    {
        if (potionBarImage != null && playerState != null)
        {
            potionBarImage.fillAmount = (float)playerState.potionCount / playerState.maxPotionCount;
        }
    }
}
