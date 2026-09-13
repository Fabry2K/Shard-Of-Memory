using UnityEngine;

public class FocusRingBig : MonoBehaviour
{
    [SerializeField] private GameObject effect1;
    [SerializeField] private GameObject effect2;
    [SerializeField] private GameObject effect3;

    [Header("Sounds")]
    [SerializeField] private AudioClip appearSound;
    [SerializeField] private AudioClip explodeSound;

    private GameObject effect1Instance;
    private GameObject effect2Instance;
    private GameObject effect3Instance;

    // Played through the AudioManager rather than a local AudioSource: these rings destroy
    // themselves, which would cut their own sound off, and this routes through the Master mixer
    // group so the volume slider still governs them.
    private void Play(AudioClip clip)
    {
        if (clip != null) AudioManager.PlayClipAtPoint(clip, transform.position);
    }

    public void SpawnEffect1()
    {
        if (effect1Instance == null)
            effect1Instance = Instantiate(effect1, transform.position, Quaternion.identity);

        Play(appearSound);
    }

    public void SpawnEffect2()
    {
        if (effect2Instance == null)
            effect2Instance = Instantiate(effect2, transform.position, Quaternion.identity);
    }

    public void SpawnEffect3()
    {
        // effect3 is the expanding shockwave, the one that actually hurts - so this is the bang.
        if (effect3Instance == null)
            effect3Instance = Instantiate(effect3, transform.position, Quaternion.identity);

        Play(explodeSound);
    }

    // Richiamabile tramite Animation Event
    public void DestroyAll()
    {
        if (effect1Instance != null)
            Destroy(effect1Instance);

        if (effect2Instance != null)
            Destroy(effect2Instance);

        if (effect3Instance != null)
            Destroy(effect3Instance);

        Destroy(gameObject);
    }
}