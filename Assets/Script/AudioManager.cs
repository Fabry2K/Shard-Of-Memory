using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioMixer audioMixer;

    public float CurrentVolume { get; private set; }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);


        CurrentVolume = PlayerPrefs.GetFloat("Volume", -30f);

        audioMixer.SetFloat("Volume", CurrentVolume);
    }


    public void SetVolume(float volume)
    {
        CurrentVolume = volume;

        audioMixer.SetFloat("Volume", volume);

        PlayerPrefs.SetFloat("Volume", volume);
        PlayerPrefs.Save();
    }
}