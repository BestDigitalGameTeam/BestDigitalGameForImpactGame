using UnityEngine;

public class LedgeGrabScript : MonoBehaviour
{
    private PlayerController Controller;
    public Transform CamTrans;
    public CharacterController CharController;

    public float m_fMoveToLedgeSpeed;
    public float m_fMaxLedgeGrabDistance;
    public float m_fMinTimeOnLedge;
    private float m_fTimeOnLedge;
    private bool m_bOnLedge;
    
    public float m_fLedgeDetectionLength;
    public float m_fLedgeCastRadius;
    private LayerMask LedgeMask;

    private Transform lastLedge;
    private Transform currentLedge;

    private RaycastHit ledgeCast;

    private void LedgeDetection()
    {
        //Check if player is looking at a ledge and is in range
        bool bLedgeDetected = Physics.SphereCast(transform.position, m_fLedgeCastRadius, CamTrans.forward,
            out ledgeCast, m_fLedgeDetectionLength, LedgeMask);
        
        if (!bLedgeDetected || CharController.isGrounded) return;
        
        float fDistanceToLedge = Vector3.Distance(transform.position, ledgeCast.transform.position);

        if (ledgeCast.transform == lastLedge) return;
        
        if (fDistanceToLedge < m_fMaxLedgeGrabDistance && !m_bOnLedge)
        {
            EnterLedge();
        }
    }

    private void EnterLedge()
    {
        //Enter ledge trigger
        m_bOnLedge = true;

        currentLedge = ledgeCast.transform;
        lastLedge = currentLedge;

        Controller.EnteredLedge();
    }

    private void HoldCharacterOnLedge()
    {
        //Moves player towards ledge and holds them there
        Vector3 dirToLedge = currentLedge.position - transform.position;
        float distToLedge = Vector3.Distance(transform.position, currentLedge.position);

        if (distToLedge > 0.01f)
        {
            Controller.m_vec3MoveDir = dirToLedge.normalized * (m_fMoveToLedgeSpeed * Time.deltaTime);
        }
        
        if(distToLedge>m_fMaxLedgeGrabDistance) ExitLedge();
    }

    private void ExitLedge()
    {
        //Exit ledge trigger
        m_bOnLedge = false;
        m_fTimeOnLedge = 0.0f;
        Controller.ExitedLedge();
        
        //CoolDown on same ledge so cant just jump back on
        Invoke("ResetLastLedge", 1f);
    }

    private void ResetLastLedge()
    {
        lastLedge = null;
    }
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LedgeMask = LayerMask.GetMask("Ledge");
        Controller = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_bOnLedge)
        {
            HoldCharacterOnLedge();
            m_fTimeOnLedge += Time.deltaTime;
            if (m_fTimeOnLedge >= m_fMinTimeOnLedge &&
                (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0 || Input.GetKeyDown(KeyCode.Space)))
            {
                //once initial hold time has past (stopping the ledges just becoming a ladder) exiting player from ledge
                ExitLedge();
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    //manually override jump controls when jumping off of ledge
                    Controller.m_vec3MoveDir.y = Controller.m_fJumpForce;
                }
            }
        }
        
        //Run checks for ledges in range
        LedgeDetection();
    }
}
