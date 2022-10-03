using UnityEngine;
public class LookAt : MonoBehaviour
{
    public Transform lookPos;
    // Update is called once per frame
    void LateUpdate()
    {
        this.transform.LookAt(lookPos);
    }
}
