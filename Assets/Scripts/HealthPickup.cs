using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healAmount = 20;
    public AudioClip pickupSound;
    public float pickupVolume = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

        if (playerHealth == null)
            playerHealth = other.GetComponentInParent<PlayerHealth>();

        if (playerHealth != null)
        {
            if (playerHealth.currentHealth >= playerHealth.maxHealth)
                return;

            playerHealth.ChangeHealth(healAmount);

            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position, pickupVolume);
            }

            Destroy(gameObject);
        }
    }
}