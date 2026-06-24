using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Slider bgmSlider;
    public Toggle fullScreenToggle;

    void Start()
    {
        // 게임 시작 시 저장된 값 불러오기
        bgmSlider.value = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
        fullScreenToggle.isOn = PlayerPrefs.GetInt("FullScreen", 1) == 1;
    }

    public void SaveSettings()
    {
        // 설정 저장
        PlayerPrefs.SetFloat("BGMVolume", bgmSlider.value);
        PlayerPrefs.SetInt("FullScreen", fullScreenToggle.isOn ? 1 : 0);
        PlayerPrefs.Save();

        gameObject.SetActive(false); // 창 닫기
    }
}
