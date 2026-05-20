using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Call your death system here
            RunManager.Instance.OnPlayerDeath();
        }
    }
}
