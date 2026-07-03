using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    // 인스펙터에서 직접 드래그해서 연결할 패널
    [Header("UI 구성요소")]
    public GameObject SkillUI;

    //두트윈으로 커질 전체 배경
    [SerializeField] private GameObject panelRect;

    //꿀렁이며 안착할 Exit 버튼
    [SerializeField] private RectTransform exitButtonRect;

    //순서대로 나올 스킬 목록들
    [SerializeField] private List<RectTransform> skillRows = new List<RectTransform>();


    [Header("두트윈 시간 설정")]
    [SerializeField] private float panelOpenDuration = 0.4f;    //메인패널 열리는데 걸리는 시간
    [SerializeField] private float rowOpenDuration = 0.3f;      //스킬 항목 한 줄당 나타나는 시간
    [SerializeField] private float delayBetweenRows = 0.05f;    //앞둘이 나온 후 다음 줄이 나올때까지 대기텀

    //원래 위치 기억해둘 리스트
    private List<Vector2> originalAnchoredPositions = new List<Vector2>();

    //스킬 항목들 페이드인 위해 컴포넌트 보관해줄 리스트
    private List<CanvasGroup> rowCanvasGroups = new List<CanvasGroup>();
    private Coroutine openRoutine;  //순서대로 나오는 코루틴 제어 및 중복 실행 방지

    private bool isPositionCached = false;

    private void Start()
    {
        rowCanvasGroups.Clear();

        //하위 스킬 목록들의 페이드 인을 위해 CanvasGroup 세팅
        foreach (var row in skillRows)
        {
            if (row != null)
            {
                //오브젝트에 CanvasGroup이 있으면 가져오고, 없으면 실시간으로 붙여서 변수에 저장
                CanvasGroup cg = row.GetComponent<CanvasGroup>() ?? row.gameObject.AddComponent<CanvasGroup>();
                rowCanvasGroups.Add(cg);    //캐싱 전용 리스트에 보관
            }
        }
    }

    //"스킬" 버튼 누르면 실행될 메서드
    public void OpenUserInfo()
    {
        if (SkillUI == null || panelRect == null) return;    //연결된 스킬 UI가 없으면 리턴

        SkillUI.SetActive(true);        //껍데기 오브젝트를 활성화 해서 눈에 보이게 함

        if (!isPositionCached)
        {
            originalAnchoredPositions.Clear();
            foreach (var row in skillRows)
            {
                if (row != null)
                {
                    originalAnchoredPositions.Add(row.anchoredPosition);
                }
            }
            isPositionCached = true; //좌표를 갱신하지 않고 고정
        }

        //이전 연출 초기화 및 청소
        if (openRoutine != null) StopCoroutine(openRoutine);
        panelRect.transform.DOKill();
        if (exitButtonRect != null) exitButtonRect.DOKill();

        //모든 오브젝트 스케일 0, 투명도 0으로 기본 세팅
        panelRect.transform.localScale = Vector3.zero;
        if (exitButtonRect != null) exitButtonRect.localScale = Vector3.zero;

        //모든 자식 스킬 항목들의 크기를 0으로 만들고 알파값 0으로 만들어 숨김
        for (int i = 0; i < skillRows.Count; i++)
        {
            if (skillRows[i] != null)
            {
                skillRows[i].DOKill();
                rowCanvasGroups[i].DOKill();

                //위치가 밀리지 않게 아까 기억해둔 "원래 위치"로 강제 리셋
                skillRows[i].anchoredPosition = originalAnchoredPositions[i];

                skillRows[i].localScale = Vector3.zero;
                rowCanvasGroups[i].alpha = 0f;
            }
        }

        //배경 창 + Exit 버튼 동시에 꿀렁 연출
        Sequence slimeSeq = DOTween.Sequence();

        //배경 패널 꿀렁꿀렁
        slimeSeq.Append(panelRect.transform.DOScale(new Vector3(1.15f, 0.85f, 1f), panelOpenDuration * 0.4f).SetEase(Ease.OutQuad));
        slimeSeq.Append(panelRect.transform.DOScale(new Vector3(0.95f, 1.05f, 1f), panelOpenDuration * 0.3f).SetEase(Ease.InOutQuad));
        slimeSeq.Append(panelRect.transform.DOScale(Vector3.one, panelOpenDuration * 0.3f).SetEase(Ease.OutQuad));

        //Exit 버튼도 꿀렁이면서 같이 등장
        if (exitButtonRect != null)
        {
            Sequence exitSeq = DOTween.Sequence();
            exitSeq.Append(exitButtonRect.DOScale(new Vector3(1.2f, 0.8f, 1f), panelOpenDuration * 0.4f).SetEase(Ease.OutQuad));
            exitSeq.Append(exitButtonRect.DOScale(new Vector3(0.9f, 1.1f, 1f), panelOpenDuration * 0.3f).SetEase(Ease.InOutQuad));
            exitSeq.Append(exitButtonRect.DOScale(Vector3.one, panelOpenDuration * 0.3f).SetEase(Ease.OutQuad));
        }

        //배경 켜진 뒤, 스킬 목록들이 주르륵 튀어나오기
        slimeSeq.OnComplete(() =>
        {
            openRoutine = StartCoroutine(ShowSkillsSequentiallyCo());
        });
    }

    //자식 스킬 항목들을 위에서부터 아래로 주르륵 켜주는 미친 코루틴
    private IEnumerator ShowSkillsSequentiallyCo()
    {
        //리스트에 들어있는 스킬 항목 개수만큼 루프 돌리기
        for (int i = 0; i < skillRows.Count; i++)
        {
            if (skillRows[i] == null) continue; //중간에 비어있는 칸이 있다면 스킵

            //알파값을 1로 만들어서 투명했던 글자들을 부드럽게 노출
            rowCanvasGroups[i].DOFade(1f, rowOpenDuration);

            //크기를 0에서 1로 키우되, Ease.OutBack을 써서 살짝 과장되게 커졌다가 원상복구 시키는 탄성 연출
            skillRows[i].DOScale(Vector3.one, rowOpenDuration).SetEase(Ease.OutBack);

            //한 줄이 튀어나올 때마다 사운드 매니저를 통해 효과음 재생
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySFX("SFX_UIHoverPaperDash01");
            }

            //지정한 딜레이 시간만큼 실시간으로 잠깐 대기 후 다음 줄 루프로 이동
            yield return new WaitForSeconds(delayBetweenRows);
        }
        openRoutine = null; //모든 리스트 출력이 완벽하게 완료되었으므로 코루틴 참조 변수를 비워줌
    }

    //"닫기" 버튼에 연결할 함수
    public void CloseUserInfo()
    {
        if (SkillUI == null) return; //닫을 창이 없으면 리턴

        if (openRoutine != null) StopCoroutine(openRoutine); //혹시 리스트가 주르륵 나오는 도중에 닫았으면 코루틴 즉시 강제 정지

        //닫힐 때는 전체 패널 크기를 0.15초 만에 0으로 압축시키며 사라지게 만듦
        panelRect.transform.DOScale(Vector3.zero, 0.15f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            SkillUI.SetActive(false); //크기가 0이 되어서 완전히 사라진 순간, 실제 게임 오브젝트도 SetActive(false)로 완전히 꺼줌
        });
    }
}
