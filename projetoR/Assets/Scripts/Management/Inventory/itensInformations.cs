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

    public void loadItemInfo()
    {
        if (slotObject != null) 
        {
            Item itemInfo = slotObject.GetComponent<Item>();

            int itemId = itemInfo.idItem;

            itemName.text = gameDataController.itemName[itemId];
            itemImage.sprite = gameDataController.itemImage[itemId];
            itemImage.color = new Color(1, 1, 1, 1);
            itemDescription.text = gameDataController.itemDescription[itemId];
        }
    }

    public void unloadItemInfo()
    {
            itemName.text = null;
            itemImage.sprite = null;
            itemImage.color = new Color(1, 1, 1, 0);
            itemDescription.text = null;
    }
}
