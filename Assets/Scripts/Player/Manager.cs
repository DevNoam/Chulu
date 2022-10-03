using UnityEngine;
using Mirror;

public class Manager : NetworkBehaviour
{
    [SerializeField]
    private Camera cam;
    public GameObject holdPos; // Where should the item be held in the character
    private GameObject heldObj; // Refrence to pickable object
    public float pickupRange = 1f; // The range the player can pickup objects
    [SerializeField] private LayerMask pickUpLayer; // Which layer can be pickable
    // Update is called once per frame
    //protected IKControl ikControl; // Refrence to IK modifier script, important for animations.

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
    [SerializeField]
    private characterCanvas characterCanvas; // Script that manages the Front end UI.



    //IK STUFF
    [SerializeField]
    private Animator animator;
    private bool ikActive = false;

    private Transform rightHandPick = null;
    private Transform leftHandPick = null;
    
    
    void Start() {
    }
    GameObject item;
    bool isLockedItem = false;
    Ray ray;
    public void Update()
    {
        if (!hasAuthority) return;
        ray = new Ray(cam.transform.position, cam.transform.forward);
        bool isItem = (Physics.Raycast(ray, out RaycastHit raycastHit, pickupRange, pickUpLayer));
        Debug.DrawRay(cam.transform.position, cam.transform.forward * pickupRange, Color.yellow);

        if(isItem && heldObj == null)
        {
            if(isLockedItem == false)
            {
                item = raycastHit.transform.gameObject;
                characterCanvas.itemName.text = item.name.ToString();
                characterCanvas.itemHealth.value = item.GetComponent<chair>().getHP;
                characterCanvas.pickupNotice.SetActive(true);
                isLockedItem = true;
                return;
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
            return;
        }
        if (heldObj != null && Input.GetKeyDown("e"))
        {
            DropObj();
            return;
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

            PickUp(rightPos, leftPos);
        }
    }

    void DropObj()
    {
        heldObj.GetComponent<Rigidbody>().isKinematic = false;
        heldObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        heldObj.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * heldObj.GetComponent<chair>().dropForce());
        heldObj.transform.parent = null;
        heldObj = null;
        Drop();
    }





#region IK

    //a callback for calculating IK
    private void OnAnimatorIK()
    {
        //if the IK is active, set the position and rotation directly to the goal.
        animator.SetLookAtWeight(1f, 0f, 1f);
        animator.SetLookAtPosition(ray.GetPoint(25));
        if(ikActive) {
        // Set the look target position, if one has been assigned
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandPick.position);
        animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandPick.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandPick.rotation);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandPick.rotation);

        }

        //if the IK is not active, set the position and rotation of the hand and head back to the original position
        else {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
            //animator.SetLookAtWeight(0);
        }
    }

    public void PickUp(Transform rightPos, Transform leftPos)
    {
        leftHandPick = leftPos;
        rightHandPick = rightPos;
        ikActive = true;
    }
    public void Drop()
    {
        leftHandPick = null;
        rightHandPick = null;
        ikActive = false;
    }
#endregion

}

