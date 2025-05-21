using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector3 m_vec3Velocity = Vector3.zero;

    private bool m_bSprinting;
    private bool m_bGroundedPlayer;
    private CharacterController m_ccPlayerController;
    public float m_fJumpHeight = 1.0f;
    public float m_fGravity = -9.81f;
    

    public float m_fMoveSpeed;

    public float m_fSprintSpeed;
    // Start is called before the first frame update
    void Start()
    {
        m_ccPlayerController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        m_bGroundedPlayer = m_ccPlayerController.isGrounded;
        m_bSprinting = Input.GetKeyDown(KeyCode.LeftShift);
        m_vec3Velocity = Vector2.zero;
        m_vec3Velocity = new Vector3(Input.GetAxis("Horizontal"),0, Input.GetAxis("Vertical"));
        m_vec3Velocity *= m_bSprinting ? m_fMoveSpeed : m_fSprintSpeed; // sprinting
        m_ccPlayerController.Move(m_vec3Velocity);
        transform.forward = (m_vec3Velocity == Vector3.zero) ? transform.forward : m_vec3Velocity;
        m_vec3Velocity.y = (Input.GetKeyDown(KeyCode.Space) && m_bGroundedPlayer) ? Mathf.Sqrt(m_fJumpHeight * -2.0f * m_fGravity) : 0.0f;
        m_vec3Velocity.y += m_fGravity * Time.deltaTime;
        m_ccPlayerController.Move(m_vec3Velocity * Time.deltaTime);
    }
}
