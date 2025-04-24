using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneNavigator
{
    /// <summary>
    /// Fades out the game music (if any) then loads the MainMenu scene.
    /// </summary>
    public static void GoToMainMenu(float fadeDuration = 1f)
    {
        // If the GameMusicManager exists, ask it to fade us out *then* load.
        if (GameMusicManager.Instance != null)
        {
            GameMusicManager.Instance.FadeOutAndLoad("Main Menu", fadeDuration);
        }
        else
        {
            // no game music running? just jump straight there.
            SceneManager.LoadScene("Main Menu");
        }
    }
}


