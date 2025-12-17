using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float baseMoveSpeed = 5f;
    public float baseJumpForce = 7f;

    private float currentSpeed;
    private float currentJump;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // APPLY BUFFS FROM SHOP
        currentSpeed = baseMoveSpeed * GlobalData.SpeedMultiplier;
        currentJump = baseJumpForce * GlobalData.SpeedMultiplier; // Or use a separate Jump multiplier if you want
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * currentSpeed, rb.velocity.y);

        if (moveInput > 0)
            transform.localScale = new Vector3(1f, 1, 1);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-1f, 1, 1);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, currentJump);
        }
    }
}
