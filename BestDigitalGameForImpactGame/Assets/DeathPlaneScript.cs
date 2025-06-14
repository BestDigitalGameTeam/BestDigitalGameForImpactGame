using System;
using UnityEngine;

public class DeathPlaneScript : MonoBehaviour
{
    public Transform SpawnPos;
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.transform.position = SpawnPos.position;
        }
    }
}
