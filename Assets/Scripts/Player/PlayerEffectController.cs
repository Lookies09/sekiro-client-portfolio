using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Pool;

public class EffectPool<T> where T : Component
{
    private IObjectPool<T> pool;

    public EffectPool(T prefab, Transform parent = null, int maxSize = 10)
    {
        pool = new ObjectPool<T>(
            () => CreateObject(prefab, parent),  // 오브젝트 생성
            OnGetObject,                        // 풀에서 꺼낼 때 호출
            OnReleaseObject,                    // 풀로 반환할 때 호출
            OnDestroyObject,                    // 오브젝트 파괴 시 호출
            maxSize: maxSize                    // 최대 크기
        );
    }

    // 오브젝트 생성
    private T CreateObject(T prefab, Transform parent)
    {
        var obj = Object.Instantiate(prefab, parent);
        return obj;
    }

    // 오브젝트 풀에서 꺼낼 때 호출
    private void OnGetObject(T obj)
    {
        obj.gameObject.SetActive(true);
    }

    // 오브젝트 풀에 반환할 때 호출
    private void OnReleaseObject(T obj)
    {
        obj.gameObject.SetActive(false);
    }

    // 오브젝트 파괴 시 호출
    private void OnDestroyObject(T obj)
    {
        Object.Destroy(obj.gameObject);
    }

    // 오브젝트를 풀에서 꺼내는 메서드
    public T Get() => pool.Get();

    // 오브젝트를 풀로 반환하는 메서드
    public void Release(T obj) => pool.Release(obj);
}


public class PlayerEffectController : MonoBehaviour
{
    [Header("오브젝트 풀 최대 크기")]
    [SerializeField] private int maxPoolSize = 10;


    [SerializeField] private ParticleSystem hitEffectPrefab;
    [SerializeField] private ParticleSystem parryEffectPrefab;

    // 각각의 이펙트 풀 관리
    private EffectPool<ParticleSystem> hitEffectPool;
    private EffectPool<ParticleSystem> parryEffectPool;

    private void Awake()
    {
        // 각 이펙트 풀을 별도로 생성
        hitEffectPool = new EffectPool<ParticleSystem>(hitEffectPrefab,null, maxPoolSize);
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

    // 패링 이펙트 재생
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

