using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public static Main Instance;
    public static int lvl = 1;
   // EnemySpawner2 EnemySpawner;
    
    public static int enemyCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
       // EnemySpawner = GetComponent<EnemySpawner2>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void PlusEnemyCounter()
    {
        enemyCounter++;
        print(enemyCounter + " +");
    }

    public static void MinusEnemyCounter()
    {
        enemyCounter--;
        print(enemyCounter + " -");
    }
}
