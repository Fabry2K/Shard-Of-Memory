using UnityEngine;

// The one place gameplay reads input from, so the on-screen Android controls drive everything the
// keyboard already drives without each caller having to know they exist.
public static class GameInput
{
    public static float GetAxisRaw(string axis)
    {
        float hardware = Input.GetAxisRaw(axis);
        if (!Mathf.Approximately(hardware, 0f)) return hardware;

        if (axis == "Horizontal") return TouchInput.Horizontal;
        if (axis == "Vertical") return TouchInput.Vertical;
        return 0f;
    }

    public static bool GetButton(string button)
    {
        return Input.GetButton(button) || TouchInput.GetButton(button);
    }

    public static bool GetButtonDown(string button)
    {
        return Input.GetButtonDown(button) || TouchInput.GetButtonDown(button);
    }

    public static bool GetButtonUp(string button)
    {
        return Input.GetButtonUp(button) || TouchInput.GetButtonUp(button);
    }
}
