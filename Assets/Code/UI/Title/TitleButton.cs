using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleButton : MonoBehaviour
{
    public void StartButton(string Lobby)
    {
        SceneManager.LoadScene("Lobby");
    }

    public void SPButton(string SampleScene)
    {
        SceneManager.LoadScene("SampleScene");
    }
}
