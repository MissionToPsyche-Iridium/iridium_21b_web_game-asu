using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Timer : MonoBehaviour
{
    public Slider timerSlider;
    public Text timerText;
    public Text waveText; // UI Element for displaying the wave number
    public float waveDuration = 30f; // Default duration per wave
    private float waveTimer;
    public bool stopTimer;
    private int waveNumber = 1;

    void Start()
    {
        stopTimer = false;
        waveTimer = waveDuration;
        timerSlider.maxValue = waveDuration;
        timerSlider.value = waveDuration;
        waveText.text = "Wave: " + waveNumber; // Initialize UI
    }

    void Update()
    {
        if (!stopTimer)
        {
            waveTimer -= Time.deltaTime;
            int minutes = Mathf.FloorToInt(waveTimer / 60);
            int seconds = Mathf.FloorToInt(waveTimer % 60);

            timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
            timerSlider.value = waveTimer;

            if (waveTimer <= 0)
            {
                StartCoroutine(NextWave());
            }
        }
    }

    IEnumerator NextWave()
    {
        stopTimer = true;
        yield return new WaitForSeconds(5f); // Short break between waves

        waveNumber++;
        waveText.text = "Wave: " + waveNumber; // Update wave UI

        // Reset the timer
        waveTimer = waveDuration;
        timerSlider.maxValue = waveDuration;
        timerSlider.value = waveDuration;

        stopTimer = false;
    }
}
