using System;
using UnityEngine;

public class FollowPlatformScript : MonoBehaviour
{
    public float m_fRayDist;
    private RaycastHit platformCast;
    private LayerMask platformMask;
    private SplineFollower m_currentSpline;
    private GameObject m_currentPlatform;
    private CharacterController characterController;

    void Start()
    {
        platformMask = LayerMask.GetMask("Platform");
        characterController = GetComponent<CharacterController>();
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
        if (m_currentPlatform && m_currentSpline)
        {
            characterController.Move(m_currentSpline.GetDeltaPos());
        }
        
    }
}

