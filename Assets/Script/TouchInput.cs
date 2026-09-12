using System.Collections.Generic;
using UnityEngine;

// Virtual input state written by the on-screen buttons.
//
// Press/release edges are not recorded when the UI event arrives, they are recomputed once per
// frame by TouchControls before any gameplay script runs. That way a button reads the same for
// every caller within a frame - Jump alone is read from three different places in PlayerController
// - exactly like UnityEngine.Input behaves, and a tap can never be missed or counted twice
// because the UI event happened to land after the player had already updated.
public static class TouchInput
{
    public const string Left = "Left";
    public const string Right = "Right";
    public const string Up = "Up";
    public const string Down = "Down";

    private static readonly HashSet<string> held = new HashSet<string>();
    private static readonly HashSet<string> heldLastFrame = new HashSet<string>();
    private static readonly HashSet<string> pressedThisFrame = new HashSet<string>();
    private static readonly HashSet<string> releasedThisFrame = new HashSet<string>();

    public static float Horizontal { get; private set; }
    public static float Vertical { get; private set; }

    public static void SetHeld(string button, bool isHeld)
    {
        if (string.IsNullOrEmpty(button)) return;

        if (isHeld) held.Add(button);
        else held.Remove(button);
    }

    public static void Clear()
    {
        held.Clear();
        heldLastFrame.Clear();
        pressedThisFrame.Clear();
        releasedThisFrame.Clear();
        Horizontal = 0f;
        Vertical = 0f;
    }

    public static void RefreshEdges()
    {
        pressedThisFrame.Clear();
        releasedThisFrame.Clear();

        foreach (string button in held)
            if (!heldLastFrame.Contains(button)) pressedThisFrame.Add(button);

        foreach (string button in heldLastFrame)
            if (!held.Contains(button)) releasedThisFrame.Add(button);

        heldLastFrame.Clear();
        foreach (string button in held) heldLastFrame.Add(button);

        Horizontal = (held.Contains(Right) ? 1f : 0f) - (held.Contains(Left) ? 1f : 0f);
        Vertical = (held.Contains(Up) ? 1f : 0f) - (held.Contains(Down) ? 1f : 0f);
    }

    public static bool GetButton(string button) => held.Contains(button);
    public static bool GetButtonDown(string button) => pressedThisFrame.Contains(button);
    public static bool GetButtonUp(string button) => releasedThisFrame.Contains(button);
}
