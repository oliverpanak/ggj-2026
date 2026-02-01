using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("Timing")]
    [SerializeField] private float delayBeforePause = 0.3f;

    private bool isGameOver;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Multiple GameOverManagers in scene");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void TriggerGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        // Wait a moment before pausing so the player sees what happened
        yield return new WaitForSeconds(delayBeforePause);

        // Pause the game
        Time.timeScale = 0f;

        // Show game over UI
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void ReloadScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void OnDestroy()
    {
        // Ensure time scale is reset if this object is destroyed
        Time.timeScale = 1f;
    }
}
