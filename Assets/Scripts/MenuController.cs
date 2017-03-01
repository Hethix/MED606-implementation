using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour {

    public GameObject[] prefabArr;
    public short pageNumber = 0;
    private short currentNum = 0;
    private short maxPageNum = 7;
    private Texture[] pageTexture;
    public MeshRenderer[] pageDisplay;

    public GameObject[] displayArr;
    public Transform[] displayPos;
    private Transform rotate;

    public Transform dumpster;
    



    // Use this for initialization
    void Start () {
        if(prefabArr.Length == 0)
        {
            prefabArr = Resources.LoadAll<GameObject>("Prefabs");
            pageTexture = Resources.LoadAll<Texture>("Texture");
        }

        SetMenu(prefabArr, currentNum);

    }
	
	// Update is called once per frame
	void Update () {
        //If the page goes above or below the amount of pages we have then set it.
        if(pageNumber > maxPageNum)
        {
            pageNumber = 0;
        }
        else if(pageNumber < 0)
        {
            pageNumber = maxPageNum;
        }

        //Change the page of the menu if it has changed since the last known page.
	    if(currentNum != pageNumber)
        {
            currentNum = pageNumber;
            SetMenu(prefabArr, currentNum);
        }

        //Move the objects on the menu onto the menu and keep em there. (Could be done with making them parent for effiency or not??)
        for (int i = 0; i < transform.childCount; i++)
        {
            displayArr[i + (6 * currentNum)].transform.SetParent(displayPos[i]);
            displayArr[i + (6 * currentNum)].transform.localPosition = new Vector3(0.11f, 0.015f, -0.07f);
            //displayArr[i + (6 * currentNum)].transform.position = displayPos[i].position + new Vector3(0.11f, 0f, -0.055f); //offset compensation ???Not sure why they are not placed correctly???
            displayArr[i + (6 * currentNum)].transform.rotation = displayPos[i].rotation * Quaternion.Euler(Vector3.right * 15f) * Quaternion.Euler(Vector3.up * 180f);
        }


    }


    void SetMenu(GameObject[] arr, short num)
    {
        //Remove all the displayed objects from the menu
        for(int i = 0; i < displayArr.Length; i++)
        {
            displayArr[i].transform.position = dumpster.position;
            displayArr[i].transform.SetParent(null);
        }

        //Choose which objects are corrently on the menu, and place them correctly on the menu
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform currentChild;
            currentChild = this.gameObject.transform.GetChild(i);
            currentChild.GetComponent<InteractableItem>().worldPrefab = prefabArr[i + (6 * num)];
        }

        switch(num)
        {
            case 0:
                pageDisplay[0].material.mainTexture = pageTexture[7];
                pageDisplay[1].material.mainTexture = pageTexture[0];
                pageDisplay[2].material.mainTexture = pageTexture[1];
                break;
            case 1:
                pageDisplay[0].material.mainTexture = pageTexture[0];
                pageDisplay[1].material.mainTexture = pageTexture[1];
                pageDisplay[2].material.mainTexture = pageTexture[2];
                break;
            case 2:
                pageDisplay[0].material.mainTexture = pageTexture[1];
                pageDisplay[1].material.mainTexture = pageTexture[2];
                pageDisplay[2].material.mainTexture = pageTexture[3];
                break;
            case 3:
                pageDisplay[0].material.mainTexture = pageTexture[2];
                pageDisplay[1].material.mainTexture = pageTexture[3];
                pageDisplay[2].material.mainTexture = pageTexture[4];
                break;
            case 4:
                pageDisplay[0].material.mainTexture = pageTexture[3];
                pageDisplay[1].material.mainTexture = pageTexture[4];
                pageDisplay[2].material.mainTexture = pageTexture[5];
                break;
            case 5:
                pageDisplay[0].material.mainTexture = pageTexture[4];
                pageDisplay[1].material.mainTexture = pageTexture[5];
                pageDisplay[2].material.mainTexture = pageTexture[6];
                break;
            case 6:
                pageDisplay[0].material.mainTexture = pageTexture[5];
                pageDisplay[1].material.mainTexture = pageTexture[6];
                pageDisplay[2].material.mainTexture = pageTexture[7];
                break;
            case 7:
                pageDisplay[0].material.mainTexture = pageTexture[6];
                pageDisplay[1].material.mainTexture = pageTexture[7];
                pageDisplay[2].material.mainTexture = pageTexture[0];
                break;
        }
    }

    //Used to clear the menu just before it is disabled
    public void DisableMenu()
    {
        for (int i = 0; i < displayArr.Length; i++)
        {
            displayArr[i].transform.position = dumpster.position;
        }
    }
}
