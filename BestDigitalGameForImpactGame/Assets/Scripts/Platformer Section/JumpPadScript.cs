using System;
using UnityEngine;

public class JumpPadScript : MonoBehaviour
{
    public float m_fJumpPadForce = 10.0f;
    
    
    //Changes Player jump force to m_fJumpPadForce while standing on the jump pad
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.transform.position.y > transform.position.y)
            {
                //Collider does extend underneath jump pad - preventing this from causing issues
                //TODO Fix in the future
                other.gameObject.GetComponentInParent<PlayerController>().ApplyJumpForce(m_fJumpPadForce);
            }
        }
    }
}
