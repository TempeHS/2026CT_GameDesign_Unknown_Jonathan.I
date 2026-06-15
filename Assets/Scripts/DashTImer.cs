using UnityEngine;

public class DashTimer : MonoBehaviour
{
    public float dashCooldown = 1f;

    private float dashTimer = 0f;
    private bool canDash = true;

    public SpriteRenderer playerSprite;

    public Color dashReadyColor = Color.green;   // dash available
    public Color dashCooldownColor = Color.blue; // dash unavailable

    void Start()
    {
        playerSprite.color = dashReadyColor; // start green
    }

    void Update()
    {
        if (!canDash)
        {
            dashTimer += Time.deltaTime;

            if (dashTimer >= dashCooldown)
            {
                dashTimer = 0f;
                canDash = true;

                playerSprite.color = dashReadyColor; // turn green
            }
        }
    }

    public bool TryDash()
    {
        if (canDash)
        {
            canDash = false;
            dashTimer = 0f;

            playerSprite.color = dashCooldownColor; // turn blue

            return true;
        }

        return false;
    }
}
