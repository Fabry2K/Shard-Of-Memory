using UnityEngine;
using UnityEngine.UI;

// A circle drawn as geometry instead of a sprite, so it stays clean at any size - a texture-based
// knob blurs as soon as it is shown larger than its source resolution.
//
// The outer rim is a second ring of vertices at zero alpha, which gives the edge a soft, properly
// antialiased falloff rather than the stair-stepping a bare polygon would have.
public class UICircle : MaskableGraphic
{
    [Tooltip("Sides of the polygon. 64 is already indistinguishable from a circle at button size.")]
    [SerializeField] private int segments = 64;

    [Tooltip("Width in pixels of the faded rim that smooths the edge.")]
    [SerializeField] private float edgeSoftness = 1.5f;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        Rect r = rectTransform.rect;
        float radius = Mathf.Min(r.width, r.height) * 0.5f;
        if (radius <= 0f || segments < 3) return;

        float inner = Mathf.Max(0f, radius - edgeSoftness);
        Vector2 centre = r.center;

        Color32 solid = color;
        Color32 clear = new Color32(solid.r, solid.g, solid.b, 0);

        vh.AddVert(centre, solid, Vector2.zero);

        for (int i = 0; i <= segments; i++)
        {
            float angle = (float)i / segments * Mathf.PI * 2f;
            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            vh.AddVert(centre + dir * inner, solid, Vector2.zero);
            vh.AddVert(centre + dir * radius, clear, Vector2.zero);
        }

        for (int i = 0; i < segments; i++)
        {
            int innerA = 1 + 2 * i;
            int outerA = 2 + 2 * i;
            int innerB = 3 + 2 * i;
            int outerB = 4 + 2 * i;

            vh.AddTriangle(0, innerA, innerB);      // the disc itself
            vh.AddTriangle(innerA, outerA, outerB); // the faded rim
            vh.AddTriangle(innerA, outerB, innerB);
        }
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        segments = Mathf.Clamp(segments, 3, 256);
        edgeSoftness = Mathf.Max(0f, edgeSoftness);
    }
#endif
}
