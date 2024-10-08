using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{

    private GameDataController gameDataController;

    public Button[] slot;
    public Image[] itemIcon;

    public List<GameObject> inventoryItens;
    public List<GameObject> loadedItems;

    void Start()
    {
        gameDataController = FindFirstObjectByType(typeof(GameDataController)) as GameDataController;
    }

    void Update()
    {
        
    }

    public void LoadInventory()
    {
        ClearLoadedItems();

        foreach (Button b in slot)
        {
            b.interactable = false;
        }

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

    public void ClearLoadedItems()
    {
        foreach (GameObject li in loadedItems)
        {
            Destroy(li);
        }

        loadedItems.Clear();
    }
}
