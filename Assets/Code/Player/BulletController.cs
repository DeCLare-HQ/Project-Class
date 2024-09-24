using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    void Start()
    {
        Invoke ("DestroyTime", 2.0f); // 투사체 자동 파괴 시간
    }

    void DestroyTime()
    {
        Destroy(gameObject); // 투사체 파괴
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            Debug.Log("Hit Platform");
            Destroy(gameObject); // 플랫폼 충돌 시 파괴
        }

        else if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Hit Enemy");
            Destroy(gameObject); // 적 충돌 시 파괴
        }
    }
}
