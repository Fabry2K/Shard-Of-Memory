using UnityEngine;

public class GameManager : MonoBehaviour
{
    public string transitionedFromScene;

    public Vector2 platformingRespawnPoint;
    public Vector2 respawnPoint;
    [SerializeField] Bench bench;

    [SerializeField] private FadeUI pauseMenu;
    [SerializeField] private float fadeTime;
    public bool gameIsPaused;
    public bool hiveKnightDialogueDone;
    public bool hasKey;
    public bool forestGateOpened;

    public static GameManager Instance { get; private set; }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && !gameIsPaused)
        {
            pauseMenu.FadeUIIn(fadeTime);
            Time.timeScale = 0;
            gameIsPaused = true;
        }
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1;
        gameIsPaused = false;
    }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        } else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
        bench = FindObjectOfType<Bench>();
    }

    public void RespawnPlayer()
    {
        if (bench != null) {
            if (bench.interacted)
            {
                respawnPoint = bench.transform.position;
            }
            else
            {
                respawnPoint = platformingRespawnPoint;
            }
        }
        else
        {
            respawnPoint = platformingRespawnPoint;
        }

        PlayerController.Instance.transform.position = respawnPoint;

        StartCoroutine(UIManager.Instance.DeactivateDeathScreen());
        PlayerController.Instance.Respawned();
    }
}
