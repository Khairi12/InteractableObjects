using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    [HideInInspector] public List<Item> database;

    private List<GameObject> inventory;
    private List<GameObject> slots;
    private RectTransform canvas;
    private Transform parent;
    private bool inventoryOpen;

    //--------------------------------------------------------------------------------------------------------------------
    // Public Methods
    //--------------------------------------------------------------------------------------------------------------------

    public void AddItem(ref GameObject obj)
    {
        inventory.Add(obj);
        Destroy(obj);
    }
    
    public void RemoveItem(int id, Vector3 position, Vector3 direction)
    {
        // instantiate world object
        // remove item from inventory
        GameObject spawnItem = Instantiate(Resources.Load(GetItemById(id, database).Slug)) as GameObject;
        spawnItem.transform.position = position + (direction * 2);
    }

    //--------------------------------------------------------------------------------------------------------------------
    // Public Static Methods
    //--------------------------------------------------------------------------------------------------------------------

    public static Item GetItemById(int id, List<Item> itemlist)
    {
        foreach (Item i in itemlist)
            if (i.Id == id)
                return i;

        return new Item();
    }

    //--------------------------------------------------------------------------------------------------------------------
    // Private Methods
    //--------------------------------------------------------------------------------------------------------------------

    private void Start()
    {
        canvas = gameObject.GetComponent<RectTransform>();
        inventory = new List<GameObject>();
        slots = new List<GameObject>();
        database = new List<Item>();
        parent = transform.parent;
        inventoryOpen = false;

        CreateDatabase();
        CreateSlots(5);
    }

    private void CreateDatabase()
    {
        database.Add(new Item(0, true, "Spellbook", "A spellbook missing pages", "Spellbook"));
        database.Add(new Item(1, true, "Warding", "Ward spell description", "Spell_Ward"));
        database.Add(new Item(2, true, "Radiance", "Radiant spell description", "Spell_Radiant"));
        database.Add(new Item(3, true, "Conscious", "Conscious spell description", "Item_3"));
    }

    private void CreateSlots(int slotCap)
    {
        float width = canvas.rect.width / 5;
        float height = canvas.rect.height / 2;

        for (int x = 0; x < slotCap; x++)
        {
            slots.Add(Instantiate(Resources.Load("Item_Slot", typeof(GameObject))) as GameObject);
            slots[x].transform.SetParent(transform, false);
            slots[x].GetComponent<RectTransform>().anchoredPosition = new Vector3(
                (slots[x].GetComponent<RectTransform>().rect.width / 2 ) + width * x, height, x * 30);
        }
    }

    private void ToggleInventoryUI()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (inventoryOpen)
            {
                canvas.parent = parent;
                inventoryOpen = false;
                foreach (GameObject x in slots)
                    x.GetComponent<Image>().enabled = false;
            }
            else
            {
                canvas.parent = null;
                inventoryOpen = true;
                foreach (GameObject x in slots)
                    x.GetComponent<Image>().enabled = true;
            }
        }
    }

    private void Update()
    {
        ToggleInventoryUI();
    }
}
