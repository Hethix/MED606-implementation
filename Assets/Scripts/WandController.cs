using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WandController : MonoBehaviour
{
    private Valve.VR.EVRButtonId menuButton = Valve.VR.EVRButtonId.k_EButton_ApplicationMenu;
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private Valve.VR.EVRButtonId padButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;

    public SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    private SteamVR_TrackedObject trackedObj;

    HashSet<InteractableItem> objectsHoveringOver = new HashSet<InteractableItem>();

    private InteractableItem closestItem;
    private InteractableItem interactingItem;

    private GameObject prefab;

    private GameObject menu;
    public GameObject[] menuItems;
    private bool isMenuActive = false;
    private bool changeMenu = false;

    // Use this for initialization
    void Start()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        menu = GameObject.FindGameObjectWithTag("Menu");
    }

    // Update is called once per frame
    void Update()
    {
        if (controller == null)
        {
            Debug.Log("Controller not initialized");
            return;
        }

        else
        {
            if (controller.GetPressDown(triggerButton))
            {
                float minDistance = float.MaxValue;

                float distance;
                foreach (InteractableItem item in objectsHoveringOver) //Goes through all the objects and detects which is closest to the controller
                {
                    distance = (item.transform.position - transform.position).sqrMagnitude;

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestItem = item;
                    }
                }

                if (closestItem != null && closestItem.isMenuItem) //If the item pressed is something on the menu, then instantiate an object corresponding to the one on the menu
                {
                    prefab = (GameObject)Instantiate(closestItem.worldPrefab, transform.position, Quaternion.Euler(0,0,0)); //Spawn it at the controllers pos and with 0 rotation (facing upwards)
                    interactingItem = prefab.GetComponent<InteractableItem>(); //Is only used for letting an object go again in this case
                    closestItem = null;
                    interactingItem.BeginInteraction(this);
                }
                else if (closestItem != null && closestItem.isArrow) //If an arrow is pressed. simply tell the arrow and make it change menu page
                {
                    closestItem.GetComponent<Arrows>().pressed = true;
                    interactingItem = null;
                    closestItem = null;
                }
                else
                {
                    interactingItem = closestItem;
                    closestItem = null;
                }

                if (interactingItem) //Starts interacting with the chosen item
                {
                    if (interactingItem.IsInteracting()) //this statement is used in order to grap an item in the other hand
                    {
                        interactingItem.EndInteraction(this, false);
                    }

                    interactingItem.BeginInteraction(this);
                }
            }

            if (controller.GetPressUp(triggerButton) && interactingItem != null)
            {
                interactingItem.EndInteraction(this, false); //Stops interaction with the item held
                interactingItem = null;
            }
        }
        if(interactingItem != null)
        {
            if (controller.GetPressDown(menuButton) && !interactingItem.isMenuItem && !interactingItem.isArrow)
            {
                objectsHoveringOver.Clear();
                closestItem = null;
                interactingItem.EndInteraction(this, true);
                interactingItem = null;
            }
        }

        //if the controller has the menu attached then we can disable/enable the menu with that controller by pressing the Touch Pad
        if (controller.GetPressDown(padButton) && changeMenu == false && controller.index == 2)
        {
            isMenuActive = !isMenuActive;
            changeMenu = true;
            Menu(isMenuActive);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        InteractableItem collidedItem = collider.GetComponent<InteractableItem>();
        if (collidedItem)
        {
            objectsHoveringOver.Add(collidedItem);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        InteractableItem collidedItem = collider.GetComponent<InteractableItem>();
        if (collidedItem)
        {
            objectsHoveringOver.Remove(collidedItem);
        }
    }

    //Method used to disable the menu object and the arrows.
    private void Menu(bool isActive)
    {
        if (isActive)
        {
            menu.SetActive(true);
            for(int i = 0; i < menuItems.Length; i++)
            {
                menuItems[i].SetActive(true);
            }
            changeMenu = false;
        }
        else
        {
            menu.GetComponent<MenuController>().DisableMenu();
            for (int i = 0; i < menuItems.Length; i++)
            {
                menuItems[i].SetActive(false);
            }

            menu.SetActive(false);
            changeMenu = false;

        }
    }
}