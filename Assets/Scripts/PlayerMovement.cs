using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public int facingDirection = 1;
    public Rigidbody2D rb;
    public Animator anim;
    public PlayerCombat playerCombat;

    private float moveHorizontal;
    private float moveVertical;

    public SpriteRenderer sr;

    [Header("Footsteps")]
    public AudioSource footstepSource;

    void Update()
    {
        moveHorizontal = 0f;
        moveVertical = 0f;

        if (Input.GetKey(KeyCode.A))
            moveHorizontal = -1f;
        else if (Input.GetKey(KeyCode.D))
            moveHorizontal = 1f;

        if (Input.GetKey(KeyCode.S))
            moveVertical = -1f;
        else if (Input.GetKey(KeyCode.W))
            moveVertical = 1f;

        if (Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.K))
        {
            playerCombat.Attack();
        }

        if (moveHorizontal > 0)
        {
            facingDirection = 1;
            sr.flipX = false;
        }
        else if (moveHorizontal < 0)
        {
            facingDirection = -1;
            sr.flipX = true;
        }

        anim.SetFloat("horizontal", Mathf.Abs(moveHorizontal));
        anim.SetFloat("vertical", Mathf.Abs(moveVertical));

        HandleFootsteps();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveHorizontal, moveVertical).normalized * speed;
    }

    void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(
            transform.localScale.x * -1,
            transform.localScale.y,
            transform.localScale.z
        );
    }

    void HandleFootsteps()
    {
        bool isMoving = moveHorizontal != 0 || moveVertical != 0;

        if (footstepSource == null) return;

        if (isMoving)
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
}
