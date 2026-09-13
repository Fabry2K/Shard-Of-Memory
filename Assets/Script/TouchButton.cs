using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// One on-screen control. It reports held/released rather than clicks, so holding to run or to
// keep healing behaves exactly like holding the key down.
public class TouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Tooltip("Inputs this button feeds: Jump, Attack, Dash, Healing, CastSpell, Interact, or one of Left/Right/Up/Down. Usually one, but Up feeds both the axis and Interact - the same way the W key does on a keyboard.")]
    [SerializeField] private string[] buttons;

    [Tooltip("Greys the button out and ignores taps until the ability pickup has been collected.")]
    [SerializeField] private bool requiresSpellsUnlocked;
    [SerializeField] private Color lockedTint = new Color(0.3f, 0.3f, 0.33f, 1f);

    [SerializeField] private float pressedAlpha = 0.9f;
    [SerializeField] private float releasedAlpha = 0.45f;

    // Graphic rather than Image, so the backdrop can be the mesh-drawn UICircle.
    private Graphic background;
    private Graphic icon;
    private bool appliedLockState;

    private bool Locked
    {
        get
        {
            if (!requiresSpellsUnlocked) return false;
            return GameManager.Instance == null || !GameManager.Instance.spellsUnlocked;
        }
    }

    private void Awake()
    {
        background = GetComponent<Graphic>();
        if (transform.childCount > 0) icon = transform.GetChild(0).GetComponent<Graphic>();

        appliedLockState = Locked;
        Repaint(releasedAlpha);
    }

    private void Update()
    {
        // Only the spell button cares, and only until the pickup is collected.
        if (!requiresSpellsUnlocked) return;

        if (appliedLockState != Locked)
        {
            appliedLockState = Locked;
            Repaint(releasedAlpha);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Locked) return;

        SetHeld(true);
        Repaint(pressedAlpha);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SetHeld(false);
        Repaint(releasedAlpha);
    }

    // Being switched off mid-press (pausing, or the pad hiding) must not leave the input stuck on.
    private void OnDisable()
    {
        SetHeld(false);
        Repaint(releasedAlpha);
    }

    private void SetHeld(bool isHeld)
    {
        if (buttons == null) return;

        foreach (string b in buttons) TouchInput.SetHeld(b, isHeld);
    }

    private void Repaint(float alpha)
    {
        bool locked = Locked;
        Color tint = locked ? lockedTint : Color.white;

        if (background != null) background.color = new Color(tint.r, tint.g, tint.b, alpha);
        if (icon != null) icon.color = new Color(tint.r, tint.g, tint.b, locked ? 0.5f : 0.95f);
    }
}
