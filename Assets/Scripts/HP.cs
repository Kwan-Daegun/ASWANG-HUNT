using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class HP : MonoBehaviour
{
    [SerializeField] private string type;
    [SerializeField] private GameObject HPGO;
    [SerializeField] private GameObject BarGO;
    [SerializeField] private Image BarMask;
    [SerializeField] public float currentBarValue;
    [SerializeField] private int bulletDamage = 0;
    float maxBarValue = 100;

    void Start()
    {
        BarGO.SetActive(true);
    }

    public void AddHealth(float number)
    {
        if (number <= 0) return;
        currentBarValue = Mathf.Clamp(currentBarValue + number, 0, maxBarValue);
        /*if (currentBarValue >= maxBarValue)
        {
            //off the player's collider
        }*/
        float fill = currentBarValue / maxBarValue;
        BarMask.fillAmount = fill;
    }

    public void SubHealth(float number)
    {
        if (number <= 0) return;
        currentBarValue = Mathf.Clamp(currentBarValue - number, 0, maxBarValue);
        /*if (currentBarValue >= maxBarValue)
        {
            //off the player's collider
        }*/
        float fill = currentBarValue / maxBarValue;
        BarMask.fillAmount = fill;
    }

    private void Update()
    {
        if (currentBarValue <= 0)
        {
            if (type == "Enemy")
            {
                Main.MinusEnemyCounter();
                print(Main.lvl + " lvl");
                print(Main.enemyCounter + "EC");

                //  Link to EnemyAI for item drop
                EnemyAI enemyAI = GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    enemyAI.Die();
                    return; // ensure Die() handles destroy
                }
            }

            Destroy(gameObject);
            BarGO.SetActive(false);
        }

        //HPGO.transform.rotation = Quaternion.Euler(0, 0, 0);
        //BarGO.transform.position.x *= Vector(-1f);
        //BarGO.transform.localScale = new Vector2(1,1);
        //BarGO.transform.position = new Vector2(BarGO.transform.position.x * -1, BarGO.transform.position.y);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(type == "House")
        {
            if(collision.CompareTag("Enemy"))
            {
                Main.MinusEnemyCounter();
                print(Main.lvl + " lvl");

                print(Main.enemyCounter + "EC");
                SubHealth(10 /*enemyDamage*/);
            }
        }

        if(type == "Enemy")
        {
            if(collision.CompareTag("Bullet"))
            {
                SubHealth(bulletDamage /*bulletDamage*/);
            }            
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (type == "Player")
        {
            if (collision.collider.CompareTag("Enemy"))
            {
                SubHealth(5 /*enemyDamage*/);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (type == "Player")
        {
            if (collision.collider.CompareTag("Enemy"))
            {
                
                SubHealth(1 /*enemyDamage*/);
            }
        }
    }


}
