using System.Collections;
using UnityEngine;

public class PlayerEffect : MonoBehaviour
{
    public GameObject meleeEffectPrefab;  // 근접 공격 이펙트 프리팹 참조

    public Transform effectSpawnPoint; // 이펙트가 생성될 위치
    
    public float effectDuration = 0.6f;  // 이펙트가 유지될 시간 (초)
    public float effectDelay = 0.2f;  // 이펙트 생성 지연 시간 (초)
    
    // 근접 공격 이펙트 생성 함수
    public void SpawnMeleeEffect()
    {
        if (meleeEffectPrefab != null && effectSpawnPoint != null)
        {
            // 이펙트 생성 지연 시작
            StartCoroutine(SpawnEffectWithDelay(effectDelay));
        }
        else
        {
            Debug.LogWarning("MeleeEffect 프리팹 또는 이펙트 생성 위치가 설정되지 않았습니다.");
        }
    }

    // 지연 시간 후 이펙트를 생성하는 코루틴
    private IEnumerator SpawnEffectWithDelay(float delay)
    {
        // 지연 시간 동안 대기
        yield return new WaitForSeconds(delay);

        // 이펙트 생성 위치
        GameObject effect = Instantiate(meleeEffectPrefab, effectSpawnPoint.position, Quaternion.identity);

        // 이펙트가 플레이어를 따라다니도록 부모 설정
        effect.transform.SetParent(effectSpawnPoint);

        // 이펙트의 크기를 부모의 방향에 맞춰 반전 가능
        Vector3 effectScale = effect.transform.localScale;
        if (effectSpawnPoint.localScale.x < 0)
        {
            effectScale.x = -Mathf.Abs(effectScale.x);  // 반전
        }
        else
        {
            effectScale.x = Mathf.Abs(effectScale.x);  // 정상
        }
        effect.transform.localScale = effectScale;

        // 지정된 시간이 지난 후 이펙트 삭제
        StartCoroutine(DestroyEffectAfterDuration(effect, effectDuration));
    }

    // 지정된 시간 후 이펙트를 삭제하는 코루틴
    private IEnumerator DestroyEffectAfterDuration(GameObject effect, float duration)
    {
        // 지정된 시간 동안 대기
        yield return new WaitForSeconds(duration);

        // 이펙트 삭제
        Destroy(effect);
    }
}
