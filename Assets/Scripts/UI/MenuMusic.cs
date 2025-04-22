using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class MusicManager : MonoBehaviour {
    public static MusicManager Instance;

    [Tooltip("The AudioSource playing your menu music.")]
    public AudioSource audioSource;

    [Tooltip("Seconds it takes to fade out.")]
    public float fadeDuration = 1.0f;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
            return;
        }
    }

    void Start() {
        audioSource.loop = true;
        audioSource.Play();
    }

    /// <summary>
    /// Fade menu music out over fadeDuration, then load sceneName.
    /// </summary>
    public void FadeOutAndLoad(string sceneName) {
        StartCoroutine(FadeOutAndLoadCoroutine(sceneName));
    }

    private IEnumerator FadeOutAndLoadCoroutine(string sceneName) {
        float startVol = audioSource.volume;
        float t = 0f;
        while (t < fadeDuration) {
            t += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(startVol, 0f, t / fadeDuration);
            yield return null;
        }
        SceneManager.LoadScene(sceneName);
    }
}
