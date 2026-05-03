using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public int damage = 1;
    public float attackRange = 0.7f;
    public float attackRadius = 0.35f;
    public float attackCooldown = 0.4f;
    public LayerMask enemyLayer;

    private Animator animator;
    private Rigidbody2D rb;
    private PlayerMovement movement;

    private bool canAttack = true;
    private Vector2 lastDirection = Vector2.down;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        float x = animator.GetFloat("LastInputX");
        float y = animator.GetFloat("LastInputY");

        if (Mathf.Abs(x) > Mathf.Abs(y))
            lastDirection = x > 0 ? Vector2.right : Vector2.left;
        else if (Mathf.Abs(y) > 0)
            lastDirection = y > 0 ? Vector2.up : Vector2.down;
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (!canAttack) return;

        StartAttack();
    }

    void StartAttack()
    {
        canAttack = false;

        animator.SetBool("isWalking", false);
        animator.SetFloat("InputX", lastDirection.x);
        animator.SetFloat("InputY", lastDirection.y);
        animator.SetTrigger("Attack");

        Invoke(nameof(DoDamage), 0.15f);
        Invoke(nameof(EndAttack), attackCooldown);
    }

    void DoDamage()
    {
        Vector2 attackPos = (Vector2)transform.position + lastDirection * attackRange;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPos,
            attackRadius,
            enemyLayer
        );

        foreach (Collider2D hit in hits)
        {
            EnemyHealth enemy = hit.GetComponent<EnemyHealth>();

            if (enemy != null)
                enemy.TakeDamage(damage);
        }
    }

    void EndAttack()
    {
        canAttack = true;
    }

    void OnDrawGizmosSelected()
    {
        Vector2 direction = lastDirection == Vector2.zero ? Vector2.down : lastDirection;
        Vector2 attackPos = (Vector2)transform.position + direction * attackRange;

        Gizmos.DrawWireSphere(attackPos, attackRadius);
    }
}