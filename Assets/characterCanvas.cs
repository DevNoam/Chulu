using UnityEngine;
using TMPro;
using UnityEngine.UI;
[RequireComponent(typeof(Manager))]
public class characterCanvas : MonoBehaviour
{
    public TMP_Text healthUI; 
    
    [Header("Pickup notice components")]
    public GameObject pickupNotice;
    public TMP_Text itemName;
    public Slider itemHealth;
}
