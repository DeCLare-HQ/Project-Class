using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Gameover : MonoBehaviour
{
    public GameObject gameOverUI; // 게임 오버 화면 UI
    public Button retryButton;    // 로비로 돌아가는 버튼

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // 씬이 로드될 때마다 호출
        InitializeUI();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // 씬 언로드 시 이벤트 해제
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeUI(); // 씬 이동 후 UI 요소 재설정
    }

    // UI 요소 초기화 메서드
    private void InitializeUI()
    {
        // gameOverUI가 null이면 다시 찾음
        if (gameOverUI == null)
        {
            gameOverUI = GameObject.Find("GameOverUI"); // 예: GameOverUI 오브젝트 이름
            if (gameOverUI == null)
            {
                Debug.LogError("GameOver UI not found in the scene.");
                return;
            }
        }

        // 태그 "ReButton"으로 retryButton을 찾아 할당
        GameObject retryButtonObject = GameObject.FindWithTag("ReButton");
        if (retryButtonObject != null)
        {
            retryButton = retryButtonObject.GetComponent<Button>();
            retryButton.onClick.RemoveAllListeners(); // 중복 리스너 제거
            retryButton.onClick.AddListener(OnRetryButtonClicked);
            Debug.Log("Retry Button assigned and click event set.");
        }
        else
        {
            Debug.LogError("Retry Button with the tag 'ReButton' not found in the scene.");
        }

        // 기본적으로 gameOverUI 비활성화
        gameOverUI.SetActive(false);
    }

    // 게임 오버 화면을 표시하는 메서드
    public void ShowGameOverScreen()
    {
        if (gameOverUI == null || retryButton == null)
        {
            InitializeUI(); // UI 요소 재설정
        }

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
            Time.timeScale = 0; // 게임 일시정지
        }
        else
        {
            Debug.LogWarning("GameOver UI is still missing after initialization.");
        }
    }

    // 로비로 돌아가는 버튼을 클릭했을 때 실행되는 메서드
    private void OnRetryButtonClicked()
    {
        PlayerState.instance.RetryGame(); // 리트라이로 설정하여 돈 유지
        Time.timeScale = 1;               // 게임 속도 복구
        SceneManager.LoadScene("Lobby");  // 로비 씬 로드
    }
}