using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int idItem;
    public string itemDescription;

    public void Equip()
    {
        print("o item" + idItem + " foi equipado");
    }
}
