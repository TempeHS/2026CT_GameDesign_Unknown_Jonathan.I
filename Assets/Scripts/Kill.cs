using UnityEngine;

public class KillBlock : MonoBehaviour
{
    public UIManager uiManager;
    public DeathExplosion deathExplosion;   // reference to world GIF

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement pm = other.GetComponent<PlayerMovement>();
            pm.FreezePlayer();

            // ⭐ Play world explosion GIF
            deathExplosion.PlayExplosion(other.transform.position);

            // ⭐ Show death popup
            uiManager.ShowDeathPopup();
        }
    }
}
