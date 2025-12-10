using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner2 : MonoBehaviour
{
    public GameObject[] wave1;
    public GameObject[] wave2;
    public GameObject[] wave3;
    public int currentlvl = 0;
    public float spawnTime = 2;
    private float timer;
    private int currentEnemy = 0;
    public bool spawning;
    // Start is called before the first frame update
    void Start()
    {
        currentlvl++;
        timer = spawnTime;
        spawning = true;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        timer -= Time.deltaTime;
        if (currentlvl != Main.lvl) 
        {
            currentlvl++;
            currentEnemy = 0;
        }

        if(timer <= 0)
        {
            SpawnEnemy();
        }             
        
    }

    void SpawnEnemy()
    {
        timer = spawnTime;
        if (Main.lvl == 1)
        {
            if (currentEnemy < wave1.Length)
            {
                spawning = true;
                Instantiate(wave1[currentEnemy], transform.position, Quaternion.identity);
                //this.enabled = false;
            }
            else
            {
                spawning = false;
                return;
            }
        }
        else if (Main.lvl == 2) 
        {
            if (currentEnemy < wave2.Length)
            {
                spawning = true;
                Instantiate(wave2[currentEnemy], transform.position, Quaternion.identity);
                //this.enabled = false;
            }
            else
            {
                spawning = false;
                return;
            }
        }
        else if (Main.lvl == 3)
        {
            
            if (currentEnemy < wave3.Length)
            {
                spawning = true;
                Instantiate(wave3[currentEnemy], transform.position, Quaternion.identity);
                //this.enabled = false;
            }
            else
            {
                spawning = false;
                return;
            }
        }
        Main.PlusEnemyCounter();
        currentEnemy++;
        
    }
}
