using UnityEngine;

public class PlayerLevelSystem : MonoBehaviour
{
    [Header("레벨 및 경험치 스탯")]
    [SerializeField] private int currentLevel = 1;  //현재 레벨
    [SerializeField] private float currentExp = 0f; //현재 경험치
    [SerializeField] private float maxExp = 100.0f; //다음 레벨업까지 필요한 요구 경험치

    //다른 스크립트에서 읽을 수 있게 프로퍼티
    public int CurrentLevel => currentLevel;
    public float CurrentExp => currentExp;
    public float MaxExp => maxExp;

    private LevelUpManager uiManager;
    //플레이어 상태 알 수 있게 코어 연결
    private PlayerBase playerBase;
    private void Awake()
    {
        uiManager = FindFirstObjectByType<LevelUpManager>();
    }
    void Start()
    {
        playerBase = GetComponent<PlayerBase>();
        //시작시 필요 경험치
        CalculateMaxExp();

        if (HUDController.Instance != null)
        {
            HUDController.Instance.UpdateEXP(currentExp, maxExp); //시작 시 경험치바 초기화

            HUDController.Instance.UpdateLevel(currentLevel);
        }
    }

    //경험치 구슬 먹었을 때
    public void EarnExp(float amount)
    {
        //플레이어가 이미 죽은 상태면 리턴
        if (playerBase != null && playerBase.IsDead) return;

        currentExp += amount; //경험치 누적

        if (HUDController.Instance != null)
        {
            HUDController.Instance.UpdateEXP(currentExp, maxExp); //실시간 경험치 게이지 상승
        }

        //현재경험치가 요구경험치 이상이면 
        while (currentExp >= maxExp)
        {
            //레벨업
            LevelUp();
        }
    }

    //레벨업 로직
    private void LevelUp()
    {
        //찼던 경험치만큼 요구량을 깎아서 잔여 경험치 이월
        currentExp -= maxExp;
        currentLevel++; //레벨 1 올리기

        //필요 경험치 불러오기
        CalculateMaxExp();

        if (HUDController.Instance != null)
        {
            HUDController.Instance.UpdateEXP(currentExp, maxExp); //레벨업 후 바 리셋 및 이월 적용

            HUDController.Instance.UpdateLevel(currentLevel);
        }

        //레벨업 시 스킬 찍는 선택지
        TriggerSkillSelectionWindow();
    }

    //레벨 증가할때마다 다음 레벨업에 필요한 경험치 요구량 늘려줄 메서드
    private void CalculateMaxExp()
    {
        if (currentLevel == 1)
        {
            //1레벨땐 100으로 시작
            maxExp = 100.0f;
        }
        //1레벨 아니면 이후부턴
        else
        {
            //경험치 요구량을 1.2배로 늘린후 Mathf.Round로 반올림 처리
            maxExp = Mathf.Round(maxExp * 1.2f);
        }
    }

    //스킬 선택 UI불러와줄 메서드
    private void TriggerSkillSelectionWindow()
    {   
        if (uiManager != null)
        {
            uiManager.OpenLevelUpUI();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            uiManager.CloseLevelUpUI();
        }
    }
}//
