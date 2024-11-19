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
        SceneManager.LoadScene("Lobby"); // 씬 로드
    }

    public void SPButton()
    {
        SceneManager.LoadScene("SampleScene");
    }
}

