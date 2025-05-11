using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour
{

    private GameDataController gameDataController;

    [Header("Equipment Notchs")]
    public Button[] notch;
    public Image[] notchImages;

    [Header("Inventory Slots")]
    public Button[] slot;
    public Image[] itemIcon;

    public List<GameObject> inventoryItens;
    public List<GameObject> loadedItems;
    public int[] equippedItemIDs = new int[3];

    void Start()
    {
        gameDataController = FindFirstObjectByType(typeof(GameDataController)) as GameDataController;

        for (int i = 0; i < equippedItemIDs.Length; i++)
        {
            equippedItemIDs[i] = -1; // -1 == vazio;
        }

        for (int i = 0; i < notch.Length; i++)
        {
            int index = i;
            notch[i].onClick.AddListener(() => UnequipItem(index));
        }

        for (int i = 0; i < slot.Length; i++)
        {
            int index = i;
            slot[i].onClick.AddListener(() => EquipItem(index));
        }

        RestoreEquippedItems();
    }

    void Update()
    {
        
    }

    public void LoadInventory()
    {
        ClearLoadedItems();

        int s = 0;

        foreach(GameObject i in inventoryItens)
        {
            GameObject temporaryInstance = Instantiate(i);

            Item itemInfo = temporaryInstance.GetComponent<Item>();

            loadedItems.Add(temporaryInstance);

            slot[s].GetComponent<Slots>().slotObject = temporaryInstance;
            slot[s].interactable = true;

            itemIcon[s].sprite = gameDataController.itemIcon[itemInfo.idItem];
            itemIcon[s].gameObject.SetActive(true);

            s++;
        }
    }

    public void EquipItem(int slotIndex)
    {
        GameObject itemToEquip = slot[slotIndex].GetComponent<Slots>().slotObject;

        if (itemToEquip == null) return;

        Item itemInfo = itemToEquip.GetComponent<Item>();

        for (int i = 0; i < equippedItemIDs.Length; i++)
        {
            if (equippedItemIDs[i] == itemInfo.idItem)
            {
                Debug.Log("Este item já está equipado!");
                return;
            }
        }

        for (int i = 0; i < notch.Length; i++)
        {
            if (equippedItemIDs[i] == -1)
            {
                equippedItemIDs[i] = itemInfo.idItem;
                notchImages[i].sprite = gameDataController.itemIcon[itemInfo.idItem];
                notchImages[i].gameObject.SetActive(true);
                Debug.Log($"Item {itemInfo.idItem} equipado no notch {i}");
                return;
            }
        }

        Debug.Log("Todos os notches estão ocupados!");
    }

    public void UnequipItem(int notchIndex)
    {
        if (equippedItemIDs[notchIndex] != -1)
        {
            Debug.Log($"Item {equippedItemIDs[notchIndex]} removido do notch {notchIndex}");
            equippedItemIDs[notchIndex] = -1;
            notchImages[notchIndex].gameObject.SetActive(false);
        }
    }

    public void RestoreEquippedItems()
    {
        for (int i = 0; i < notch.Length; i++)
        {
            if (equippedItemIDs[i] != -1)
            {
                GameObject prefab = inventoryItens.Find(item => item.GetComponent<Item>().idItem == equippedItemIDs[i]);

                if (prefab != null)
                {
                    GameObject tempInstance = Instantiate(prefab);
                    notchImages[i].sprite = gameDataController.itemIcon[equippedItemIDs[i]];
                    notchImages[i].gameObject.SetActive(true);
                    Debug.Log($"Item recriado no notch {i}");
                }
            }
            else
            {
                notchImages[i].gameObject.SetActive(false);
            }
        }
    }

    public void ClearLoadedItems()
    {
        foreach (GameObject li in loadedItems)
        {
            Destroy(li);
        }

        loadedItems.Clear();
    }
}
