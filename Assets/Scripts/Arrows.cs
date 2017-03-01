using UnityEngine;
using System.Collections;

public class Arrows : MonoBehaviour {
    public bool pressed = false;
    public bool leftArrow;

    public MenuController menu;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if (pressed)
        {
            if (leftArrow) //swap menu left is left is pressed
            {
                menu.pageNumber--;
                pressed = false;
            }
            else //Swap menu right if right is pressed
            {
                menu.pageNumber++;
                pressed = false;
            }
        }
	}
}
