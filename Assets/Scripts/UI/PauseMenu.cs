using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;

    private void Start()
    {
        pauseMenu.SetActive(false); // Ensure the pause menu is hidden at the start
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }
    public void RestartGame()
    {
        Time.timeScale = 1f; // Ensure time resumes
        EnemyHealth.numMetals = 1;
        EnemyHealth.healthScale = 1.0f;
        basic_enemy_behavior.speedFactor = 1.0f; //no speed scaling
        dash_enemy_script.speedFactor = 1.0f;
        shoot_enemy_behavior.speedFactor = 1.0f;
        head_behavior.speedFactor = 1.0f;
        body_follow.speedFactor = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload current scene
    }

    

    public void BackToMainMenu()
    {
        SceneNavigator.GoToMainMenu(2f); // Loads the Main Menu scene
    }
}
