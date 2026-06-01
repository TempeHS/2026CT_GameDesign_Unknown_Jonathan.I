using UnityEngine;

public class KillBlock : MonoBehaviour
{
    public UIManager uiManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(other.gameObject);
            uiManager.ShowPopup("You Died!");
        }
    }
}
