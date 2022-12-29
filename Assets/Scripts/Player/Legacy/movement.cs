using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class movement : NetworkBehaviour
{
    [SerializeField]
    private CharacterController controller;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private float speed = 12f;
    [SerializeField]
    private float jumpHeight = 1f;
    [ShowInInspector]
    protected Transform groundCheck;
    [ShowInInspector]
    protected float groundDistance = 0.1f;
    [SerializeField]
    private LayerMask groundMask;

    [SyncVar]
    Vector3 velocity;

    void Start()
    {

    }

    [Client]
    void Update()
    {
        if (!hasAuthority) return;

        if (CheckGround() && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        //Get parms
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        bool jump = Input.GetButtonDown("Jump");
        if (x != 0 || z != 0)
            animator.SetBool("move", true);
        else
            animator.SetBool("move", false);

        //Check if parms are set (Player moves)
        if (x != 0 || z != 0 || jump)
        {
            Vector3 move = (transform.right * x) + (transform.forward * z);
            controller.Move(move * speed * Time.deltaTime);
        }
        if (jump && CheckGround())
        {
            animator.SetTrigger("jump");
            velocity.y += Mathf.Sqrt(jumpHeight * -2.0f * Physics.gravity.y);
        }

        velocity.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

    }

    public bool CheckGround()
    {
        return Physics.Raycast(this.transform.position, Vector3.down, groundDistance + 0.1f);
    }
}
