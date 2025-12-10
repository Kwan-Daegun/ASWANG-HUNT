using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEffects : MonoBehaviour
{
    public float lifeTime;

    void Update()
    {
        lifeTime -= Time.deltaTime;

        if (lifeTime < 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }

/*    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            enemy.subHealth(bulletDmg);
            Destroy(gameObject);
        }
    }*/
}
