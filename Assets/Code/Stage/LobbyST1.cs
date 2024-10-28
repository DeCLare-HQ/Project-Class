using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyST1 : MonoBehaviour
{
    public string targetSceneName = "ST1"; // 이동할 씬 이름

    private void OnTriggerStay2D(Collider2D collision)
    {
        // 플레이어가 포탈에 닿았고 위쪽 방향키 입력 시 씬 전환
        if (collision.gameObject.CompareTag("Player") && Input.GetKeyDown(KeyCode.UpArrow))
        {
            StartCoroutine(LoadSceneAsync()); // 비동기적으로 씬 로드
        }
    }

    private IEnumerator LoadSceneAsync()
    {
        // 비동기적으로 씬 로드, 자동으로 활성화하지 않도록 설정
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetSceneName);
        asyncLoad.allowSceneActivation = false;

        // 씬이 완전히 로드될 때까지 대기
        while (!asyncLoad.isDone)
        {
            // 로드가 완료되었고, 활성화 준비가 된 경우
            if (asyncLoad.progress >= 0.9f)
            {
                // 씬을 활성화하여 모든 오브젝트가 생성되도록 설정
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}