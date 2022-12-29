using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class mouseLook : NetworkBehaviour
{
    float mouseSens = 100f;
    
    [SerializeField]
    private Transform Target;
    [SerializeField]
    private Transform playerBody;
    [SerializeField]
    private Transform cameraPos;
    
    float xRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        if(!hasAuthority) { this.enabled = false; return;};
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    [ClientCallback]
    void Update()
    {
        if (!hasAuthority)
            return;
        //Client mouse pos generation
        float mouseX = Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -50f, 46f);
        if(cameraPos != null)
            Target.position = cameraPos.position;
        Target.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);

        //Upload data to server
        CmdRotation(xRotation);
    }
    [Command]
    void CmdRotation(float xRotation)
    {
        Target.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        RpcRotation();
    }
    [ClientRpc]
    void RpcRotation()
    {
        Target.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
