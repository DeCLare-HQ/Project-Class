using System.Collections;
using UnityEngine;

public class PlayerEffect : MonoBehaviour
{
    // 근접 공격 이펙트 관련 설정
    public GameObject meleeEffectPrefab;  // 근접 공격 이펙트 프리팹 참조
    public Transform meleeEffectSpawnPoint; // 근접 공격 이펙트 생성 위치
    public float meleeEffectDuration = 0.6f;  // 근접 공격 이펙트 유지 시간 (초)
    public float meleeEffectDelay = 0.2f;  // 근접 공격 이펙트 생성 지연 시간 (초)

    // 원거리 공격 이펙트 관련 설정
    public GameObject rangedEffectPrefab;  // 원거리 공격 이펙트 프리팹 참조
    public Transform rangedEffectSpawnPoint; // 원거리 공격 이펙트 생성 위치
    public float rangedEffectDuration = 0.4f;  // 원거리 공격 이펙트 유지 시간 (초)
    public float rangedEffectDelay = 0.1f;  // 원거리 공격 이펙트 생성 지연 시간 (초)

    // 근접 공격 이펙트 생성 함수
    public void SpawnMeleeEffect()
    {
        if (meleeEffectPrefab != null && meleeEffectSpawnPoint != null)
        {
            // 근접 공격 이펙트 생성 지연 시작
            StartCoroutine(SpawnEffectWithDelay(meleeEffectPrefab, meleeEffectSpawnPoint, meleeEffectDelay, meleeEffectDuration));
        }
        else
        {
            Debug.LogWarning("MeleeEffect 프리팹 또는 이펙트 생성 위치가 설정되지 않았습니다.");
        }
    }

    // 원거리 공격 이펙트 생성 함수
    public void SpawnRangedEffect()
    {
        if (rangedEffectPrefab != null && rangedEffectSpawnPoint != null)
        {
            // 원거리 공격 이펙트 생성 지연 시작
            StartCoroutine(SpawnEffectWithDelay(rangedEffectPrefab, rangedEffectSpawnPoint, rangedEffectDelay, rangedEffectDuration));
        }
        else
        {
            Debug.LogWarning("RangedEffect 프리팹 또는 이펙트 생성 위치가 설정되지 않았습니다.");
        }
    }

    // 지연 시간 후 이펙트를 생성하는 코루틴 (근접, 원거리 둘 다 사용)
    private IEnumerator SpawnEffectWithDelay(GameObject effectPrefab, Transform effectSpawnPoint, float delay, float duration)
    {
        // 지연 시간 동안 대기
        yield return new WaitForSeconds(delay);

        // 이펙트 생성
        GameObject effect = Instantiate(effectPrefab, effectSpawnPoint.position, Quaternion.identity);

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
        StartCoroutine(DestroyEffectAfterDuration(effect, duration));
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
