using UnityEngine;

public class BladeController : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private AudioClip shootSound;

    private void Start()
    {
        // One shot per blade, so a four-blade throw is heard four times. Played through the
        // AudioManager because the blade is destroyed well before the clip ends - and that keeps
        // it on the Master mixer group, so the volume slider governs it.
        if (shootSound != null) AudioManager.PlayClipAtPoint(shootSound, transform.position);
    }

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }
}
