using System.Collections;
using UnityEngine;

public class DamageKnockback : MonoBehaviour
{
    public Rigidbody2D rb;
    public float knockbackForce = 8f;
    public float knockbackDuration = 0.15f;

    private Coroutine knockbackCoroutine;
    public bool IsKnockedBack { get; private set; }

    void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    public void ApplyKnockback(Vector2 knockbackDirection)
    {
        if (rb == null) return;

        if (knockbackCoroutine != null)
            StopCoroutine(knockbackCoroutine);

        if (knockbackDirection == Vector2.zero)
            knockbackDirection = Vector2.right;

        knockbackDirection.Normalize();

        Debug.Log(name + " knockback direction: " + knockbackDirection);

        knockbackCoroutine = StartCoroutine(KnockbackRoutine(knockbackDirection));
    }

    private IEnumerator KnockbackRoutine(Vector2 direction)
    {
        IsKnockedBack = true;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackDuration);

        rb.linearVelocity = Vector2.zero;
        IsKnockedBack = false;
        knockbackCoroutine = null;
    }
}