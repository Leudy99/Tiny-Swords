using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElfMovement : MonoBehaviour
{
     private Rigidbody2D rb;
    private Transform player;
    private Animator anim;
    private SpriteRenderer sr;

    [Header("Movement")]
    public float speed = 2f;

    [Header("Facing")]
    [Tooltip("1 = mirando a la derecha, -1 = mirando a la izquierda al iniciar")]
    public int facingDirection = -1;

    private EnemyState enemyState;

    [Header("Combat")]
    public float attackRange = 1.2f;
    public float attackCooldown = 1f;
    private float attackCooldownTimer;

    [Header("Detection")]
    public float playerDetectionRange = 4f;
    public Transform detectionPoint;
    public LayerMask playerLayer;

    [Header("Footsteps")]
    public AudioSource footstepSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        ApplyFacingVisual();
        ChangeState(EnemyState.Idle);
    }

    void Update()
    {
        CheckForPlayer();

        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
        }

        if (enemyState == EnemyState.Chasing)
        {
            Chase();
        }
        else if (enemyState == EnemyState.Attacking)
        {
            rb.linearVelocity = Vector2.zero;
        }
        else if (enemyState == EnemyState.Idle)
        {
            rb.linearVelocity = Vector2.zero;
        }

        HandleFootsteps();
    }

    void Chase()
    {
        if (player == null) return;

        if ((player.position.x > transform.position.x && facingDirection == -1) ||
            (player.position.x < transform.position.x && facingDirection == 1))
        {
            Flip();
        }

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }

    void Flip()
    {
        facingDirection *= -1;
        ApplyFacingVisual();
    }

    void ApplyFacingVisual()
    {
        if (sr == null) return;
        sr.flipX = (facingDirection == 1);
    }

    private void CheckForPlayer()
    {
        Collider2D hitPlayer = Physics2D.OverlapCircle(
            detectionPoint.position,
            playerDetectionRange,
            playerLayer
        );

        if (hitPlayer != null)
        {
            player = hitPlayer.transform;

            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackRange && attackCooldownTimer <= 0)
            {
                attackCooldownTimer = attackCooldown;
                ChangeState(EnemyState.Attacking);
            }
            else if (distanceToPlayer > attackRange)
            {
                ChangeState(EnemyState.Chasing);
            }
        }
        else
        {
            player = null;
            rb.linearVelocity = Vector2.zero;
            ChangeState(EnemyState.Idle);
        }
    }

    void ChangeState(EnemyState newState)
    {
        if (enemyState == EnemyState.Idle)
        {
            anim.SetBool("isIdle", false);
        }
        else if (enemyState == EnemyState.Chasing)
        {
            anim.SetBool("isChasing", false);
        }
        else if (enemyState == EnemyState.Attacking)
        {
            anim.SetBool("isAttacking", false);
        }

        enemyState = newState;

        if (enemyState == EnemyState.Idle)
        {
            anim.SetBool("isIdle", true);
        }
        else if (enemyState == EnemyState.Chasing)
        {
            anim.SetBool("isChasing", true);
        }
        else if (enemyState == EnemyState.Attacking)
        {
            anim.SetBool("isAttacking", true);
        }
    }

    void HandleFootsteps()
    {
        if (footstepSource == null) return;

        bool shouldPlay = enemyState == EnemyState.Chasing && rb.linearVelocity.magnitude > 0.05f;

        if (shouldPlay)
        {
            if (!footstepSource.isPlaying)
            {
                footstepSource.Play();
            }
        }
        else
        {
            if (footstepSource.isPlaying)
            {
                footstepSource.Stop();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (detectionPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(detectionPoint.position, playerDetectionRange);
        }
    }
}

public enum EnemyState
{
    Idle,
    Chasing,
    Attacking
}