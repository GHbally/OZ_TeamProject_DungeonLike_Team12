using Unity.VisualScripting;
using UnityEngine; // Unity 기능 사용

public class Portal : MonoBehaviour
{
    public StageManager stageManager; // 다음 스테이지를 시작할 StageManager 연결

    public Vector3 spawnPosition = new Vector3(-4.4f, -9.7f, 0f); // 포탈 이동 후 플레이어가 도착할 위치

    [Header("상호작용 UI")]
    [SerializeField] private GameObject pressEUI; // 포탈 근처에 있을 때 보여줄 Press E UI

    [Header("상호작용 키")]
    [SerializeField] private KeyCode interactKey = KeyCode.E; // 다음 스테이지 이동 키

    private Transform playerInRange; // 현재 포탈 범위 안에 있는 플레이어
    private bool isPlayerInRange = false; // 플레이어가 포탈 범위 안에 있는지 확인
    private bool isUsed = false; // 포탈 중복 사용 방지

    private void Awake()
    {
        // Press E UI 꺼두기
        HidePressEUI();
    }

    private void OnEnable()
    {
        // 포탈이 다시 켜질 때마다 다시 사용할 수 있게 초기화
        isUsed = false;
        isPlayerInRange = false;
        playerInRange = null;

        HidePressEUI();
    }

    private void Update()
    {
        // 플레이어가 포탈 범위 안에 없으면 E키를 눌러도 실행하지 않음
        if (!isPlayerInRange)
        {
            return;
        }

        // 이미 사용한 포탈이면 중복 실행X
        if (isUsed)
        {
            return;
        }

        // E키를 눌렀을 때만 다음 스테이지로 넘어감
        if (Input.GetKeyDown(interactKey))
        {
            UsePortal();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) // 플레이어가 포탈에 닿았을 때 실행
    {
        if (!other.CompareTag("Player")) // 닿은 오브젝트가 Player가 아니면
            return; // 아래 코드 실행하지 않음

        // 플레이어가 포탈 범위 안에 들어왔음
        playerInRange = other.transform;
        isPlayerInRange = true;

        // 아직 포탈을 사용하지 않았다면 Press E UI를 보여줌
        if (!isUsed)
        {
            ShowPressEUI();
        }
    }

    private void OnTriggerExit2D(Collider2D other) // 플레이어가 포탈 범위에서 나갔을 때
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        // 플레이어가 포탈 범위 밖으로 나갔다고 기록한다.
        playerInRange = null;
        isPlayerInRange = false;

        // 포탈에서 멀어지면 Press E UI를 숨긴다.
        HidePressEUI();
    }

    private void UsePortal()
    {
        // 포탈 중복 실행 방지
        isUsed = true;

        // 이동을 시작하면 Press E UI를 숨긴다.
        HidePressEUI();

        // 플레이어 스폰 포인트가 이상하면 주석 해제
        if (playerInRange != null)
        {
            playerInRange.position = spawnPosition;
        }

        if (stageManager != null)
        {
            stageManager.NextStage();
        }
    }
    
    private void ShowPressEUI()
    {
        if(pressEUI != null)
        {
            pressEUI.SetActive(true);
        }
    }
    
    private void HidePressEUI()
    {
        if(pressEUI != null)
        {
            pressEUI.SetActive(false);
        }
    }
}
        //other.transform.position = spawnPosition; // 플레이어를 지정한 위치로 이동

        ////여기 코드를 나중에 씬이 바뀔때 교체할 것
        //stageManager.NextStage(); // 스테이지 번호 증가 후 다음 웨이브 시작