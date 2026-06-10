using UnityEngine;
using TMPro;

public class BestTime : MonoBehaviour
{
    public TMP_Text bestTimeText;

    void Start()
    {
        float best = PlayerPrefs.GetFloat("BestTime", 999999f);

        if (best < 999999f)
            bestTimeText.text = "Best: " + FormatTime(best);
        else
            bestTimeText.text = "Best: --:---:---";
    }

    public void TrySetBest(float newTime)
    {
        float best = PlayerPrefs.GetFloat("BestTime", 999999f);

        if (newTime < best)
        {
            PlayerPrefs.SetFloat("BestTime", newTime);
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
