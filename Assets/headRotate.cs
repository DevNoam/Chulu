using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class headRotate : MonoBehaviour
{
    void update()
    {
        this.transform.rotation = Camera.main.transform.rotation;
    }
}
