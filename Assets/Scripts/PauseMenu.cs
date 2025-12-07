using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseUI;                 // Assign PauseMenu panel here
    public string firstSceneName = "GetStarted_Scene";  // Name of your first scene

    bool isPaused = false;

    void Update()
    {
        // Toggle pause with ESC or P
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseUI.SetActive(true);
        Time.timeScale = 0f;   // freeze gameplay
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseUI.SetActive(false);
        Time.timeScale = 1f;   // unfreeze
        isPaused = false;
    }

    public void RestartCurrentLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RestartAll()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(firstSceneName);
    }
    public void QuitGame()
    {
        Debug.Log("Game Quit");
        Application.Quit();

        // For editor testing only (won't be needed in final build)
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

}
