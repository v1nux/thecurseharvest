using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;
    private PlayerStatsManager playerStats; // ← add this
    private bool canMove = true;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerStats = GetComponent<PlayerStatsManager>(); // ← add this
    }

    void FixedUpdate()
    {
        if (!canMove)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        // use speed from stats if available, otherwise use moveSpeed
        float speed = playerStats != null ? playerStats.GetSpeed() : moveSpeed;
        rb.linearVelocity = moveInput * speed;

        // drain stamina while walking ← add this
        if (moveInput != Vector2.zero && playerStats != null)
            playerStats.DrainStamina(playerStats.walkStaminaDrain * Time.fixedDeltaTime);
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (!canMove)
        {
            moveInput = Vector2.zero;
            animator.SetBool("isWalking", false);
            return;
        }

        moveInput = context.ReadValue<Vector2>();

        animator.SetBool("isWalking", moveInput != Vector2.zero);

        if (moveInput != Vector2.zero)
        {
            animator.SetFloat("InputX", moveInput.x);
            animator.SetFloat("InputY", moveInput.y);

            animator.SetFloat("LastInputX", moveInput.x);
            animator.SetFloat("LastInputY", moveInput.y);
        }
    }

   public void PlayAxeAnimation(Vector2 direction)
    {
        canMove = false;
        rb.linearVelocity = Vector2.zero;

        animator.SetBool("isWalking", false);
        animator.SetFloat("InputX", direction.x);
        animator.SetFloat("InputY", direction.y);
        animator.SetTrigger("UseAxe");

        Invoke(nameof(EndToolAnimation), 0.5f);
    }

    public void EndToolAnimation()
    {
        canMove = false;
    }
}