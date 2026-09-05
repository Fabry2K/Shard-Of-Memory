using UnityEngine;

public class CameraFollow : MonoBehaviour
{


    [SerializeField] private float followSpeed = 0.1f;
    [SerializeField] private Vector3 offset;

    private float shakeTimer;
    private float shakeMagnitude;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, PlayerController.Instance.transform.position + offset, followSpeed);

        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;
            Vector2 shakeOffset = Random.insideUnitCircle * shakeMagnitude;
            transform.position += new Vector3(shakeOffset.x, shakeOffset.y, 0f);
        }
    }

    public void Shake(float duration, float magnitude)
    {
        shakeTimer = duration;
        shakeMagnitude = magnitude;
    }
}
