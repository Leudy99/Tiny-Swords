using UnityEngine;

public class TrollBossCombat : MonoBehaviour
{
    public int damage = 25;
    public Transform attackPoint;
    public float weaponRange = 3f;
    public LayerMask playerLayer;

    public AudioSource audioSource;
    public AudioClip attackSound;

    public void PlayAttackSound()
    {
        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
    }

    public void Attack()
    {
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, playerLayer);

        if (hitPlayer.Length > 0)
        {
            PlayerHealth playerHealth = hitPlayer[0].GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.ChangeHealth(-damage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, weaponRange);
    }
}