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

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // when we hit MainMenu, kill ourselves so menu‑music can take over
        if (scene.name == "Main Menu")
            Destroy(gameObject);
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
