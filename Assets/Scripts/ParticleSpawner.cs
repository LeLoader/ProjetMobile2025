using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class ParticleSpawner : MonoBehaviour
{
    [SerializeField] GameObject particlePrefab;
    [SerializeField] Vector3 initialVelocity;
    [SerializeField, MinValue(0), Tooltip("Objet/s")] float spawnRate;

    bool bShouldSpawn = false;
    float timeSinceLastSpawn;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (bShouldSpawn)
        {
            if (Mathf.Abs(timeSinceLastSpawn - Time.time) > 1 / spawnRate)
            {
                timeSinceLastSpawn = Time.time;
                Debug.Log("Spawn!");
                GameObject particle = Instantiate(particlePrefab, transform.position, Quaternion.identity);
                particle.GetComponent<Rigidbody2D>().linearVelocity = initialVelocity;
            }
        }
    }

    [Button]
    void StartSim()
    {
        bShouldSpawn = true;
    }

    [Button]
    void StopSim()
    {
        bShouldSpawn = false;
    }
}
