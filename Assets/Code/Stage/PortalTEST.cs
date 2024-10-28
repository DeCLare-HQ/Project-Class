using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalTEST : MonoBehaviour
{
    public string targetSceneName = "ST1"; // 이동할 씬 이름

    // 트리거에 플레이어가 닿았는지 확인
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player is inside the portal."); // 포탈에 플레이어가 있는지 확인
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Debug.Log("UpArrow key pressed."); // 위쪽 방향키가 눌렸는지 확인
                ChangeScene();
            }
        }
    }

    // 씬 전환
    private void ChangeScene()
    {
        SceneManager.LoadScene(targetSceneName);
    }
}