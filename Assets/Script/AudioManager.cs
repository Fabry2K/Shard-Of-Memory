using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("References")]
    public AudioMixer audioMixer;

    public float CurrentVolume { get; private set; } = 1f;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Carica il volume salvato
        CurrentVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        ApplyVolume(CurrentVolume);
    }

    public void SetVolume(float volume)
    {
        CurrentVolume = volume;

        ApplyVolume(volume);

        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();
    }

    private void ApplyVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }
}