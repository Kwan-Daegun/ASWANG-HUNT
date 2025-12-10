using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolyWater : MonoBehaviour
{
    [SerializeField] private float blastRadius = 3f;
    [SerializeField] private GameObject splashEffect; // Optional particle

    // We no longer need the Start() timer because PlayerThrow handles the ignore logic!

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Explode on impact with anything solid (Ground, Walls, Enemy)
        Explode();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // If we hit a trigger (like the Santelmo), explode.
        // We explicitly ignore the Player tag so we don't blow up in our own face
        if (!other.CompareTag("Player") && !other.isTrigger)
        {
            Explode();
        }
        // Special case: If we hit the Santelmo directly
        else if (other.CompareTag("Enemy") || other.GetComponent<SantelmoScript>() != null)
        {
            Explode();
        }
    }

    void Explode()
    {
        // 1. Find everything in the circle
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, blastRadius);

        foreach (Collider2D obj in hitObjects)
        {
            // Check for Santelmo to kill
            SantelmoScript santelmo = obj.GetComponent<SantelmoScript>();
            if (santelmo != null)
            {
                santelmo.ExtinguishSelf();
            }

            // Check for HP scripts to Cure (Player/House)
            HP hpScript = obj.GetComponent<HP>();
            if (hpScript != null)
            {
                hpScript.ExtinguishFire();
            }
        }

        // 2. Visuals
        if (splashEffect != null)
        {
            Instantiate(splashEffect, transform.position, Quaternion.identity);
        }

        Debug.Log("Holy Water Exploded!");
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, blastRadius);
    }
}
