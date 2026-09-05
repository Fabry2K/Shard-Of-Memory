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

    // Like AudioSource.PlayClipAtPoint, but routed through the Master mixer group so it
    // actually responds to the game's volume controls instead of playing at a fixed level.
    public static void PlayClipAtPoint(AudioClip clip, Vector3 position)
    {
        if (clip == null) return;

        GameObject tempGO = new GameObject("OneShotAudio_" + clip.name);
        tempGO.transform.position = position;
        AudioSource source = tempGO.AddComponent<AudioSource>();
        source.clip = clip;
        source.spatialBlend = 0f;
        source.outputAudioMixerGroup = GetMasterGroup();

        source.Play();
        Destroy(tempGO, clip.length);
    }

    // AudioManager only lives in the Main Menu scene (it persists via DontDestroyOnLoad from there),
    // so Instance can be null when Play is pressed directly inside a level scene. In that case, fall
    // back to whatever mixer group the player's own AudioSource is already using.
    private static AudioMixerGroup GetMasterGroup()
    {
        if (Instance != null && Instance.audioMixer != null)
        {
            var groups = Instance.audioMixer.FindMatchingGroups("Master");
            if (groups.Length > 0) return groups[0];
        }

        if (PlayerController.Instance != null)
        {
            var playerSource = PlayerController.Instance.GetComponent<AudioSource>();
            if (playerSource != null) return playerSource.outputAudioMixerGroup;
        }

        return null;
    }
}