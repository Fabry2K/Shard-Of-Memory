using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    [SerializeField] private float floatAmplitude = 0.25f;
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private AudioClip pickupSound;

    private Vector3 basePosition;
    private bool collected;

    private void Start()
    {
        basePosition = transform.position;
    }

    private void Update()
    {
        float offset = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = basePosition + new Vector3(0f, offset, 0f);
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (collected) return;
        if (!_other.CompareTag("Player")) return;

        collected = true;

        if (pickupSound != null)
        {
            AudioManager.PlayClipAtPoint(pickupSound, transform.position);
        }

        if (GameManager.Instance != null) GameManager.Instance.hasKey = true;

        Destroy(gameObject);
    }
}
