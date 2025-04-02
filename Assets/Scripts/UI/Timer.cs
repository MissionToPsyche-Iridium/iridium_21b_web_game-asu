using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Slider timerSlider;
    public Text timerText;
    public Text waveText; // UI Element for wave number
    public bool stopTimer = false;
    public float waveDuration = 30f; // Wave length in seconds
    private float waveTimer; // This was missing before
    private int waveNumber = 1;

    void Start()
    {
        waveTimer = waveDuration; // Initialize wave timer
        timerSlider.maxValue = waveDuration;
        timerSlider.value = waveDuration;
        UpdateWaveUI();
    }

    void Update()
    {
        if (!stopTimer)
        {
            waveTimer -= Time.deltaTime;

            // Prevents negative timer display
            if (waveTimer < 0)
            {
                waveTimer = 0;
                //StartCoroutine(NextWave());
                RestartTimer();
            }

            int minutes = Mathf.FloorToInt(waveTimer / 60);
            int seconds = Mathf.FloorToInt(waveTimer % 60);
            timerText.text = $"{minutes:0}:{seconds:00}";
            timerSlider.value = waveTimer;
        }
    }

    IEnumerator NextWave()
{
    stopTimer = true; // Stops timer updates during wave transition
    waveText.text = "Wave Complete! Next wave in 10s..."; // Show break message in UI

    yield return new WaitForSeconds(10f); // Increased break time

    RestartTimer(); // Call function to start the next wave
}


    public void RestartTimer()
    {
        waveTimer = waveDuration; // Reset wave timer
        timerSlider.value = waveTimer;
        stopTimer = false;
        //UpdateWaveUI();
        try
        {
            FindObjectOfType<enemy_spawner>().ResetScaling();
        }
        catch (Exception e) {}
    }

    public int getWaveTime()
    {
        return Mathf.FloorToInt(waveTimer);
    }

    public void UpdateWaveUI()
    {
        waveText.text = "Wave: " + waveNumber; // Updates UI wave text
        waveNumber++;
    }
}
