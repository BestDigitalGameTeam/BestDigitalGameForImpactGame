using System;
using UnityEngine;

public class ObjectHoldingScript : MonoBehaviour
{
    private RaycastHit objectHit;
    private LayerMask PickupAbleMask;
    private LayerMask PlayerMask;
    private LayerMask nullMask;
    public Transform CameraTrans;
    public Transform HoldTrans;
    public float fRayDist = 5.0f;
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
        HeldRB.excludeLayers = nullMask;
        HeldRB = null;
        HeldObject = null;
    }
    private void Start()
    {
        PickupAbleMask = LayerMask.GetMask("Pickup");
        characterController = GetComponent<CharacterController>();
        PlayerMask = LayerMask.GetMask("Player");
        nullMask = LayerMask.GetMask();
    }

    private void Update()
    {
        if (!HeldObject && Physics.Raycast(CameraTrans.position, CameraTrans.forward, out objectHit, fRayDist, PickupAbleMask))
        {
            UIManager.Instance.ShowInteractUI.Invoke();
            if (Input.GetKeyDown(KeyCode.E))
            {
                HeldObject = objectHit.transform.gameObject;
                HeldObject.transform.position = HoldTrans.position;

                HeldRB = HeldObject.GetComponent<Rigidbody>();
                if (!HeldRB)
                {
                    Debug.LogError("Object Tagged Pickup without Rigidbody");
                }

                HeldRB.excludeLayers = PlayerMask;
                HeldRB.useGravity = false;
                HeldRB.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                HeldRB.interpolation = RigidbodyInterpolation.Interpolate;
                HeldRB.freezeRotation = true;

                FixedJoint joint = HoldTrans.gameObject.AddComponent<FixedJoint>();
                joint.connectedBody = HeldRB;
                joint.breakForce = Mathf.Infinity;
                joint.breakTorque = Mathf.Infinity;
                joint.enableCollision = true;

                UIManager.Instance.HideInteractUI.Invoke();
            }
        }
        else if (HeldObject)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                DropObject();
            }
            else if (characterController.isGrounded && Physics.Raycast(transform.position, -transform.up, out objectHit, 10f, PickupAbleMask))
            {
                if (objectHit.collider.gameObject == HeldObject)
                {
                    DropObject();
                }
            }
        }
        else
        {
            if (UIManager.Instance.IntActive) UIManager.Instance.HideInteractUI.Invoke();
        }
        
    }
}
