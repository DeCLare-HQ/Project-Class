using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleButton : MonoBehaviour
{
    public void ContinueButton()
    {
        if (PlayerState.instance != null)
        {
            PlayerState.instance.RetryGame(); // 상태 유지
        }
        SceneManager.LoadScene("Lobby");
    }

    public void NewGameButton()
    {
        if (PlayerState.instance != null)
        {
            PlayerState.instance.StartNewGame(); // 상태 초기화
        }
        SceneManager.LoadScene("Tutorial"); // 씬 로드
    }

    public void SPButton()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void ExitButton()
    {
    #if UNITY_EDITOR
        // 에디터에서 종료를 에뮬레이션
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        // 빌드된 게임 종료
        Application.Quit();
    #endif
    }
}

