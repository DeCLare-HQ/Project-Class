using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    public float damage = 20.0f; // 플레이어에게 줄 데미지

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어와 충돌 시 데미지를 전달
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage); // 플레이어에게 데미지 전달
                Debug.Log("Spike hit player! Damage: " + damage);
            }
        }
    }
}
