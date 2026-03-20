using System.Collections;
using UnityEngine;

public class TrollBossController : MonoBehaviour
{
    public Transform player;

    public float detectionRange = 6f;
    public float attackRange = 2.5f;
    public float speed = 2f;

    public float windupTime = 0.8f;
    public float recoveryTime = 1f;

    private Animator animator;
    private Rigidbody2D rb;

    private bool isAttacking = false;
    private bool isDead = false;

    private Vector3 originalScale;
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            else
            {
                Debug.LogWarning("No se encontró un objeto con Tag 'Player' para el TrollBoss.");
            }
        }
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange && !isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
        else if (distance <= detectionRange && !isAttacking)
        {
            ChasePlayer();
        }
        else
        {
            StopMoving();
        }
    }

    void ChasePlayer()
    {
        animator.SetBool("isChasing", true);

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;

        // Flip del sprite
        Vector3 scale = transform.localScale;
        float absX = Mathf.Abs(scale.x);

        if (direction.x < 0)
            transform.localScale = new Vector3(-absX, scale.y, scale.z);
        else if (direction.x > 0)
            transform.localScale = new Vector3(absX, scale.y, scale.z);
    }

    void StopMoving()
    {
        animator.SetBool("isChasing", false);
        rb.linearVelocity = Vector2.zero;
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;

        StopMoving();

        // WINDUP
        animator.SetBool("isWindingUp", true);
        yield return new WaitForSeconds(windupTime);

        animator.SetBool("isWindingUp", false);

        // ATTACK
        animator.SetBool("isAttacking", true);
        yield return new WaitForSeconds(0.3f);

        animator.SetBool("isAttacking", false);

        // RECOVERY
        animator.SetBool("isRecovering", true);
        yield return new WaitForSeconds(recoveryTime);

        animator.SetBool("isRecovering", false);

        isAttacking = false;
    }

    public void Die()
    {
        isDead = true;

        rb.linearVelocity = Vector2.zero;

        animator.SetBool("isDead", true);

        // opcional: destruir después de unos segundos
        Destroy(gameObject, 3f);
    }
}