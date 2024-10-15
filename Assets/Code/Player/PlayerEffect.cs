using System.Collections;
using UnityEngine;

public class PlayerEffect : MonoBehaviour
{
    // 근접 공격 이펙트 관련 설정
    public GameObject meleeEffectPrefab;  // 근접 공격 이펙트 프리팹 참조
    public Transform meleeEffectSpawnPoint; // 근접 공격 이펙트 생성 위치
    public float meleeEffectDuration = 0.5f;  // 근접 공격 이펙트 유지 시간 (초)
    public float meleeEffectDelay = 0.2f;  // 근접 공격 이펙트 생성 지연 시간 (초)

    // 원거리 공격 이펙트 관련 설정
    public GameObject rangedEffectPrefab;  // 원거리 공격 이펙트 프리팹 참조
    public Transform rangedEffectSpawnPoint; // 원거리 공격 이펙트 생성 위치
    public float rangedEffectDuration = 0.4f;  // 원거리 공격 이펙트 유지 시간 (초)
    public float rangedEffectDelay = 0.1f;  // 원거리 공격 이펙트 생성 지연 시간 (초)

    // 점프 이펙트 관련 설정
    public GameObject jumpEffectPrefab;  // 점프 이펙트 프리팹 참조
    public Transform jumpEffectSpawnPoint; // 점프 이펙트 생성 위치
    public float jumpEffectDuration = 0.5f;  // 점프 이펙트 유지 시간 (초)
    public float jumpEffectDelay = 0.0f;  // 점프 이펙트 생성 지연 시간 (초) - 즉시 생성 가능

    // 근접 공격 이펙트 생성 함수
    public void SpawnMeleeEffect()
    {
        if (meleeEffectPrefab != null && meleeEffectSpawnPoint != null)
        {
            // 근접 공격 이펙트 생성 지연 시작
            StartCoroutine(SpawnEffectWithDelay(meleeEffectPrefab, meleeEffectSpawnPoint, meleeEffectDelay, meleeEffectDuration, true));
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
            StartCoroutine(SpawnEffectWithDelay(rangedEffectPrefab, rangedEffectSpawnPoint, rangedEffectDelay, rangedEffectDuration, true));
        }
        else
        {
            Debug.LogWarning("RangedEffect 프리팹 또는 이펙트 생성 위치가 설정되지 않았습니다.");
        }
    }

    // **점프 이펙트 생성 함수 (플레이어를 따라다니지 않음)**
    public void SpawnJumpEffect()
    {
        if (jumpEffectPrefab != null && jumpEffectSpawnPoint != null)
        {
            // 점프 이펙트 생성 지연 시작 (따라다니지 않도록 false 설정)
            StartCoroutine(SpawnEffectWithDelay(jumpEffectPrefab, jumpEffectSpawnPoint, jumpEffectDelay, jumpEffectDuration, false));
        }
        else
        {
            Debug.LogWarning("JumpEffect 프리팹 또는 이펙트 생성 위치가 설정되지 않았습니다.");
        }
    }

    // 지연 시간 후 이펙트를 생성하는 코루틴 (근접, 원거리, 점프 이펙트 사용)
    private IEnumerator SpawnEffectWithDelay(GameObject effectPrefab, Transform effectSpawnPoint, float delay, float duration, bool followPlayer)
    {
        // 지연 시간 동안 대기
        yield return new WaitForSeconds(delay);

        // 이펙트 생성
        GameObject effect = Instantiate(effectPrefab, effectSpawnPoint.position, Quaternion.identity);

        // 이펙트가 플레이어를 따라다니도록 설정 (점프 이펙트는 따라다니지 않음)
        if (followPlayer)
        {
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
        }

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
