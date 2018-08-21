using UnityEngine;

public class ItemInfo : MonoBehaviour
{
    public string ItemName;

    private Item itemData;
    private Inventory inventory;

    //--------------------------------------------------------------------------------------------------------------------
    // Public Methods
    //--------------------------------------------------------------------------------------------------------------------

    public Item GetItemData()
    {
        return itemData;
    }

    //--------------------------------------------------------------------------------------------------------------------
    // Private Methods
    //--------------------------------------------------------------------------------------------------------------------

    private void Start()
    {
        itemData = new Item();
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
        SetItemData();
    }

    private void SetItemData()
    {
        switch (ItemName)
        {
            case "Spellbook":
                itemData = Inventory.GetItemById(0, inventory.database);
                break;
            case "Warding":
                itemData = Inventory.GetItemById(1, inventory.database);
                break;
            case "Radiance":
                itemData = Inventory.GetItemById(2, inventory.database);
                break;
            case "Conscious":
                itemData = Inventory.GetItemById(3, inventory.database);
                break;
        }
    }
}
