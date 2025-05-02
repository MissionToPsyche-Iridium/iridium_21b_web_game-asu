using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Loads the game scene
    public void PlayGame()
    {
        Time.timeScale = 1f;
        MusicManager.Instance.FadeOutAndLoad("CutScene");
    }

    // Loads the About scene
    public void LoadAboutScene()
    {
        SceneManager.LoadSceneAsync("About");
    }

    // Loads the Help scene
    public void LoadHelpScene()
    {
        SceneManager.LoadSceneAsync("Help");
    }

    // Loads the Main Menu (Index 0)
    public void BackToMainMenu()
    {
        SceneManager.LoadSceneAsync(0); // Loads the Main Menu scene
    }

    // Loads Special Thanks Menu
    public void LoadSpecialThanks() {
        SceneManager.LoadSceneAsync("SpecialThanks");
    }
}
