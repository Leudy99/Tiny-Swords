using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElfCombat : MonoBehaviour
{
    public int damage = 1;
    public Transform attackPoint;
    public float weaponRange;
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

        if(hitPlayer.Length > 0)
        {
            hitPlayer[0].GetComponent<PlayerHealth>().ChangeHealth(-damage);
        }
    }
}