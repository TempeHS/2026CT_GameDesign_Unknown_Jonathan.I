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
            // Stop timer and get final time
            float finalTime = timer.StopTimer();

            // Save best time for THIS level
            bestTime.TrySetBest(finalTime);

            // Freeze player movement
            other.GetComponent<PlayerMovement>().FreezePlayer();

            // Show popup
            uiManager.ShowPopup("Level Complete!\nTime: " + finalTime.ToString("F2"));
        }
    }
}
