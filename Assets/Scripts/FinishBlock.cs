using UnityEngine;
using UnityEngine.SceneManagement;

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

            other.GetComponent<PlayerMovement>().FreezePlayer();

            string sceneName = SceneManager.GetActiveScene().name;

            bool allowNext = sceneName != "TestLevel";

            uiManager.ShowFinishOptions(finalTime, allowNext);
        }
    }
}
