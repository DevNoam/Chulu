using UnityEngine;
using TMPro;

public class Manager : MonoBehaviour
{
    protected Camera camera;
    public GameObject holdPos; // Where should the item be held in the character
    private GameObject heldObj; // Refrence to pickable object
    public float pickupRange = 1f; // The range the player can pickup objects
    [SerializeField] private LayerMask pickUpLayer; // Which layer can be pickable
    // Update is called once per frame
    protected IKControl ikControl; // Refrence to IK modifier script, important for animations.

    [SerializeField]
    private float playerHealth = 100f; // Your actual number
    public float PlayerHealth // Health modifier
    {
        get{
            return playerHealth;
        }
        set {
            playerHealth = Mathf.Round(value);
            characterCanvas.healthUI.text = playerHealth.ToString();
        }
    }

    protected characterCanvas characterCanvas; // Script that manages the Front end UI.
    void Start()
    {
        camera = Camera.main;
        characterCanvas = GetComponent<characterCanvas>();
        ikControl = GetComponent<IKControl>();
    }
    GameObject item;
    bool isLockedItem = false;
    void Update()
    {
        bool isItem = (Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit raycastHit, pickupRange, pickUpLayer));
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * pickupRange, Color.yellow);

        if(isItem && heldObj == null)
        {
            if(isLockedItem == false)
            {
                item = raycastHit.transform.gameObject;
                characterCanvas.itemName.text = item.name.ToString();
                characterCanvas.itemHealth.value = item.GetComponent<chair>().getHP;
                characterCanvas.pickupNotice.SetActive(true);
                isLockedItem = true;
            }
            if(Input.GetKeyDown("e"))
            {
                characterCanvas.pickupNotice.SetActive(false);
                isLockedItem = false;
                PickupObject(raycastHit.transform.gameObject);
                return;
            }
        }
        else if(!isItem && characterCanvas.pickupNotice.activeSelf)
        {
            characterCanvas.pickupNotice.SetActive(false);
            isLockedItem = false;
        }
        if (heldObj != null && Input.GetKeyDown("e"))
            DropObj();

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

