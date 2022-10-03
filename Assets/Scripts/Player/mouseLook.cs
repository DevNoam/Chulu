using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class mouseLook : NetworkBehaviour
{
    float mouseSens = 100f;
    public Transform Target;
    public Transform playerBody;
    public Transform cameraPos;
    
    float xRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        if (!hasAuthority) 
        {
            DestroyImmediate(this.gameObject);
        }
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority)
            return; 
        float mouseX = Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -50f, 46f);
        if(cameraPos != null)
            Target.position = cameraPos.position;
        Target.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        if(playerBody != null)
        {
            playerBody.Rotate(Vector3.up * mouseX);
        }

    }
}
