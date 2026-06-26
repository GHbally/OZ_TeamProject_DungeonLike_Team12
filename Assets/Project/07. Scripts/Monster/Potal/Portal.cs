using UnityEngine;

public class Portal : MonoBehaviour
{
    public StageManager stageManager; // StageManager 연결

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) // 닿은 오브젝트가 Player가 아니면
            return; // 실행하지 않음

        stageManager.NextStage(); // 다음 스테이지로 이동
    }
}