using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject playerPrefab; // 주인공 프리팹
    public Transform spawnPoint; // 스폰 위치

    private void Start()
    {
        if (playerPrefab != null && spawnPoint != null)
        {
            Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity); // 주인공 소환
        }
    }
}
