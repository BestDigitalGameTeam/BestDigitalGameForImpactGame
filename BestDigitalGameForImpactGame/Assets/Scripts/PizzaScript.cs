using System;
using UnityEngine;

public class PizzaScript : MonoBehaviour
{
    private BoxCollider HitBox;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HitBox = GetComponent<BoxCollider>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Projectile"))
        {
            foreach(var Slice in GetComponentsInChildren<Transform>())
            {
                Slice.parent = null;
                Slice.gameObject.GetComponent<MeshRenderer>().enabled = true;
                Slice.gameObject.AddComponent<Rigidbody>();
            }
            gameObject.SetActive(false);
        }
    }

    
        
    
}
