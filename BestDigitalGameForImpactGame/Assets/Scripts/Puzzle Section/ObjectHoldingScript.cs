using System;
using UnityEngine;

public class ObjectHoldingScript : MonoBehaviour
{
    private RaycastHit objectHit;
    private LayerMask PickupAbleMask;
    private LayerMask nullMask;
    public Transform CameraTrans;
    public Transform HoldTrans;
    public float fRayDist = 10.0f;
    public float fMoveForce = 1.0f;
    public float fSlowRadius = 1.0f;
    public GameObject HeldObject;
    private Rigidbody HeldRB;
    private CharacterController characterController;


    private void DropObject()
    {
        FixedJoint joint = HoldTrans.GetComponent<FixedJoint>();
        if (joint) Destroy(joint);
        
        HeldRB.useGravity = true;
        HeldRB.freezeRotation = false;
        HeldRB.linearVelocity = characterController.velocity;
        HeldRB = null;
        HeldObject = null;
    }
    private void Start()
    {
        PickupAbleMask = LayerMask.GetMask("Pickup");
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)  && !HeldObject && Physics.Raycast(transform.position,CameraTrans.forward,out objectHit,fRayDist,PickupAbleMask))
        {
            HeldObject = objectHit.transform.gameObject;
            HeldRB = HeldObject.GetComponent<Rigidbody>();
            if (!HeldRB)
            {
                Debug.LogError("Object Tagged Pickup without Rigidbody");
            }
            HeldRB.useGravity = false;
            HeldRB.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            HeldRB.interpolation = RigidbodyInterpolation.Interpolate;
            HeldRB.freezeRotation = true;

            FixedJoint joint = HoldTrans.gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = HeldRB;
            joint.breakForce = Mathf.Infinity;
            joint.breakTorque = Mathf.Infinity;
            joint.enableCollision = true;
        }
        else if (HeldObject )
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                DropObject();
            }
            else if(characterController.isGrounded && Physics.Raycast(transform.position,-transform.up,out objectHit,10f,PickupAbleMask))
            {
                if (objectHit.collider.gameObject == HeldObject)
                {
                    DropObject();
                }
            }
        }
        
    }
}
