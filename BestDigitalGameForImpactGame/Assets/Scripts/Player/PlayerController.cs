using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : SingletonPersistent<PlayerController>
{
    [SerializeField] private GameObject[] guns; // Assign guns in Inspector
    private int m_iCurrentGunIndex = 0;
    
    private CharacterController playerController;
    private Vector3 m_vec3MoveDir;
    
    private bool m_bSprinting;
    private float m_fBaseJumpForce = 5.0f;
    public float m_fJumpForce = 5.0f;
    public float m_fGravityForce = 9.81f;
    
    public Camera PlayerCam;
    public float m_fSensitivity = 2.0f;
    private float m_fCamXLimit = 88.0f;
    private float rotationX = 0.0f;
    
    public float m_fMoveSpeed;
    public float m_fSprintSpeed;
    public float m_fAirResistance = 1.5f;
    public bool m_bCanMove = true;
    public bool m_bWasSprinting;
    

    public AudioSource m_defaultAudioPlayer; 

    private LayerMask interactablesMask;

    public UnityEvent SwitchWeapon; // to cancel the reload when swapping weapon to fix soft locking all guns

    
    //Public Ledge functions
    public void EnteredLedge()
    {
        m_bCanMove = false;
    }
    
    public void ExitedLedge()
    {
        m_bCanMove = true;
    }

    public void ApplyJumpForce(float _fJumpPadForce)
    {
        m_vec3MoveDir.y = _fJumpPadForce;
    }

    public void SetVelocity(Vector3 _velocity)
    {
        m_vec3MoveDir = _velocity;
    }

    public void AddVelocity(Vector3 _velocity)
    {
        m_vec3MoveDir += _velocity;
    }

    public void Jump()
    {
        m_vec3MoveDir.y = m_fJumpForce;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            m_bWasSprinting = true;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        //Establishing variables
        playerController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        interactablesMask = LayerMask.GetMask("Interactable");
        m_fBaseJumpForce = m_fJumpForce;
        
        ActivateGun(m_iCurrentGunIndex);
    }

    private void ActivateGun(int index)
    {
        for (int i = 0; i < guns.Length; i++)
        {
            guns[i].SetActive(i == index);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        { GameManager.Instance.PauseGame.Invoke(!GameManager.Instance.isPaused); }

        // Scroll Selected Gun
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        //if (scroll > 0f)
        //{
        //    SwitchWeapon.Invoke();
        //    m_iCurrentGunIndex = (m_iCurrentGunIndex + 1) % guns.Length;

        //    ActivateGun(m_iCurrentGunIndex);
        //}
        //else if (scroll < 0f)
        //{
        //    SwitchWeapon.Invoke();
        //    m_iCurrentGunIndex = (m_iCurrentGunIndex - 1 + guns.Length) % guns.Length;
        //    ActivateGun(m_iCurrentGunIndex);
        //}
        // ---
        
        
        //Movement

        if (m_bWasSprinting && playerController.isGrounded)
        {
            m_bWasSprinting = false;
        }
        
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        
        m_bSprinting = Input.GetKey(KeyCode.LeftShift);

        float fXSpeed = m_bCanMove ? (m_bSprinting && playerController.isGrounded ? m_fSprintSpeed : m_bWasSprinting ? m_fSprintSpeed : m_fMoveSpeed) * Input.GetAxis("Vertical") : 0.0f;
        float fYSpeed = m_bCanMove ? (m_bSprinting && playerController.isGrounded ? m_fSprintSpeed : m_bWasSprinting ? m_fSprintSpeed : m_fMoveSpeed) * Input.GetAxis("Horizontal") : 0.0f;
        float fMoveDirY = m_vec3MoveDir.y;
        
        if (m_bCanMove)
        {
            //If not on ledge
            m_vec3MoveDir = (forward * (playerController.isGrounded ? fXSpeed : (fXSpeed/m_fAirResistance)) + (right * (playerController.isGrounded ? fYSpeed : fYSpeed/m_fAirResistance)));
        }
        
        
        //Jump Controls
        if (Input.GetKey(KeyCode.Space) && m_bCanMove && playerController.isGrounded)
        {
            Jump();
        }
        else if(m_bCanMove)
        {
            m_vec3MoveDir.y = fMoveDirY;
        }
        
        //Gravity
        if(!playerController.isGrounded && m_bCanMove)
        {
            m_vec3MoveDir.y -= m_fGravityForce * Time.deltaTime;
        }

        if (playerController.enabled)
        {
            playerController.Move(m_vec3MoveDir * Time.deltaTime);
        }

        if (Time.timeScale > 0)
        {
            //Camera movement
            rotationX += -Input.GetAxis("Mouse Y") * m_fSensitivity;
            rotationX = Mathf.Clamp(rotationX, -m_fCamXLimit, m_fCamXLimit);
            PlayerCam.transform.localRotation = Quaternion.Euler(rotationX, 0.0f, 0.0f);
            transform.rotation *= Quaternion.Euler(0.0f, Input.GetAxis("Mouse X"), 0.0f);

            if (Input.GetKey(KeyCode.LeftControl))
            {
                //Crouch - this can be implemented better
                playerController.height = 0.5f;
            }
            else
            {
                playerController.height = 2.0f;
            }
        }
        
        //Disabled sound effects cause it was getting annoying
        /*
        if (Input.anyKeyDown)
        {
            //Input Sound Effects
            if (!Input.GetKeyDown(KeyCode.Space) && !Input.GetKeyDown(KeyCode.E) && !Input.GetKeyDown(KeyCode.LeftShift) && !Input.GetKeyDown(KeyCode.Mouse0) && !Input.GetKeyDown(KeyCode.W) && !Input.GetKeyDown(KeyCode.A) && !Input.GetKeyDown(KeyCode.S) && !Input.GetKeyDown(KeyCode.D))
            {
                m_defaultAudioPlayer.pitch = Random.Range(0.1f, 1.0f);
                m_defaultAudioPlayer.Play();
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.W))
            {
                //Currently disabled audio source cause it was getting annoying
                //m_movementAudioPlayer.pitch = Random.Range(0.85f, 1.15f);
                //m_movementAudioPlayer.Play();
            }
            // Moving to Gun.cs - August
            /*else if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                m_shootAudioPlayer.pitch = Random.Range(0.85f, 1.15f);
                m_shootAudioPlayer.Play();
            }
            else if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                m_speedAudioPlayer.pitch = Random.Range(0.85f, 1.15f);
                m_speedAudioPlayer.Play();
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                if (Physics.Raycast(PlayerCam.transform.position,PlayerCam.transform.TransformDirection(Vector3.forward),Mathf.Infinity,interactablesMask))
                {
                    m_interactAudioPlayer.pitch = Random.Range(0.85f, 1.15f);
                    m_interactAudioPlayer.Play();
                }
            }
        }*/
    }
}
