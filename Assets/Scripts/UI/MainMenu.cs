using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Loads the game scene
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("SampleScene");
    }

    // Loads the About scene
    public void LoadAboutScene()
    {
        SceneManager.LoadSceneAsync("About");
    }

    // Loads the Main Menu (Index 0)
    public void BackToMainMenu()
    {
        SceneManager.LoadSceneAsync(0); // Loads the Main Menu scene
    }
}
