using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Gameover : MonoBehaviour
{
    public GameObject gameOverUI; // 게임 오버 화면 UI
    public Button retryButton;    // 로비로 돌아가는 버튼

    private void Start()
    {
        gameOverUI.SetActive(false); // 게임 오버 UI를 기본으로 숨김

        // retryButton이 정상적으로 설정되었는지 확인
        if (retryButton != null)
        {
            retryButton.onClick.AddListener(OnRetryButtonClicked); // 올바른 onClick 이벤트
        }
        else
        {
            Debug.LogError("Retry Button is not assigned in the inspector.");
        }
    }

    // 게임 오버 화면을 표시하는 메서드
    public void ShowGameOverScreen()
    {
        gameOverUI.SetActive(true);
        Time.timeScale = 0; // 게임 일시정지
    }

    // 로비로 돌아가는 버튼을 클릭했을 때 실행되는 메서드
    private void OnRetryButtonClicked()
    {
        Time.timeScale = 1;               // 게임 속도 복구
        SceneManager.LoadScene("Lobby");  // 로비 씬 로드
    }
}
