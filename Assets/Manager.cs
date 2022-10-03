using UnityEngine;
using TMPro;

public class Manager : MonoBehaviour
{
    public GameObject holdPos;
    private GameObject heldObj;
    public float pickupRange = 1f;
    [SerializeField] private LayerMask pickUpLayer;
    // Update is called once per frame
    protected IKControl ikControl;

    [SerializeField]
    float playerHealth = 100f; // Your actual number
    public float PlayerHealth
    {
        get{
            return playerHealth;
        }
        set {
            playerHealth = Mathf.Round(value);
            healthUI.text = playerHealth.ToString();
        }
        
    }
    public TMP_Text healthUI; 
    void Start()
    {
        ikControl = GetComponent<IKControl>();
    }
    void Update()
    {
        if (Input.GetKeyDown("f"))
        {
            if (heldObj == null)
            {
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out RaycastHit raycastHit, pickupRange, pickUpLayer))
                {
                    PickupObject(raycastHit.transform.gameObject);
                }
            }
            else
            {
                DropObj();
            }
        }

        void PickupObject(GameObject pickObj)
        {
            if (pickObj.GetComponent<Rigidbody>())
            {
                Rigidbody heldObjRB = pickObj.GetComponent<Rigidbody>();
                heldObjRB.isKinematic = true;
                heldObjRB.constraints = RigidbodyConstraints.FreezeRotation;

                heldObjRB.transform.parent = holdPos.transform;
                heldObjRB.transform.localPosition = new Vector3(0, 0, 0);
                heldObjRB.transform.localRotation = Quaternion.Euler(0,0,0);
                heldObj = pickObj;

                Transform leftPos = pickObj.transform.Find("HandPointL");
                Transform rightPos = pickObj.transform.Find("HandPointR");

                ikControl.PickUp(rightPos, leftPos);
            }
        }

        void DropObj()
        {
                heldObj.GetComponent<Rigidbody>().isKinematic = false;
                heldObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                heldObj.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * heldObj.GetComponent<chair>().dropForce());
                heldObj.transform.parent = null;
                heldObj = null;
                ikControl.Drop();
            }
        }
}
