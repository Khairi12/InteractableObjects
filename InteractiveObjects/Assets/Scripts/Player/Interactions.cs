using UnityEngine;
using UnityEngine.UI;

public class Interactions : MonoBehaviour
{
    private Inventory inventory;
    private Transform camTransform;
    private Camera cam;
    private Image image;

    private void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
        cam = GetComponentInChildren<Camera>();
        camTransform = cam.GetComponent<Transform>();
        image = GetComponentInChildren<Image>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void ItemInteraction(RaycastHit hit)
    {
        if (hit.collider.GetComponent<ItemInfo>().GetItemData().Name == "Spellbook")
        {
            GameObject gameObject = hit.transform.gameObject;
            inventory.AddItem(ref gameObject);
        }
    }

    private void ActionInteraction(RaycastHit hit)
    {
        if (hit.collider.name == "Door Left" ||
            hit.collider.name == "Door Right")
        {
            hit.transform.gameObject.GetComponent<DoorControl>().PlayerOpenControl();
        }
        else if (hit.collider.transform.parent.name == "Drawer")
        {
            hit.transform.gameObject.GetComponentInParent<SlideInteraction>().Operate();
        }
        else if (hit.collider.name == "Locker Door")
        {
            hit.transform.gameObject.GetComponentInChildren<RotateInteraction>().Operate();
        }
        else if (hit.collider.name == "Coffin Door")
        {
            hit.transform.gameObject.GetComponent<RotateInteraction>().Operate();
        }
        else if (hit.collider.name == "Chest Door")
        {
            hit.transform.gameObject.GetComponent<RotateInteraction>().Operate();
        }
        else if (hit.collider.name == "Computer")
        {
            hit.transform.gameObject.GetComponent<MonitorControl>().Operate();
        }
    }

    private void UpdateCursor()
    {
        RaycastHit hit;

        if (Physics.Raycast(camTransform.position, camTransform.TransformDirection(Vector3.forward), out hit))
        {
            if (hit.transform.tag == "Item")
            {
                image.color = Color.green;
            }
            else if (hit.transform.tag == "Monster")
            {
                image.color = Color.red;
            }
            else if (hit.transform.tag == "Action")
            {
                image.color = Color.blue;
            }
            else
            {
                image.color = Color.white;
            }
        }
        else
        {
            if (image.enabled)
                image.enabled = false;
        }
    }

    private void UpdateInteraction()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            RaycastHit hit;

            if (Physics.Raycast(camTransform.position, camTransform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
            {
                if (hit.collider.tag == "Item")
                {
                    ItemInteraction(hit);
                }
                else if (hit.collider.tag == "Action")
                {
                    ActionInteraction(hit);
                }
            }
        }
    }

    private void Update()
    {
        UpdateCursor();
        UpdateInteraction();
    }
}
