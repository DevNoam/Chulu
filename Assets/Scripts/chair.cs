using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class chair : NetworkBehaviour
{
    [Range(0f, 10000f)]
    [Tooltip("Damage to players")]
    [SerializeField]
    private float damageDeal = 0f;
    [Tooltip("The damage delt to the chair when hitting object")]
    [Range(0f, 100f)]
    [SerializeField]
    private float selfDamageMultiplier = 2f;
    [Tooltip("Chair health level besfore breakage")]
    [SerializeField] //temp
    [SyncVar]
    private float chairHP = 100f;

    public float getHP // Health modifier
    {
        get{
            return chairHP;
        }
    }


    [SerializeField]
    private float minDropForce = 0;
    [SerializeField]
    private float maxDropForce = 10;

    protected Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    [Server]
    private void OnCollisionEnter(Collision other) {
        if(rb.velocity.magnitude > 0.9)
        {
            chairHP -= (rb.velocity.magnitude * (selfDamageMultiplier));
        }
        if(other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<Manager>().PlayerHealth -= (damageDeal * rb.velocity.magnitude);
        }
        if(chairHP <= 0)
        {   
            //Destroy(this.gameObject);
            Debug.Log("Destroy chair");
        }
    }

    public float dropForce()
    {
        float force = Random.Range(minDropForce, maxDropForce);
        return force;
    }
}
