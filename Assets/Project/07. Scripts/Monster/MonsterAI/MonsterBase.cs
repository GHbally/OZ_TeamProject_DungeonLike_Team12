using UnityEditor.VersionControl;
using UnityEngine;

public abstract class MonsterBase : MonoBehaviour
{
    [Header("몬스터 능력치")]
    public MonsterType monsterType;
    public int maxHp = 100; //최대 체력
    protected int currentHp; //현재 체력
    public float moveSpeed = 3f; //이동속도
    public Transform player; //플레이어 위치
    public MonsterState currentState;
    protected Rigidbody2D rb;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void OnEnable() 
    {
        currentHp = maxHp; //체력 초기화
        currentState= MonsterState.Chase; //추적시작

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    protected virtual void Update()
    {
        switch (currentState)
        {
            case MonsterState.Chase:
                Chase();
                break;
            case MonsterState.Attack:
                Attack();
                break;
        }
    }

    protected abstract void Chase();
    protected abstract void Attack();

    //피격
    public virtual void TakeDamage(int damamge)
    {
        currentHp -= damamge;
        if (currentHp <= 0)
        {
            Die();
        }
    }

    //사망
    protected virtual void Die()
    {
        currentState = MonsterState.Dead;

        //경험치 드랍
        DropManager.Instance.DropExp(transform.position);

        PoolManager.Instance.ReturnMonster(gameObject);
        FindFirstObjectByType<WaveManager>().MonsterDead();
    }
}
