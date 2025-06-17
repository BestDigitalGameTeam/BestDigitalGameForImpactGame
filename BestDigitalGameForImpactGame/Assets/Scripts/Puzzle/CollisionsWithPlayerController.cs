using System;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Collider),typeof(Rigidbody))]
public class CollisionsWithPlayerController : MonoBehaviour
{
    private Rigidbody ObjRB;
    private CharacterController playerController;

    private void Start()
    {
        ObjRB = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!playerController)
            {
                playerController = other.transform.parent.GetComponent<CharacterController>();
            }

            ObjRB.linearVelocity = playerController.velocity;
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ObjRB.linearVelocity = playerController.velocity;
        }
    }
}
