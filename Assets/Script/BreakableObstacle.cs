using UnityEngine;
using System.Collections;

// Marker + destroy logic for obstacles that a player spell can break in a single hit.
// Add this component directly to the specific instance you want breakable -
// it is not meant to live on a shared prefab asset.
public class BreakableObstacle : MonoBehaviour
{
    [SerializeField] private float dissolveDuration = 0.35f;

    private bool broken;

    public void Break()
    {
        if (broken) return;
        broken = true;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        StartCoroutine(Dissolve());
    }

    private IEnumerator Dissolve()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color startColor = sr != null ? sr.color : Color.white;
        Vector3 startScale = transform.localScale;

        float t = 0f;
        while (t < dissolveDuration)
        {
            t += Time.deltaTime;
            float f = Mathf.Clamp01(t / dissolveDuration);

            if (sr != null)
            {
                Color c = startColor;
                c.a = Mathf.Lerp(startColor.a, 0f, f);
                sr.color = c;
            }
            transform.localScale = Vector3.Lerp(startScale, startScale * 0.6f, f);

            yield return null;
        }

        Destroy(gameObject);
    }
}
