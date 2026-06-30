using UnityEngine;

public class EnemyEffectController : MonoBehaviour
{
    [Header("오브젝트 풀 최대 크기")]
    [SerializeField] private int maxPoolSize = 10;

    [SerializeField] private ParticleSystem hitEffectPrefab;
    [SerializeField] private ParticleSystem parryEffectPrefab;
    [SerializeField] private ParticleSystem jumpEffectPrefab;
    [SerializeField] private ParticleSystem walkEffectPrefab;

    // 각각의 이펙트 풀 관리
    private EffectPool<ParticleSystem> hitEffectPool;
    private EffectPool<ParticleSystem> parryEffectPool;
    private EffectPool<ParticleSystem> jumpEffectPool;
    private EffectPool<ParticleSystem> walkEffectPool;

    private void Awake()
    {
        // 각 이펙트 풀을 별도로 생성
        hitEffectPool = new EffectPool<ParticleSystem>(hitEffectPrefab, null, maxPoolSize);
        parryEffectPool = new EffectPool<ParticleSystem>(parryEffectPrefab, null, maxPoolSize);
    }

    // 히트 이펙트 재생
    public void OnPlayHitEffect(Vector3 pos)
    {
        ParticleSystem hitEffect = hitEffectPool.Get();
        hitEffect.transform.position = pos;
        hitEffect.Play();

        // 이펙트가 끝난 후 풀에 반환
        StartCoroutine(ReturnParticleEffectAfterDelay(hitEffect, hitEffectPool));
    }

    // 점프 이펙트 재생
    public void OnPlayParryEffect(Vector3 pos)
    {
        ParticleSystem parryEffect = parryEffectPool.Get();
        parryEffect.transform.position = pos;
        parryEffect.Play();

        // 이펙트가 끝난 후 풀에 반환
        StartCoroutine(ReturnParticleEffectAfterDelay(parryEffect, parryEffectPool));
    }

    // 이펙트가 끝난 후 풀에 반환
    private System.Collections.IEnumerator ReturnParticleEffectAfterDelay(ParticleSystem particleSystem, EffectPool<ParticleSystem> pool)
    {
        // 파티클 이펙트가 끝날 때까지 기다리고, 그 후 반환
        yield return new WaitForSeconds(particleSystem.main.duration);
        pool.Release(particleSystem);
    }
}
