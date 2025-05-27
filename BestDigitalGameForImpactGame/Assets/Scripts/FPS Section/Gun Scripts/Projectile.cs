using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float lifeTime = 5.0f;
    private float fSpawnTime;

    private void Start()
    {
        fSpawnTime = Time.time;
    }

    private void Update()
    {
        if (fSpawnTime + lifeTime <= Time.time)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Hit: {collision.gameObject.name}");
        Destroy(gameObject); // Destroy on hit
    }
}