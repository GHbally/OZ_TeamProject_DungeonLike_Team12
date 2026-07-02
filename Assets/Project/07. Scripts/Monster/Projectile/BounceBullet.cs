using UnityEngine;

public class BounceBullet : MonoBehaviour
{
    public float speed = 6f;   // 이동 속도
    public int maxBounce = 3; // 최대 반사 횟수
    public float damage = 15f; // 데미지

    private Vector2 dir;  // 이동 방향
    private int bounceCount;  // 반사 횟수
    private Rigidbody2D rb;  // Rigidbody2D 저장
    private Camera mainCamera; // 메인 카메라 저장

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // 탄환의 Rigidbody2D 가져오기
        mainCamera = Camera.main;  // MainCamera 태그가 붙은 카메라 가져오기

        if (rb != null)
        {
            rb.gravityScale = 0f;    // 중력 제거: 아래로 떨어지지 않게 함
            rb.bodyType = RigidbodyType2D.Kinematic; // 물리 힘으로 밀리지 않게 함
        }
    }


    // 초기화 (보스가 호출)

    public void Init(Vector2 direction)
    {
        dir = direction.normalized; // 보스가 넘겨준 방향 저장
        bounceCount = 0; //튕김 횟수 초기화

        //SFX
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX("SFX_ArcaneSpellNoVocalsv2");
        }
    }

    
    // 이동
    
    private void Update()
    {
        transform.position += (Vector3)(dir * speed * Time.deltaTime); //탄환 이동
        CheckCameraBounce(); // 카메라 화면 끝에 닿았는지 확인
    }

    private void CheckCameraBounce()
    {
        if (mainCamera == null) return; //카메라 없으면 종료

        Vector3 pos = transform.position; //현재 탄환 위치를 저장

        float camHeight = mainCamera.orthographicSize; //카메라 세로 절반 크기
        float camWidth = camHeight * mainCamera.aspect; //카메라 가로 절반 크기

        Vector3 camPos = mainCamera.transform.position; //카메라 현재 위치

        float left = camPos.x - camWidth; //카메라 왼쪽 끝
        float right = camPos.x + camWidth; //카메라 오른쪽 끝
        float top = camPos.y + camHeight; //카메라 아래쪽 끝
        float bottom = camPos.y - camHeight; //카메로 위쪽 끝

        float padding = 0.3f;// 탄환이 화면 안쪽에 머물 여유 거리
        bool bounced = false;// 이번 프레임에 튕겼는지 확인

        // 왼쪽 화면 밖으로 나갔을 때
        if (pos.x < left + padding)
        {
            pos.x = left + padding; // 위치를 화면 안쪽으로 강제 이동
            
            dir.x = Mathf.Abs(dir.x);// 오른쪽 방향으로 반사

            bounced = true; // 튕김 처리
        }

        // 오른쪽 화면 밖으로 나갔을 때
        else if (pos.x > right - padding)
        {
            pos.x = right - padding; // 위치를 화면 안쪽으로 강제 이동

            dir.x = -Mathf.Abs(dir.x); // 왼쪽 방향으로 반사

            bounced = true;// 튕김 처리
        }

        // 아래 화면 밖으로 나갔을 때
        if (pos.y < bottom + padding)
        {
            pos.y = bottom + padding; // 위치를 화면 안쪽으로 강제 이동

            dir.y = Mathf.Abs(dir.y); // 위쪽 방향으로 반사

            bounced = true; // 튕김 처리
        }

        // 위 화면 밖으로 나갔을 때
        else if (pos.y > top - padding)
        {
            pos.y = top - padding; // 위치를 화면 안쪽으로 강제 이동

            dir.y = -Mathf.Abs(dir.y); // 아래쪽 방향으로 반사

            bounced = true;// 튕김 처리
        }

        // 보정된 위치 적용
        transform.position = pos;

        // 튕겼다면
        if (bounced)
        {
            bounceCount++; // 튕긴 횟수 증가
            
            if (bounceCount >= maxBounce) // 최대 튕김 횟수 이상이면
            {
                ReturnToPool(); // 풀로 반환
            }
        }

    }


    // 충돌 처리

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return; // 플레이어가 아니면 무시

        //플레이어 컴포넌트 줍줍
        PlayerMovement movement = other.GetComponent<PlayerMovement>();

        //만약 플레이어가 대쉬 중(무적 상태)이라면 데미지를 주지 않고 그냥 통과
        if (movement != null && movement.IsInvincible)
        {
            return; //무적이면 데미지 처리를 하지 않고 리턴
        }

        PlayerBase player = other.GetComponent<PlayerBase>(); // PlayerBase 가져오기

        if (player != null)
        {
            player.TakeDamage(damage);// 플레이어에게 데미지 주기
        }

        ReturnToPool(); // 플레이어 맞으면 탄환 반환
    }


    // 풀 반환

    void ReturnToPool()
    {
        PoolManager.Instance.ReturnBounceBullet(gameObject);
    }
}
