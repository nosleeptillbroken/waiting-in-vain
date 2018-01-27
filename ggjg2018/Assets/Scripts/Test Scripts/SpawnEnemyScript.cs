using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemyScript : MonoBehaviour
{
    public GameObject Enemy;
    int spawnCount = 0;
    private int difficultyLevel = 30; //difficulty level is the ammount of enemies you want to be able to spawn at once before waiting to spawn

    /*
     Going to want to adjust how you control the spawn timing in the future. using invokerepeating is cool, but isn't exactl precise. I mean, we could use a float value for the time slot
     but I'm not sure how it'd affect it.
     */

    void Start ()
    {
        InvokeRepeating("SpawnEnemy", 0.0f, 1.5f);
	}
	
	void Update ()
    {
        

    }

    void SpawnEnemy()
    {
        

        if (spawnCount < difficultyLevel)
        {
        Instantiate(Enemy, new Vector3(Random.Range(-7.25f, 7.25f), -5f, -2f), transform.rotation);
            spawnCount+=1; //adds to the total enemies spawned
            //Debug.Log("count is:" + spawnCount);

        }
        
    }
}
