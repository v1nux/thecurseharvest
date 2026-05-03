using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public GameObject damagePopupPrefab;

    private int currentHealth;
    private HitFlash hitFlash;
    private Knockback knockback;

    void Awake()
    {
        currentHealth = maxHealth;
        hitFlash = GetComponent<HitFlash>();
        knockback = GetComponent<Knockback>();
    }

    public void TakeDamage(int damage, Vector2 hitDirection)
    {
        currentHealth -= damage;

        if (hitFlash != null)
            hitFlash.Flash();

        if (knockback != null)
            knockback.ApplyKnockback(hitDirection);

        if (damagePopupPrefab != null)
        {
            GameObject popup = Instantiate(
                damagePopupPrefab,
                transform.position + Vector3.up * 0.7f,
                Quaternion.identity
            );

            popup.GetComponent<DamagePopup>().Setup(damage);
        }

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}