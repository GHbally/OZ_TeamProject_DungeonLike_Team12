using UnityEngine;
using System.Collections;

public class WaveClaerUI : MonoBehaviour
{
    [Header("웨이브 클리어 Image")]
    [SerializeField] private GameObject waveClearImage;

    [Header("스테이지 클리어 Image")]
    [SerializeField] private GameObject stageClearImage;

    private void Awake()
    {
        // 게임 시작 시 두 UI를 모두 꺼둔다.
        HideAll();
    }

    public void ShowWaveClear()
    {
        // 다른 클리어 UI가 켜져 있을 수 있으므로 먼저 전부 끈다.
        HideAll();

        // 웨이브 클리어 UI만 켠다.
        if (waveClearImage != null)
        {
            waveClearImage.SetActive(true);
        }
    }

    public void HideWaveClear()
    {
        // 웨이브 클리어 UI를 끈다.
        if (waveClearImage != null)
        {
            waveClearImage.SetActive(false);
        }
    }

    public void ShowStageClear()
    {
        // 다른 클리어 UI가 켜져 있을 수 있으므로 먼저 전부 끈다.
        HideAll();

        // 스테이지 클리어 UI만 켠다.
        if (stageClearImage != null)
        {
            stageClearImage.SetActive(true);
        }
    }

    public void HideStageClear()
    {
        // 스테이지 클리어 UI를 끈다.
        if (stageClearImage != null)
        {
            stageClearImage.SetActive(false);
        }
    }

    public void HideAll()
    {
        // 웨이브 클리어 UI를 끈다.
        if (waveClearImage != null)
        {
            waveClearImage.SetActive(false);
        }

        // 스테이지 클리어 UI를 끈다.
        if (stageClearImage != null)
        {
            stageClearImage.SetActive(false);
        }
    }
}
