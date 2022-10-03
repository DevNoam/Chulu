using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bot : MonoBehaviour
{
    public float health = 100f;
    public void damage(float amount = 0f)
    {
        health -= amount;
        if(health <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
