using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMusicTrigger : MonoBehaviour
{
    [Tooltip("Drag in your GameMusicManager prefab here")]
    public GameObject gameMusicManagerPrefab;

    // this method name shows up in the Signal Receiver dropdown
    public void StartGameMusic()
    {
        // first, kill any leftover menu music:
        if (MusicManager.Instance != null)
            Destroy(MusicManager.Instance.gameObject);

        // then spawn & play our game‑music manager
        Instantiate(gameMusicManagerPrefab);
    }
}
