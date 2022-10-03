using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouseLook : MonoBehaviour
{
    float mouseSens = 100f;

    public Transform playerBody;
    public Transform cameraPos;
    
    float xRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -45f, 50f);
        if(cameraPos != null)
            transform.position = cameraPos.position;
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        if(playerBody != null)
        {
            playerBody.Rotate(Vector3.up * mouseX);
        }

    }
}
