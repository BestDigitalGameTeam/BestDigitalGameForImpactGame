using System;
using UnityEngine;

public class FollowPlatformScript : MonoBehaviour
{
    public float m_fRayDist;
    public Vector3 m_SpawnPos; // Notused for this script - used by death plane dont judge me
    private RaycastHit platformCast;
    private LayerMask platformMask;
    private SplineFollower m_currentSpline;
    private GameObject m_currentPlatform;
    private CharacterController characterController;
    private Rigidbody ObjectRigidBody;

    void Start()
    {
        m_SpawnPos = transform.position;
        platformMask = LayerMask.GetMask("Platform");
        ObjectRigidBody = GetComponent<Rigidbody>();
        characterController = GetComponent<CharacterController>();
        if (ObjectRigidBody == null && characterController == null)
        {
            ObjectRigidBody = gameObject.AddComponent<Rigidbody>();
            ObjectRigidBody.useGravity = true;
        }
    }

    void Update()
    {
        if (Physics.Raycast(transform.position, -transform.up, out platformCast, m_fRayDist, platformMask))
        {
            //if standing on platform
            if (m_currentPlatform != platformCast.transform.gameObject && platformCast.transform.gameObject.CompareTag("MovingPlatform"))
            {
                m_currentSpline = platformCast.transform.GetComponent<SplineFollower>();
                m_currentPlatform = platformCast.transform.gameObject;
            }
        }
        else
        {
            m_currentSpline = null;
            m_currentPlatform = null;
        }
        
    }

    void LateUpdate()
    {
        //Moving player based on platform velocity
        if (m_currentPlatform && m_currentSpline && characterController)
        {
            //Is Player
            characterController.Move(m_currentSpline.GetDeltaPos());
        }
        else if (m_currentPlatform && m_currentSpline && ObjectRigidBody)
        {
            ObjectRigidBody.MovePosition(ObjectRigidBody.position+m_currentSpline.GetDeltaPos());
        }
        
    }
}

