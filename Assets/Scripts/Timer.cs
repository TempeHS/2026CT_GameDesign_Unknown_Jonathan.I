using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public TMP_Text timerText;

    private float time;
    private bool timerRunning = false;
    private bool hasStarted = false;

    void Update()
    {
        // Start timer on first input
        if (!hasStarted && Input.anyKeyDown)
        {
            hasStarted = true;
            timerRunning = true;
        }

        // Update timer
        if (timerRunning)
        {
            time += Time.deltaTime;
            timerText.text = "Time: " + FormatTime(time);
        }
    }

    public float StopTimer()
    {
        timerRunning = false;
        return time;
    }

    string FormatTime(float t)
    {
        int seconds = (int)t;
        int milliseconds = (int)((t - seconds) * 1000);
        int microseconds = (int)(((t - seconds) * 1000000) % 1000);

        return string.Format("{0:00}:{1:000}:{2:000}", seconds, milliseconds, microseconds);
    }
}
