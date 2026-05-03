using UnityEngine;
using System.Collections;

public class Knockback : MonoBehaviour
{
    public float knockbackForce = 5f;
    public float knockbackTime = 0.15f;

    private Rigidbody2D rb;
    public bool IsKnockbacking { get; private set; }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ApplyKnockback(Vector2 direction)
    {
        StopAllCoroutines();
        StartCoroutine(KnockbackRoutine(direction));
    }

    IEnumerator KnockbackRoutine(Vector2 direction)
    {
        IsKnockbacking = true;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction.normalized * knockbackForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackTime);

        rb.linearVelocity = Vector2.zero;
        IsKnockbacking = false;
    }
}