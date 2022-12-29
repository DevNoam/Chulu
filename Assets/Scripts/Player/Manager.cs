using UnityEngine;
using Mirror;

public class Manager : NetworkBehaviour
{
    [SerializeField]
    private Camera cam;
    public GameObject holdPos; // Where should the item be held in the character
    [SyncVar]
    private GameObject heldObj; // Refrence to pickable object
    [SerializeField]
    protected float pickupRange = 1f; // The range the player can pickup objects
    [SerializeField] private LayerMask pickUpLayer; // Which layer can be pickable
    // Update is called once per frame
    //protected IKControl ikControl; // Refrence to IK modifier script, important for animations.

    [SerializeField]
    [SyncVar]
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
    [SerializeField]
    private NetworkAnimator netAnimator;
    [SyncVar]
    private bool ikActive = false;

    private Transform rightHandPick = null;
    private Transform leftHandPick = null;
    
    bool isLockedItem = false;
    [SyncVar]
    Ray ray;


    //Movement stuff
    [Header("Movement")]
        [SerializeField]
    private CharacterController controller;
    [SerializeField]
    private float speed = 12f;
    [SerializeField]
    private float jumpHeight = 1f;
    [ShowInInspector]
    protected Transform groundCheck;
    [ShowInInspector]
    protected float groundDistance = 0.1f;
    [SerializeField]
    private LayerMask groundMask;

    [SyncVar]
    Vector3 velocity;
    private void Start()
    {
        if (isLocalPlayer)
        {
            cam.enabled = true;
            cam.GetComponent<AudioListener>().enabled = true;
        }
    }
    [ClientCallback]
    public void Update()
    {
        if (!hasAuthority) return;
        Movement();
        ray = new Ray(cam.transform.position, cam.transform.forward);
        
        bool isItem = (Physics.Raycast(ray, out RaycastHit raycastHit, pickupRange, pickUpLayer));
        Debug.DrawRay(cam.transform.position, cam.transform.forward * pickupRange, Color.yellow);

        if(isItem && heldObj == null)
        {
            if(isLockedItem == false)
            {
                GameObject item = raycastHit.transform.gameObject;
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
                characterCanvas.chairInfo.SetActive(true);
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
            characterCanvas.chairInfo.SetActive(false);
            return;
        }
    }
        



#region Movement
    [Client]
    void Movement()
    {
        if (!hasAuthority) return;

        if (CheckGround() && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        //Get parms
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        bool jump = Input.GetButtonDown("Jump");
        if (x != 0 || z != 0)
            animator.SetBool("move", true);
        else
            animator.SetBool("move", false);

        //Check if parms are set (Player moves)
        if (x != 0 || z != 0 || jump)
        {
            Vector3 move = (transform.right * x) + (transform.forward * z);
            controller.Move(move * speed * Time.deltaTime);
        }
        if (jump && CheckGround())
        {
            animator.SetTrigger("jump");
            velocity.y += Mathf.Sqrt(jumpHeight * -2.0f * Physics.gravity.y);
        }

        velocity.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    [Client]
    public bool CheckGround()
    {
        return Physics.Raycast(this.transform.position, Vector3.down, groundDistance + 0.1f);
    }


#endregion

#region Pickup
    [Command]
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

            Transform leftPos = heldObj.transform.Find("HandPointL");
            Transform rightPos = heldObj.transform.Find("HandPointR");

            //IK Stuff
            leftHandPick = leftPos;
            rightHandPick = rightPos;
            ikActive = true;
            RpcPickupObj(pickObj);
        }
    }
    [ClientRpc]
    void RpcPickupObj(GameObject pickObj)
    {
        //pickObj.transform.parent = holdPos.transform;
        pickObj.transform.localPosition = new Vector3(0, 0, 0);
        pickObj.transform.localRotation = Quaternion.Euler(0,0,0);
        leftHandPick = heldObj.transform.Find("HandPointL");
        rightHandPick = heldObj.transform.Find("HandPointR");
        ikActive = true;
    }

    [Command]
    void DropObj()
    {
        heldObj.GetComponent<Rigidbody>().isKinematic = false;
        heldObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        heldObj.transform.parent = null;
        heldObj.GetComponent<Rigidbody>().AddForce(cam.transform.forward * heldObj.GetComponent<chair>().dropForce());
        heldObj = null;
        //IK Stuff
        leftHandPick = null;
        rightHandPick = null;
        ikActive = false;
        RpcDropObj();
    }
    [ClientRpc]
    void RpcDropObj()
    {
        leftHandPick = null;
        rightHandPick = null;
        ikActive = false;
    }
#endregion

#region IK

    //a callback for calculating IK
    private void OnAnimatorIK()
    {
        //if the IK is active, set the position and rotation directly to the goal.
        netAnimator.animator.SetLookAtWeight(0.7f, 0f, 1f);
        netAnimator.animator.SetLookAtPosition(ray.GetPoint(25));
        if(ikActive) {
        // Set the look target position, if one has been assigned
        netAnimator.animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);

        netAnimator.animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        netAnimator.animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
        netAnimator.animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
        netAnimator.animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandPick.position);
        netAnimator.animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandPick.position);
        netAnimator.animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandPick.rotation);
        netAnimator.animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandPick.rotation);

        }

        //if the IK is not active, set the position and rotation of the hand and head back to the original position
        else {
            netAnimator.animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
            netAnimator.animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
            netAnimator.animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
            netAnimator.animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
            //animator.SetLookAtWeight(0);
        }
    }


#endregion

}

