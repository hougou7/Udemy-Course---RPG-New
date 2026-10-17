using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    private static readonly int IsGroundedHash = Animator.StringToHash("isGrounded");
    private static readonly int XVelocityHash = Animator.StringToHash("xvelocity");
    private static readonly int YVelocityHash = Animator.StringToHash("yvelocity");
    private static readonly int AttackHash = Animator.StringToHash("attack");

    private float xInput;
    private Rigidbody2D rb;
    private Animator animator;

    public Collider2D[] enemyColliders;
    [Header("Attack details")]
    [Min(0.01f)]
    [SerializeField] private float attackRadius = 1f;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask whatIsEnemy;

    [Header("Movement detail")]
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float jumpForce = 8;

    [Header("Collision detail")]

    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask whatIsGround;

    [SerializeField] private bool faceRight = false;

    [SerializeField]private bool isGrounded;
    private bool canmove = true;
    private bool attackRequested;
    private bool attackDamageApplied = true;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
            Attack();

        if (Input.GetKeyDown(KeyCode.Space))
            Jump();
    }

    private void Attack()
    {
        if (!canmove || !isGrounded || animator == null)
            return;

        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack") ||
            (animator.IsInTransition(0) && animator.GetNextAnimatorStateInfo(0).IsTag("Attack")))
            return;

        attackRequested = true;
        attackDamageApplied = false;
        enemyColliders = new Collider2D[0];
        canmove = false;
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        animator.SetTrigger(AttackHash);
    }

    private void UpdateMovementPermission()
    {
        bool isAttacking = animator != null &&
            (animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack") ||
            (animator.IsInTransition(0) && animator.GetNextAnimatorStateInfo(0).IsTag("Attack")));

        if (isAttacking)
            attackRequested = false;

        canmove = !attackRequested && !isAttacking;
    }

    private void Jump()
    {
        if (!canmove || !isGrounded)
            return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isGrounded = false;
    }

    private void HandleMovement()
    {
        float horizontalVelocity = canmove ? xInput * moveSpeed : 0;
        rb.linearVelocity = new Vector2(horizontalVelocity, rb.linearVelocity.y);
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
        if (!canmove)
            return;

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
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }

    public void DamageEnemies()
    {
        if (attackDamageApplied || attackPoint == null || attackRadius <= 0)
            return;

        attackDamageApplied = true;
        enemyColliders = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, whatIsEnemy);
        var damagedEnemies = new HashSet<Enemy>();

        foreach (Collider2D enemyCollider in enemyColliders)
        {
            Enemy enemy = enemyCollider.GetComponentInParent<Enemy>();
            if (enemy != null && damagedEnemies.Add(enemy))
                enemy.TakeDamage();
        }
    }

    private void Update()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        HandleCollision();
        UpdateMovementPermission();
        HandleInput();
        HandleMovement();
        HandleAnimation();
        HandleFlip();
    }
}
