using UnityEngine;

public class HideWhenBehind : MonoBehaviour
{
    private SpriteRenderer sprite;
    private Color originalColor;

    [SerializeField] private float fadeAlpha = 0.4f;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        originalColor = sprite.color;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SetAlpha(fadeAlpha);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SetAlpha(1f);
        }
    }

    void SetAlpha(float alpha)
    {
        Color c = sprite.color;
        c.a = alpha;
        sprite.color = c;
    }
}