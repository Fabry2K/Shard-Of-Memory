using UnityEngine;

// The one place gameplay reads input from, so the on-screen Android controls drive everything the
// keyboard already drives without each caller having to know they exist.
public static class GameInput
{
    // On a touchscreen the legacy input system reports every tap as mouse button 0, and Attack is
    // bound to mouse 0 - so tapping anywhere on the screen swung the sword. On a phone the
    // on-screen buttons are therefore the only accepted source; hardware input is ignored outright
    // rather than merged in.
    private static bool TouchOnly
    {
        get { return Application.isMobilePlatform; }
    }

    public static float GetAxisRaw(string axis)
    {
        if (!TouchOnly)
        {
            float hardware = Input.GetAxisRaw(axis);
            if (!Mathf.Approximately(hardware, 0f)) return hardware;
        }

        if (axis == "Horizontal") return TouchInput.Horizontal;
        if (axis == "Vertical") return TouchInput.Vertical;
        return 0f;
    }

    public static bool GetButton(string button)
    {
        if (TouchInput.GetButton(button)) return true;
        return !TouchOnly && Input.GetButton(button);
    }

    public static bool GetButtonDown(string button)
    {
        if (TouchInput.GetButtonDown(button)) return true;
        return !TouchOnly && Input.GetButtonDown(button);
    }

    public static bool GetButtonUp(string button)
    {
        if (TouchInput.GetButtonUp(button)) return true;
        return !TouchOnly && Input.GetButtonUp(button);
    }
}
