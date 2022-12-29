using UnityEngine;
using Mirror;

public class HandsFollow : NetworkBehaviour 
{
    [SerializeField]
    private Transform lookPos;
    [SerializeField]
    private Transform cam;
    [SerializeField]
    private Transform target;
    void Start()
    {
        if(!hasAuthority) { this.enabled = false; return;};
    }

    [ClientCallback]
    void Update()
    {
        if(!hasAuthority) {return;}
        target.transform.position = lookPos.position;
        target.transform.localRotation = Quaternion.Euler(cam.localEulerAngles.x, 0f, 0f);
    }
}
