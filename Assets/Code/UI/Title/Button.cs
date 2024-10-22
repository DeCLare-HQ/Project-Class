using UnityEngine;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{
    public void StartButton(string SampleScene)
    {
        SceneManager.LoadScene ("SampleScene");
    }
}
