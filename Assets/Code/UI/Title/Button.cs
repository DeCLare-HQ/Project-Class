using UnityEngine;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{
    public void StartButton(string Lobby)
    {
        SceneManager.LoadScene ("Lobby");
    }

    public void SPButton(string SampleScene)
    {
        SceneManager.LoadScene("SampleScene");
    }
}
