using UnityEngine;

public class FinishBlock : MonoBehaviour
{
    public UIManager uiManager;
    public Timer timer;
    public BestTime bestTime;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            float finalTime = timer.StopTimer();
            bestTime.TrySetBest(finalTime);

            uiManager.ShowPopup("Level Complete!\nTime: " + finalTime.ToString("F2"));
        }
    }
}
