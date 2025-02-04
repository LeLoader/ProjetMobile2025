using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class ParticleSpawner : MonoBehaviour
{
    [SerializeField] GameObject particlePrefab;
    [SerializeField] Vector3 initialVelocity;
    [SerializeField, MinValue(0), Tooltip("Particle/s")] float spawnRate;

    [SerializeField] List<GameObject> particles;

    [Header("Visual")]
    [SerializeField] Camera effectCamera;
    [SerializeField] RenderTexture targetTexture;

    bool bShouldSpawn = false;
    float timeSinceLastSpawn;

    private void Start()
    {
        effectCamera.targetTexture = targetTexture;
    }

    void FixedUpdate()
    {
        if (bShouldSpawn)
        {
            if (Mathf.Abs(timeSinceLastSpawn - Time.time) > 1 / spawnRate)
            {
                timeSinceLastSpawn = Time.time;
                GameObject particle = Instantiate(particlePrefab, transform.position, Quaternion.identity, transform);
                particle.GetComponent<Rigidbody2D>().linearVelocity = initialVelocity;
                particles.Add(particle);
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
