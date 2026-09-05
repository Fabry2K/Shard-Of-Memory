using UnityEngine;

public class RestGlowEffect : MonoBehaviour
{
    [SerializeField] private float duration = 0.8f;
    [SerializeField] private float startScale = 0.4f;
    [SerializeField] private float endScale = 1.6f;

    private SpriteRenderer sr;
    private float timer;
    private float startAlpha;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        transform.localScale = Vector3.one * startScale;
        startAlpha = sr.color.a;
    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / duration);

        float scale = Mathf.Lerp(startScale, endScale, t);
        transform.localScale = Vector3.one * scale;

        Color c = sr.color;
        c.a = Mathf.Lerp(startAlpha, 0f, t);
        sr.color = c;

        if (t >= 1f)
        {
            Destroy(gameObject);
        }
    }
}
