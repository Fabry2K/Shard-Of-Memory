using UnityEngine;

// Owns the on-screen pad: shows it only where it is wanted, and refreshes the virtual input's
// press/release edges once per frame. The early execution order is what guarantees those edges
// are already settled by the time PlayerController reads them.
[DefaultExecutionOrder(-200)]
public class TouchControls : MonoBehaviour
{
    [Tooltip("The pad itself. Kept as a child so this component can keep running while it is hidden.")]
    [SerializeField] private GameObject pad;

    [Tooltip("Show the pad while playing in the editor, to test it without an Android device. Ignored in a build, so it can't leak into a desktop release.")]
    [SerializeField] private bool showInEditor = true;

    private bool enabledForThisPlatform;

    private void Start()
    {
        enabledForThisPlatform = Application.isMobilePlatform || (showInEditor && Application.isEditor);

        if (pad != null) pad.SetActive(enabledForThisPlatform);
        if (!enabledForThisPlatform) TouchInput.Clear();
    }

    private void Update()
    {
        if (!enabledForThisPlatform) return;

        // While paused the menu owns the screen, and a button left held would keep feeding input.
        bool paused = GameManager.Instance != null && GameManager.Instance.gameIsPaused;
        if (pad != null && pad.activeSelf == paused)
        {
            pad.SetActive(!paused);
        }

        if (paused)
        {
            TouchInput.Clear();
            return;
        }

        TouchInput.RefreshEdges();
    }
}
