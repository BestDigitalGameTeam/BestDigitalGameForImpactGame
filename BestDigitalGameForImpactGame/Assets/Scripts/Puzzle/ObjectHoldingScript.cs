using System;
using UnityEngine;

public class ObjectHoldingScript : MonoBehaviour
{
    private RaycastHit objectHit;
    private LayerMask PickupAbleMask;
    private LayerMask nullMask;
    private LayerMask PlayerMask;
    public Transform CameraTrans;
    public Transform HoldTrans;
    public float fRayDist = 10.0f;
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
        PlayerMask = LayerMask.GetMask("Player");
        nullMask = LayerMask.GetMask();
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (!HeldObject && Physics.Raycast(CameraTrans.position, CameraTrans.forward, out objectHit, fRayDist, PickupAbleMask))
        {
            UIManager.Instance.ShowInteractUI.Invoke();
            if (Input.GetKeyDown(KeyCode.E))
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
                HeldRB.excludeLayers = PlayerMask;
                HeldObject.transform.position = HoldTrans.position;

                FixedJoint joint = HoldTrans.gameObject.AddComponent<FixedJoint>();
                joint.connectedBody = HeldRB;
                joint.connectedAnchor = Vector3.zero;
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
        }
        else
        {
            if (UIManager.Instance.IntActive) UIManager.Instance.HideInteractUI.Invoke();
        }
        
    }
}
