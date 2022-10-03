using UnityEngine;

public class HandsFollow : MonoBehaviour
{
    public Transform lookPos;
    public Transform cam;

    void Update()
    {
        this.transform.position = lookPos.position;
        this.transform.localRotation = Quaternion.Euler(cam.localEulerAngles.x, 0f, 0f);

    }
}
