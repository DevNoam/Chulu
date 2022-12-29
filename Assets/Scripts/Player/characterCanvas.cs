using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;
public class characterCanvas : NetworkBehaviour
{
    public TMP_Text healthUI; 
    
    [Header("Pickup notice components")]
    public GameObject pickupNotice;
    public TMP_Text itemName;
    public Slider itemHealth;
    public GameObject chairInfo;

    void Start()
    {
        if(!hasAuthority) { this.enabled = false; return;};
        healthUI = GameObject.Find("healthNum").GetComponent<TMP_Text>();
        pickupNotice = GameObject.Find("PickupNotice");
        itemName = GameObject.Find("ChairName").GetComponent<TMP_Text>();
        itemHealth = GameObject.Find("chairHealth").GetComponent<Slider>();
        chairInfo = GameObject.Find("ChairInfo");
        pickupNotice.SetActive(false);
        chairInfo.SetActive(false);
    }
}
