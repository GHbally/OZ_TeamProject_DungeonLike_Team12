using System.Collections.Generic;
using UnityEngine;

public class FireTrailProjectile : SkillProjectileBase
{
    [Header("불길 지속 피해 설정")]
    [SerializeField] private float damageMultilplier = 0.25f;
    [SerializeField] private float damageInterval = 0.5f;

    private readonly Dictionary<IDamageable1, float> damageTimers = new();

    protected override void OnInitialized()
    {
        // 불길 장판은 움직이면 안 되므로 이동 속도를 0으로 고정한다.
        SetMoveSpeed(0f);

        SetMaxHitCount(100);

        // 풀에서 다시 꺼낼 때 이전 적 타이머 기록이 남지 않게 초기화 한다.
        damageTimers.Clear();
    }

    protected override void UpdateMovement()
    {
        // 불길 장판은 이동하지 않는다.
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        // 불길에 처음 닿은 적에게 즉시 피해를 줄 수 있게 처리한다.
        TryApplyFireTrailDamage(
            collision,
            immediateDamage: true
        );
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // 불길 위에 계속 있는 적에게 일정 간격으로 피해를 준다.
        TryApplyFireTrailDamage(
            collision,
            immediateDamage: false
        );
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 적이 불길 밖으로 나가면 타이머 기록을 제거한다.
        // 다시 들어오면 새로 들어온 것으로 보고 즉시 피해를 줄 수 있다.
        IDamageable1 damageable =
            collision.GetComponentInParent<IDamageable1>();

        if (damageable == null)
        {
            return;
        }

        if (damageTimers.ContainsKey(damageable))
        {
            damageTimers.Remove(damageable);
        }
    }

    private void TryApplyFireTrailDamage(
        Collider2D collision,
        bool immediateDamage)
    {
        if (!isInitialized || isReleased)
        {
            return;
        }

        // Enemy Layer가 아닌 대상은 무시한다.
        if (!IsInEnemyLayer(collision.gameObject.layer))
        {
            return;
        }

        IDamageable1 damageable =
            collision.GetComponentInParent<IDamageable1>();

        if (damageable == null)
        {
            return;
        }

        // 처음 들어온 적이면 타이머를 등록한다.
        if (!damageTimers.ContainsKey(damageable))
        {
            // 처음 닿은 순간 즉시 피해를 줄지,
            // damageInterval 뒤에 피해를 줄지 결정한다.
            float startTimer = immediateDamage ? 0f : damageInterval;

            damageTimers.Add(
                damageable,
                startTimer
            );
        }

        damageTimers[damageable] -= Time.deltaTime;

        if (damageTimers[damageable] > 0f)
        {
            return;
        }

        DamageInfo1 fireTrailDamageInfo =
            CreateFireTrailDamageInfo();

        damageable.TakeDamage(fireTrailDamageInfo);

        // 다음 피해까지 기다릴 시간을 다시 설정한다.
        damageTimers[damageable] = damageInterval;
    }

    private DamageInfo1 CreateFireTrailDamageInfo()
    {
        // 파이어볼 데미지의 일부를 불길 지속 피해로 사용
        float fireDamage = damageInfo.Damage * damageMultilplier;

        return new DamageInfo1(fireDamage, false, owner);
    }

    protected override void OnBeforeRelease()
    {
        // 풀로 돌아가기 전에 지속 피해 타이머를 비운다.
        damageTimers.Clear();
    }
}
