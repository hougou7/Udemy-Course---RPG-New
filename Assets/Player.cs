using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    private static readonly int IsGroundedHash = Animator.StringToHash("isGrounded");
    private static readonly int XVelocityHash = Animator.StringToHash("xvelocity");
    private static readonly int YVelocityHash = Animator.StringToHash("yvelocity");

    private float xInput;
    private Rigidbody2D rb;
    private Animator animator;

    [Header("Movement detail")]
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float jumpForce = 8;

    [Header("Collision detail")]

    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask whatIsGround;

    [SerializeField] private bool faceRight = false;

    [SerializeField]private bool isGrounded;
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
        if (!isGrounded)
            return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isGrounded = false;
    }

    private void HandleMovement()
    {
        rb.linearVelocity = new Vector2(xInput * moveSpeed, rb.linearVelocity.y);
    }

    private void HandleAnimation()
    {
        if (animator == null)
            return;

        animator.SetBool(IsGroundedHash, isGrounded);
        animator.SetFloat(YVelocityHash, rb.linearVelocity.y);
        animator.SetFloat(XVelocityHash, rb.linearVelocity.x);
    }
    private void HandleFlip()
    {
        if(rb.linearVelocity.x > 0 && faceRight == false)
            Flip();   
        else if(rb.linearVelocity.x < 0 && faceRight == true)
            Flip();
    }
    private void HandleCollision()
    {
        isGrounded = rb.linearVelocity.y <= 0.01f &&
            Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
    }
    private void Flip()
    {
        transform.Rotate(0,-180,0);
        faceRight = !faceRight;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position,transform.position + new Vector3(0,-groundCheckDistance));
    }

    private void Update()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        HandleCollision();
        HandleMovement();
        HandleInput();
        HandleAnimation();
        HandleFlip();
    }
}
