using UnityEngine;
using UnityEngine.Pool;

public class TestSkillProjectilePool : MonoBehaviour
{
    [SerializeField] private SkillProjectileBase projectilePrefab;

    [SerializeField] private Transform projectileContainer;

    [SerializeField] private int defaultCapacity = 10;

    [SerializeField] private int maxPoolSize = 100;

    private ObjectPool<SkillProjectileBase> projectilePool;

    private void Awake()
    {
        if (projectilePrefab == null)
        {
            enabled = false;
            return;
        }

        projectilePool = new ObjectPool<SkillProjectileBase>(
            CreateProjectile,
            OnGetProjectile,
            OnReleaseProjectile,
            OnDestroyProjectile,
            collectionCheck: true,
            defaultCapacity: defaultCapacity,
            maxSize: maxPoolSize 
        );
    }

    // 풀이 부족할 때 새로운 투사체 생성
    private SkillProjectileBase CreateProjectile()
    {

    SkillProjectileBase projectile = Instantiate(
        projectilePrefab,
        projectileContainer
    );

    projectile.gameObject.SetActive(false);

    return projectile;
}

    // 풀에서 투사체를 꺼낼 때 실행
    private void OnGetProjectile(
        SkillProjectileBase projectile)
    {
        projectile.gameObject.SetActive(true);
    }

    // 투사체를 풀로 반환할 때 실행
    private void OnReleaseProjectile(
        SkillProjectileBase projectile)
    {
        projectile.gameObject.SetActive(false);

        if (projectileContainer != null)
        {
            projectile.transform.SetParent(
                projectileContainer
            );
        }
    }

    // 풀이 최대 크기를 초과했을 때 제거
    private void OnDestroyProjectile(
        SkillProjectileBase projectile)
    {
        Destroy(projectile.gameObject);
    }

    // 풀에서 투사체를 가져와 발사
    public SkillProjectileBase Spawn(
        Vector3 position,
        Vector2 direction,
        DamageInfo1 damageInfo)
    {
        if(projectilePool == null)
        {
            return null;
        }

        SkillProjectileBase projectile = projectilePool.Get();

        projectile.transform.SetParent(null);
        projectile.transform.position = position;
        projectile.transform.rotation = Quaternion.identity;

        projectile.Initialize(direction, damageInfo, Release);

        return projectile;
    }

    // 사용이 끝난 투사체를 풀로 반환한다.
    private void Release(SkillProjectileBase projectile)
    {
        if(projectile==null||projectilePool == null)
        {
            return;
        }
        projectilePool.Release(projectile);
    }
}
