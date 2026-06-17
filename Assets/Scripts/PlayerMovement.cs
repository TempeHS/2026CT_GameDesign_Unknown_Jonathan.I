using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 8f;
    public float jumpingPower = 16f;
    private float horizontal;
    private bool isFacingRight = true;

    [Header("Jumping")]
    private int jumpCount = 0;
    private int maxJumps = 2;

    [Header("Wall Slide")]
    public Transform wallCheck;
    public LayerMask wallLayer;
    public float wallSlideSpeed = 2f;
    private bool isWallSliding;

    [Header("Wall Jump")]
    public float wallJumpDuration = 0.2f;
    public Vector2 wallJumpForce = new Vector2(12f, 16f);
    private bool isWallJumping;

    [Header("Dash")]
    public float dashingPower = 24f;
    public float dashingTime = 0.2f;
    public float dashingCooldown = 1f;
    private bool canDash = true;
    private bool isDashing;

    [Header("References")]
    public Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public TrailRenderer tr;
    public ParticleSystem jumpParticles;

    [Header("Dash Sprites")]
    public SpriteRenderer playerSprite;
    public Sprite greenSprite; // dash ready
    public Sprite redSprite;   // dash cooldown

    private void Start()
    {
        playerSprite.sprite = greenSprite; // start green
    }

    private void Update()
    {
        if (isDashing)
            return;

        horizontal = Input.GetAxisRaw("Horizontal");

        HandleWallSlide();
        HandleWallJump();

        // Normal Jump
        if (Input.GetButtonDown("Jump") && !isWallSliding && jumpCount < maxJumps)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
            jumpParticles.Stop();
            jumpParticles.Play();
            jumpCount++;
        }

        // Reset jumps when grounded
        if (IsGrounded() && rb.linearVelocity.y <= 0.01f)
        {
            jumpCount = 0;
        }

        // Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

        Flip();
    }

    private void FixedUpdate()
    {
        if (isDashing)
            return;

        if (!isWallJumping)
        {
            rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.3f, wallLayer);
    }

    private void HandleWallSlide()
    {
        if (IsWalled() && !IsGrounded() && Mathf.Abs(horizontal) > 0.1f)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x,
                Mathf.Clamp(rb.linearVelocity.y, -wallSlideSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void HandleWallJump()
    {
        if (isWallSliding)
        {
            if (Input.GetButtonDown("Jump"))
            {
                isWallJumping = true;

                float direction = isFacingRight ? -1 : 1;

                rb.linearVelocity = new Vector2(direction * wallJumpForce.x, wallJumpForce.y);

                jumpParticles.Stop();
                jumpParticles.Play();

                Invoke(nameof(StopWallJump), wallJumpDuration);
            }
        }
    }

    private void StopWallJump()
    {
        isWallJumping = false;
    }

    private void Flip()
    {
        if (isWallJumping)
            return;

        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1f;
            transform.localScale = scale;
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        // turn RED when dash starts
        playerSprite.sprite = redSprite;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        // dash in facing direction
        rb.linearVelocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        tr.emitting = true;

        yield return new WaitForSeconds(dashingTime);

        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;

        // wait for cooldown
        yield return new WaitForSeconds(dashingCooldown);

        // turn GREEN when dash is ready again
        playerSprite.sprite = greenSprite;

        canDash = true;
    }

    // ⭐ FREEZE PLAYER AFTER FINISH
    public void FreezePlayer()
    {
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
        enabled = false; // disables movement script
    }
}
