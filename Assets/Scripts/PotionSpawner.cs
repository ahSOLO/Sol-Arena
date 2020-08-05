using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PotionSpawner : MonoBehaviour
{
    float spawnTimer;
    [SerializeField] float spawnTime;
    [SerializeField] Potion potion;
    
    // Start is called before the first frame update
    void Start()
    {
        spawnTimer = spawnTime;
    }

    // Update is called once per frame
    void Update()
    {
        // Spawn potion when timer is up
        spawnTimer -= Time.deltaTime;
        if (spawnTimer < 0)
        {
            if (GameObject.FindWithTag("Potion") == null) // Only spawn if a potion does not already exist
            {
                Instantiate(potion, transform.position, quaternion.identity);
                spawnTimer = spawnTime;
            }
        }
    }
}
