using UnityEngine;
using UnityEngine.UI;

public class WaveTransitionUI : MonoBehaviour
{
    [Header("웨이브 시작 UI")]
    [SerializeField] private GameObject waveStartPanel;
    [SerializeField] private Text waveStartText;

    [Header("웨이브 종료 UI")]
    [SerializeField] private GameObject waveEndPanel;
    [SerializeField] private Text waveEndText;

    private void Awake()
    {
        // 게임 시작시 UI는 꺼둔다.
        HideAll();
    }

    public void ShowWaveStart(
        int currentWave,
        int maxWave)
    {
        HideAll();

        if(waveStartText != null)
        {
            waveStartText.text = $"Wave {currentWave} / {maxWave} Start";
        }

        if(waveStartPanel != null)
        {
            waveStartPanel.SetActive(true);
        }
    }
    public void ShowWaveEnd(int currentWave, int maxWave)
    {
        HideAll();

        if (waveEndText != null)
        {
            waveEndText.text =
                $"Wave {currentWave} / {maxWave} Clear";
        }

        if (waveEndPanel != null)
        {
            waveEndPanel.SetActive(true);
        }
    }
    public void HideAll()
    {
        if(waveStartPanel != null)
        {
            waveStartPanel.SetActive(false);
        }
        if(waveEndPanel != null)
        {
            waveEndPanel.SetActive(false);
        }
    }
}
