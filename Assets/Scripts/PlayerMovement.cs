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
    public float accelerationRate = 5.12f;
    public float decayRate = 10f;
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

    private Coroutine dashRoutine;   // ⭐ ADDED

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

        if (Input.GetKeyDown(KeyCode.Return))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );
        }

        horizontal = Input.GetAxisRaw("Horizontal");

        HandleMomentum();
        HandleWallSlide();
        HandleWallJump();

        if (Input.GetButtonDown("Jump") && !isWallSliding && jumpCount < maxJumps)
        {
            momentum *= 0.9f;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
            jumpParticles.Stop();
            jumpParticles.Play();
            jumpCount++;
        }

        if (IsGrounded() && rb.linearVelocity.y <= 0.01f)
        {
            jumpCount = 0;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            dashRoutine = StartCoroutine(Dash());   // ⭐ CHANGED
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

            if (!IsGrounded())
                speed *= 1.04f;

            rb.linearVelocity = new Vector2(horizontal * speed * momentum, rb.linearVelocity.y);
        }
    }

    private void HandleMomentum()
    {
        int moveDir = horizontal > 0 ? 1 : horizontal < 0 ? -1 : 0;

        float maxMomentum = IsGrounded() ? groundMaxMomentum : airMaxMomentum;

        if (moveDir == 0)
        {
            momentum *= Mathf.Exp(-decayRate * Time.deltaTime);
            holdTime = 0;
            lastMoveDir = 0;
            return;
        }

        if (lastMoveDir != 0 && moveDir != lastMoveDir)
        {
            momentum *= Mathf.Exp(-decayRate * Time.deltaTime);
            holdTime = 0;
        }
        else
        {
            holdTime += Time.deltaTime;
            float target = maxMomentum * (1f - Mathf.Exp(-accelerationRate * holdTime));
            momentum = Mathf.Lerp(momentum, target, 0.5f);
        }

        lastMoveDir = moveDir;
        momentum = Mathf.Clamp(momentum, 1f, maxMomentum);
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
        if (isWallSliding && Input.GetButtonDown("Jump"))
        {
            isWallJumping = true;

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

        float dashStrength = dashPower;

        if (dashDir.x != 0 && dashDir.y != 0)
            dashStrength *= 0.75f;
        else if (dashDir.y > 0)
            dashStrength *= 0.60f;

        tr.emitting = true;

        float t = 0f;

        while (t < dashTime)
        {
            t += Time.deltaTime;

            float ease = Mathf.Lerp(1.35f, 0.65f, t / dashTime);

            rb.linearVelocity = dashDir * dashStrength * ease;

            if (Input.GetButtonDown("Jump"))
            {
                isDashing = false;
                rb.gravityScale = originalGravity;

                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
                jumpParticles.Stop();
                jumpParticles.Play();
                break;
            }

            yield return null;
        }

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
                momentum *= 0.9f;
            else
                momentum *= 1.1f;
        }
        else
        {
            momentum *= Mathf.Exp(-12f * Time.deltaTime);
        }

        float maxMomentum = IsGrounded() ? groundMaxMomentum : airMaxMomentum;
        momentum = Mathf.Clamp(momentum, 1f, maxMomentum);
    }

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

    // ⭐ FINAL FREEZE — stops EVERYTHING instantly
    public void FreezePlayer()
    {
        if (dashRoutine != null)
        {
            StopCoroutine(dashRoutine);
            dashRoutine = null;
        }

        isDashing = false;
        isWallJumping = false;

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;

        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        enabled = false;

        if (tr != null)
            tr.emitting = false;
    }
}
