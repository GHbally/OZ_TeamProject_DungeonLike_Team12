using UnityEngine; // Unity 기능 사용

public class Portal : MonoBehaviour
{
    public StageManager stageManager; // 다음 스테이지를 시작할 StageManager 연결

    public Vector3 spawnPosition = new Vector3(-4.4f, -9.7f, 0f); // 포탈 이동 후 플레이어가 도착할 위치

    private void OnTriggerEnter2D(Collider2D other) // 플레이어가 포탈에 닿았을 때 실행
    {
        if (!other.CompareTag("Player")) // 닿은 오브젝트가 Player가 아니면
            return; // 아래 코드 실행하지 않음

        other.transform.position = spawnPosition; // 플레이어를 지정한 위치로 이동

        //여기 코드를 나중에 씬이 바뀔때 교체할 것
        stageManager.NextStage(); // 스테이지 번호 증가 후 다음 웨이브 시작
        
    }
}