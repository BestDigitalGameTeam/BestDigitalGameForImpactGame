using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private CharacterController playerController;
    private Vector3 m_vec3MoveDir;
    
    private bool m_bSprinting;
    public float m_fJumpForce = 1.0f;
    public float m_fGravityForce = 9.81f;
    
    public Camera PlayerCam;
    public float m_fSensitivity = 2.0f;
    private float m_fCamXLimit = 88.0f;
    private float rotationX = 0.0f;
    
    public float m_fMoveSpeed;
    public float m_fSprintSpeed;
    public bool m_bCanMove = true;

    public AudioSource m_defaultAudioPlayer;
    public AudioSource m_movementAudioPlayer;
    public AudioSource m_shootAudioPlayer;
    public AudioSource m_speedAudioPlayer;
    public AudioSource m_interactAudioPlayer;

    private LayerMask interactablesMask;
    
    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        interactablesMask = LayerMask.GetMask("Interactable");
    }

    // Update is called once per frame
    void Update()
    {

        
        //Movement
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        
        m_bSprinting = Input.GetKey(KeyCode.LeftShift);

        float fXSpeed = m_bCanMove ? (m_bSprinting ? m_fSprintSpeed : m_fMoveSpeed) * Input.GetAxis("Vertical") : 0.0f;
        float fYSpeed = m_bCanMove ? (m_bSprinting ? m_fSprintSpeed : m_fMoveSpeed) * Input.GetAxis("Horizontal") : 0.0f;
        float fMoveDirY = m_vec3MoveDir.y;
        m_vec3MoveDir = (forward * fXSpeed) + (right * fYSpeed);

        
        //Jump Controls
        if (Input.GetKey(KeyCode.Space) && m_bCanMove && playerController.isGrounded)
        {
            m_vec3MoveDir.y = m_fJumpForce;
        }
        else
        {
            m_vec3MoveDir.y = fMoveDirY;
        }
        
        //Gravity
        if(!playerController.isGrounded)
        {
            m_vec3MoveDir.y -= m_fGravityForce * Time.deltaTime;
        }
        playerController.Move(m_vec3MoveDir * Time.deltaTime);

        if (m_bCanMove)
        {
            //Camera movement
            rotationX += -Input.GetAxis("Mouse Y") * m_fSensitivity;
            rotationX = Mathf.Clamp(rotationX, -m_fCamXLimit, m_fCamXLimit);
            PlayerCam.transform.localRotation = Quaternion.Euler(rotationX, 0.0f, 0.0f);
            transform.rotation *= Quaternion.Euler(0.0f, Input.GetAxis("Mouse X"), 0.0f);

            if (Input.GetKey(KeyCode.LeftControl))
            {
                //Crouch - this can be implemented better
                transform.localScale = new Vector3(1.0f, 0.5f, 1.0f);
            }
            else
            {
                transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
        }

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
            else if (Input.GetKeyDown(KeyCode.Mouse0))
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
        }
    }
}
