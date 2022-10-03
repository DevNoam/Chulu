using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class movement : NetworkBehaviour 
{
    protected CharacterController controller;
    protected Animator animator;

    public float speed = 12f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.1f;
    public LayerMask groundMask;

    Vector3 velocity;

    void Start()
    {
        if (!isLocalPlayer) DestroyImmediate(this);
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;
        if (CheckGround() && velocity.y < 0)
            velocity.y = -2f;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        if (x != 0 || z != 0)
            animator.SetBool("move", true);
        else
            animator.SetBool("move", false);

        Vector3 move = (transform.right * x) + (transform.forward * z);
        controller.Move(move * speed * Time.deltaTime);
        //Movement animation;

        if (Input.GetButtonDown("Jump") && CheckGround())
        {
            animator.SetTrigger("jump");
            velocity.y += Mathf.Sqrt(jumpHeight * -2.0f * Physics.gravity.y);
        }
        velocity.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    public bool CheckGround()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundDistance + 0.1f);
    }
}
