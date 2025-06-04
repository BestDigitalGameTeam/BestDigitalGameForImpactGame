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
    public float fMoveForce = 1.0f;
    public float fSlowRadius = 1.0f;
    public GameObject HeldObject;
    private Rigidbody HeldRB;
    private CharacterController characterController;


    private void MoveObj()
    {
        if (Vector3.Distance(HeldObject.transform.position, HoldTrans.position) > fSlowRadius)
        {
            HeldRB.AddForce((HoldTrans.position-HeldObject.transform.position) * fMoveForce);
        }
        else
        {
            HeldRB.linearVelocity = Vector3.zero;
        }
        
    }
    private void Start()
    {
        PickupAbleMask = LayerMask.GetMask("Pickup");
        PlayerMask = LayerMask.GetMask("Player");
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)  && !HeldObject && Physics.Raycast(transform.position,CameraTrans.forward,out objectHit,fRayDist,PickupAbleMask))
        {
            HeldObject = objectHit.transform.gameObject;
            HeldRB = HeldObject.GetComponent<Rigidbody>();
            HeldRB.useGravity = false;
            HeldRB.transform.parent = HoldTrans;
            HeldRB.freezeRotation = true;
            nullMask = HeldRB.excludeLayers;
            HeldRB.excludeLayers = PlayerMask;
        }
        else if (Input.GetKeyDown(KeyCode.E) && HeldObject)
        {
            HeldRB.useGravity = true;
            HeldRB.transform.parent = null;
            HeldRB.freezeRotation = false;
            HeldRB.excludeLayers = nullMask;
            HeldRB.linearVelocity = characterController.velocity;
            HeldObject = null;
            HeldRB = null;
        }

        if (HeldRB)
        {
            MoveObj();
        }
    }
}
