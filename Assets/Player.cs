using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    private static readonly int IsMovingHash = Animator.StringToHash("isMoving");

    private float xInput;
    private Rigidbody2D rb;
    private Animator animator;
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float jumpForce = 8;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Jump();
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    private void HandleMovement()
    {
        rb.linearVelocity = new Vector2(xInput * moveSpeed, rb.linearVelocity.y);
    }

    private void HandleAnimation()
    {
        if (animator != null)
            animator.SetBool(IsMovingHash, Mathf.Abs(xInput) > 0.01f);
    }

    private void Update()
    {
        xInput = Input.GetAxisRaw("Horizontal");

        HandleMovement();
        HandleInput();
        HandleAnimation();
    }
}
