using UnityEngine;
using System.Collections;

public class WaveClaerUI : MonoBehaviour
{
    [Header("웨이브 클리어 UI")]
    [SerializeField] private GameObject waveClearImage;

    [Header("스테이지 클리어 UI")]
    [SerializeField] private RectTransform stageClearUI;

    [Header("스테이지 클리어 UI 이동 설정")]
    [SerializeField] private float stageClearMoveDistance = 500f;
    [SerializeField] private float stageClearMoveDuration = 0.5f;
    private Vector2 stageClearStartPosition;

    private void Awake()
    {
        // 스테이지 클리어 UI의 처음 위치를 저장한다.
        // 나중에 다시 보여줄 때 원래 위치로 되돌리기 위해 필요하다.
        if (stageClearUI != null)
        {
            stageClearStartPosition = stageClearUI.anchoredPosition;
        }

        // 게임 시작 시 UI는 전부 꺼둔다.
        HideAll();
    }

    public void ShowWaveClear()
    {
        // 다른 UI가 켜져 있을 수 있으므로 먼저 전부 끈다.
        HideAll();

        // 웨이브 클리어 이미지를 켠다.
        if (waveClearImage != null)
        {
            waveClearImage.SetActive(true);
        }
    }

    public void HideWaveClear()
    {
        // 웨이브 클리어 이미지를 끈다.
        if (waveClearImage != null)
        {
            waveClearImage.SetActive(false);
        }
    }

    public void ShowStageClear()
    {
        HideAll();

        if (stageClearUI == null)
        {
            return;
        }

        // 스테이지 클리어 UI를 원래 위치로 되돌린다.
        stageClearUI.anchoredPosition = stageClearStartPosition;

        // 스테이지 클리어 UI를 켠다.
        stageClearUI.gameObject.SetActive(true);
    }

    public IEnumerator MoveStageClearDown()
    {
        if (stageClearUI == null)
        {
            yield break;
        }

        Vector2 startPosition = stageClearUI.anchoredPosition;

        Vector2 endPosition =
            startPosition + Vector2.down * stageClearMoveDistance;

        float timer = 0f;

        while (timer < stageClearMoveDuration)
        {
            timer += Time.unscaledDeltaTime;

            float t =
                Mathf.Clamp01(timer / stageClearMoveDuration);

            stageClearUI.anchoredPosition =
                Vector2.Lerp(
                    startPosition,
                    endPosition,
                    t
                );

            yield return null;
        }

        stageClearUI.anchoredPosition = endPosition;

        // 아래로 내려간 뒤 UI를 끈다.
        stageClearUI.gameObject.SetActive(false);
    }

    public void HideAll()
    {
        if (waveClearImage != null)
        {
            waveClearImage.SetActive(false);
        }

        if (stageClearUI != null)
        {
            stageClearUI.gameObject.SetActive(false);
        }
    }
}
