using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{

    [SerializeField] private Slider volumeSlider;


    private void OnEnable()
    {
        if (AudioManager.Instance != null)
        {
            volumeSlider.value = AudioManager.Instance.CurrentVolume;
        }
    }


    public void SetVolume(float volume)
    {
        AudioManager.Instance.SetVolume(volume);
    }


    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }


    public void Quit()
    {
        Application.Quit();
    }
}