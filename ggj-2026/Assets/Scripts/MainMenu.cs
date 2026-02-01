using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "MainScene01";
    [SerializeField] private string manualTestingSceneName = "MainScene01_ManualTesting";

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void StartManualTesting()
    {
        SceneManager.LoadScene(manualTestingSceneName);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
