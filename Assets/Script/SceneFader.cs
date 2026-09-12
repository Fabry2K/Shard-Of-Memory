using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFader : MonoBehaviour
{

    [SerializeField] public float fadeTime;
    [Tooltip("How long the screen stays fully black once the next scene is ready, so its first frames are never seen.")]
    [SerializeField] private float holdBlackTime = 0.15f;

    private Image fadeOutUIImage;
    private Coroutine activeFade;

    public enum FadeDirection
    {
        In,
        Out
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        fadeOutUIImage = GetComponent<Image>();
    }

    public void CallFadeAndLoadScene(string _sceneToLoad)
    {
        StartCoroutine(FadeAndLoadScene(FadeDirection.In, _sceneToLoad));
    }

    // Entry point for the arriving scene. Guarded so several exits in the same room can't each
    // kick off their own fade and fight over the alpha.
    public void FadeOut()
    {
        if (activeFade != null) StopCoroutine(activeFade);
        activeFade = StartCoroutine(Fade(FadeDirection.Out));
    }

    public IEnumerator Fade(FadeDirection _fadeDirection)
    {
        bool fadingIn = _fadeDirection == FadeDirection.In;
        float from = fadingIn ? 0f : 1f;
        float to = fadingIn ? 1f : 0f;

        fadeOutUIImage.enabled = true;

        float t = 0f;
        while (t < fadeTime)
        {
            // Unscaled, so a paused game or a hitching frame still fades evenly.
            t += Time.unscaledDeltaTime;
            SetAlpha(Mathf.Lerp(from, to, Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / fadeTime))));
            yield return null;
        }

        SetAlpha(to);

        // Stop drawing only once the veil is actually transparent. Disabling it any earlier is
        // what made the new scene pop in instead of fading up.
        if (!fadingIn) fadeOutUIImage.enabled = false;
    }

    public IEnumerator FadeAndLoadScene(FadeDirection _fadeDirection, string _sceneToLoad)
    {
        fadeOutUIImage.enabled = true;

        yield return Fade(_fadeDirection);

        // SceneManager.LoadScene stalls the main thread until the whole scene is ready, which is
        // the hitch between rooms. Loading asynchronously keeps the frame loop running, and
        // holding activation back means the swap happens while the screen is fully black.
        AsyncOperation load = SceneManager.LoadSceneAsync(_sceneToLoad);
        load.allowSceneActivation = false;

        // progress tops out at 0.9 while activation is withheld
        while (load.progress < 0.9f) yield return null;

        yield return new WaitForSecondsRealtime(holdBlackTime);

        load.allowSceneActivation = true;
    }

    private void SetAlpha(float _alpha)
    {
        Color c = fadeOutUIImage.color;
        c.a = _alpha;
        fadeOutUIImage.color = c;
    }
}
