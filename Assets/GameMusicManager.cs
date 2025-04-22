using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMusicManager : MonoBehaviour
{
    public static GameMusicManager Instance;
    [Tooltip("The AudioSource playing your game music.")]
    public AudioSource audioSource;

    void Awake()
    {
        // singleton + persist
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // watch for menu return
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // actually kick it off
        audioSource.Play();
    }

    /// <summary>
    /// Fade out over the default fadeDuration (1s), then load MainMenu.
    /// </summary>
    public void FadeOutAndLoad(string sceneName)
    {
        // default fade duration
        FadeOutAndLoad(sceneName, 1f);
    }

    /// <summary>
    /// Fade out over the given duration, then load the given scene.
    /// </summary>
    public void FadeOutAndLoad(string sceneName, float fadeDuration)
    {
        StartCoroutine(FadeOutAndLoadCoroutine(sceneName, fadeDuration));
    }

    private IEnumerator FadeOutAndLoadCoroutine(string sceneName, float fadeDuration)
    {
        float startVol = audioSource.volume;
        float t = 0f;

        // unscaledDeltaTime so that UI transitions don't slow it
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(startVol, 0f, t / fadeDuration);
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // when we hit MainMenu, kill ourselves so menu‑music can take over
        if (scene.name == "Main Menu")
            Destroy(gameObject);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
