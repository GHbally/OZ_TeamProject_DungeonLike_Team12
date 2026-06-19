//기본 일회성 투사체
using UnityEngine;

// 이 스크립트가 붙은 Gameobj에 Collider2D와 Rigidbody2D가 반드시 존재하도록 함.
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]

public class SkillProjectlie : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f; //투사체 비행 속도
    [SerializeField] private float lifeTime = 1f;   //허공에서 버틸 최대 시간
    [SerializeField] private LayerMask enemyLayer;  //충돌할 적 레이어

    private Vector2 moveDirection;                  //날아갈 방향 벡터
    private DamageInfo1 damageInfo;                 //데미지 정보
    private float remainingLifeTime;                //남은 수명 시간 카운터
    private bool isInitialized;                     //발사 준비 완료 스위치

    //투사체 스폰시 초기화 함수
    public void Initialize(Vector2 direction, DamageInfo1 newDamageInfo)
    {
        moveDirection = direction.normalized;       //방향 정리
        damageInfo = newDamageInfo;                 //데미지 정보 내 저장
        remainingLifeTime = lifeTime;               //투사체 수명
        isInitialized = true;                       //스위치 켜기
        RotateToDirection();                        //날아가는 방향으로 이미지 돌리기
    }

    private void Update()
    {
        if(!isInitialized)
        {
            return;
        }
        //투사체 등속도로 이동
        transform.position += (Vector3)(moveDirection * moveSpeed * Time.deltaTime);
        remainingLifeTime -= Time.deltaTime;

        if(remainingLifeTime < 0f)
        {
            //시간 내에 못맞추면 파괴
            Destroy(gameObject);
        }
    }

    //적의 콜라이더와 트리거 충돌이 일어났을 때
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isInitialized)
        {
            return;
        }
        if (!IsInEnemyLayer(collision.gameObject.layer))
        {
            return;
        }

        IDamageable1 damageable = collision.GetComponentInParent<IDamageable1>();
        if (damageable == null)
        {
            return;
        }

        damageable.TakeDamage(damageInfo); //몬스터에게 데미지
    }

    //투사체의 앞부분이 날아가는 방향을 보도록
    private void RotateToDirection()
    {
        if (moveDirection.sqrMagnitude <= 0f)
        {
            return;
        }

        float angle = Mathf.Atan2(moveDirection.y,moveDirection.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    // 전달받은 레이어가 enemyLayer에 포함되어 있는지 확인
    private bool IsInEnemyLayer(int layer)
    {
        return (enemyLayer.value & (1 << layer)) != 0;
    }
}
