using UnityEngine;

public class FinishBlock : MonoBehaviour
{
    public UIManager uiManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            uiManager.ShowPopup("Level Complete!");
        }
    }
}
