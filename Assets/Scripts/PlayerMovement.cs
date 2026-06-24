using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Base Movement")]
    public float baseSpeed = 8f;
    public float jumpingPower = 16f;
    private float horizontal;
    private bool isFacingRight = true;

    [Header("Momentum System")]
    public float groundMaxMomentum = 2.46f;
    public float airMaxMomentum = 2.51f;
    public float accelerationRate = .089f;   // slower exponential acceleration
    public float decayRate = 10f;            // exponential decay
    private float momentum = 1f;
    private float holdTime = 0f;
    private int lastMoveDir = 0;

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
    public float dashPower = 24f;
    public float dashTime = 0.2f;
    public float dashCooldown = 1f;
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
    public Sprite greenSprite;
    public Sprite redSprite;

    private void Start()
    {
        playerSprite.sprite = greenSprite;
    }

    private void Update()
    {
        if (isDashing)
            return;

        horizontal = Input.GetAxisRaw("Horizontal");

        HandleMomentum();
        HandleWallSlide();
        HandleWallJump();

        // DEBUG MOMENTUM + SPEED
        Debug.Log(
            "Momentum: " + momentum.ToString("F3") +
            " | SpeedX: " + rb.linearVelocity.x.ToString("F3") +
            " | SpeedMag: " + rb.linearVelocity.magnitude.ToString("F3")
        );

        // Normal Jump
        if (Input.GetButtonDown("Jump") && !isWallSliding && jumpCount < maxJumps)
        {
            // ⭐ Lose 10% momentum on jump
            momentum *= 0.9f;

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
            float speed = baseSpeed;

            // Air movement is 4% faster
            if (!IsGrounded())
                speed *= 1.04f;

            rb.linearVelocity = new Vector2(horizontal * speed * momentum, rb.linearVelocity.y);
        }
    }

    // -------------------------
    // MOMENTUM SYSTEM
    // -------------------------
    private void HandleMomentum()
    {
        int moveDir = horizontal > 0 ? 1 : horizontal < 0 ? -1 : 0;

        float maxMomentum = IsGrounded() ? groundMaxMomentum : airMaxMomentum;

        // No input → decay momentum
        if (moveDir == 0)
        {
            momentum *= Mathf.Exp(-decayRate * Time.deltaTime);
            holdTime = 0;
            lastMoveDir = 0;
            return;
        }

        // Changing direction → strong decay
        if (lastMoveDir != 0 && moveDir != lastMoveDir)
        {
            momentum *= Mathf.Exp(-decayRate * Time.deltaTime);
            holdTime = 0;
        }
        else
        {
            // Holding same direction → exponential acceleration
            holdTime += Time.deltaTime;
            float target = maxMomentum * (1f - Mathf.Exp(-accelerationRate * holdTime));
            momentum = Mathf.Lerp(momentum, target, 0.5f);
        }

        lastMoveDir = moveDir;

        // Clamp
        momentum = Mathf.Clamp(momentum, 1f, maxMomentum);
    }

    // -------------------------
    // WALL SLIDE
    // -------------------------
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

    // -------------------------
    // WALL JUMP
    // -------------------------
    private void HandleWallJump()
    {
        if (isWallSliding && Input.GetButtonDown("Jump"))
        {
            isWallJumping = true;

            // ⭐ Lose 10% momentum on wall jump too
            momentum *= 0.9f;

            float direction = isFacingRight ? -1 : 1;

            rb.linearVelocity = new Vector2(direction * wallJumpForce.x, wallJumpForce.y);

            jumpParticles.Stop();
            jumpParticles.Play();

            Invoke(nameof(StopWallJump), wallJumpDuration);
        }
    }

    private void StopWallJump()
    {
        isWallJumping = false;
    }

    // -------------------------
    // DASH
    // -------------------------
    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        playerSprite.sprite = redSprite;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector2 dashDir = new Vector2(x, y);

        if (dashDir == Vector2.zero)
            dashDir = new Vector2(isFacingRight ? 1 : -1, 0);

        dashDir.Normalize();

        rb.linearVelocity = dashDir * dashPower;
        tr.emitting = true;

        yield return new WaitForSeconds(dashTime);

        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;

        ApplyDashMomentum(dashDir);

        yield return new WaitForSeconds(dashCooldown);

        playerSprite.sprite = greenSprite;
        canDash = true;
    }

    private void ApplyDashMomentum(Vector2 dashDir)
    {
        bool sameDir = (isFacingRight && dashDir.x > 0) || (!isFacingRight && dashDir.x < 0);
        bool slightAngle = sameDir && Mathf.Abs(dashDir.y) > 0;

        if (sameDir)
        {
            if (slightAngle)
                momentum *= 0.9f; // lose 10%
            else
                momentum *= 1.1f; // gain 10%
        }
        else
        {
            // Wrong direction → exponential decay to 0
            momentum *= Mathf.Exp(-12f * Time.deltaTime);
        }

        float maxMomentum = IsGrounded() ? groundMaxMomentum : airMaxMomentum;
        momentum = Mathf.Clamp(momentum, 1f, maxMomentum);
    }

    // -------------------------
    // HELPERS
    // -------------------------
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.3f, wallLayer);
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

    // -------------------------
    // FREEZE PLAYER
    // -------------------------
    public void FreezePlayer()
    {
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
        enabled = false;
    }
}
