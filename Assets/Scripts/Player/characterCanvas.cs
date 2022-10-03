using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class characterCanvas : MonoBehaviour
{
    public TMP_Text healthUI; 
    
    [Header("Pickup notice components")]
    public GameObject pickupNotice;
    public TMP_Text itemName;
    public Slider itemHealth;

    void Start()
    {
        healthUI = GameObject.Find("healthNum").GetComponent<TMP_Text>();
        pickupNotice = GameObject.Find("PickupNotice");
        itemName = GameObject.Find("ChairName").GetComponent<TMP_Text>();
        itemHealth = GameObject.Find("chairHealth").GetComponent<Slider>();
        pickupNotice.SetActive(false);
    }
}
