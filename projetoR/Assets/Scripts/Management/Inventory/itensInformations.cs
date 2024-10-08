using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class itensInformations : MonoBehaviour
{

    private GameDataController gameDataController;

    public int slotID;
    public GameObject slotObject;

    [Header("Item Information")]
    public Image itemImage;
    public TMP_Text itemName;
    public TMP_Text itemDescription;

    void Start()
    {
        gameDataController = FindFirstObjectByType(typeof(GameDataController)) as GameDataController;
    }

    void Update()
    {
        
    }
}
