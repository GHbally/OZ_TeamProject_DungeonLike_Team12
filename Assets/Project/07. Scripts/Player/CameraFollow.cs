//[카메라 플레이어 추적]
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("카메라 대상 및 부드러운 정도")]
    [SerializeField] private Transform target;
    //카메라 추적을 부드럽게 해 유저의 멀미를 줄이고, 대쉬를 사용 시 역동적인 느낌을 주게함
    [SerializeField] private float smoothTime = 0.15f; //값이 작을 수록 칼 같이 따라 붙음

    //카메라 시작 속도 0
    private Vector3 currentVelocity = Vector3.zero;
    //카메라 Z축 깊이값
    private float initialZ;
    void Start()
    {
        //게임 시작 후 현재 카메라의 Z값을 저장 (카메라 앞뒤로 흔들림 방지)
        initialZ = transform.position.z;
    }

    void LateUpdate()
    {
        //타겟(플레이어)이 없으면 리턴
        if (target == null) return;

        //타겟(플레이어)의 X축,Y축 위치를 복사하고 Z축은 카메라 깊이값(X축 위치, Y축 위치, Z축)
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, initialZ);

        //SmoothDamp()로 부드러운 감속 이동(현재 위치, 타겟위치, 실시간 이속 참조, 목적지 도달까지 걸리는 시간)
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);
    }
}
