using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class BestTime : MonoBehaviour
{
    public TMP_Text bestTimeText;
    private string key;

    void Start()
    {
        // Unique key per level
        key = "BestTime_" + SceneManager.GetActiveScene().name;

        float best = PlayerPrefs.GetFloat(key, 999999f);

        if (best < 999999f)
            bestTimeText.text = "Best: " + FormatTime(best);
        else
            bestTimeText.text = "Best: --:---:---";
    }

    public void TrySetBest(float newTime)
    {
        float best = PlayerPrefs.GetFloat(key, 999999f);

        if (newTime < best)
        {
            PlayerPrefs.SetFloat(key, newTime);
            bestTimeText.text = "Best: " + FormatTime(newTime);
        }
    }

    string FormatTime(float t)
    {
        int seconds = (int)t;
        int milliseconds = (int)((t - seconds) * 1000);
        int microseconds = (int)(((t - seconds) * 1000000) % 1000);

        return string.Format("{0:00}:{1:000}:{2:000}", seconds, milliseconds, microseconds);
    }
}
