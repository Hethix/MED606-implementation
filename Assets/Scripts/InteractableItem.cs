using UnityEngine;
using System.Collections;

public class InteractableItem : MonoBehaviour
{
    private Rigidbody rigidbody;

    private WandController attachedWand;
    private Transform interactionPoint;
    private bool currentlyInteracting;
    private Coroutine stopRoutine;
    private Coroutine newItemRoutine;

    private float velocityFactor = 20000f;
    private Vector3 posDelta;

    private float rotationFactor = 400f;
    private Quaternion rotationDelta;
    private float angle;
    private Vector3 axis;

    //Variables for lerping size of instantiated objects
    private Vector3 startScale;
    private float distance;
    private GameObject menuPosition;

    public bool isMenuItem;
    public bool isArrow;
    public bool isNewItem;
    public GameObject worldPrefab;

    // Use this for initialization
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        velocityFactor /= rigidbody.mass;
        rotationFactor /= rigidbody.mass;
        startScale = transform.localScale;
        menuPosition = GameObject.FindGameObjectWithTag("Menu");
        interactionPoint = new GameObject().transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (attachedWand && currentlyInteracting)
        {
            posDelta = attachedWand.transform.position - interactionPoint.position;
            this.rigidbody.velocity = posDelta * velocityFactor * Time.fixedDeltaTime;

            rotationDelta = attachedWand.transform.rotation * Quaternion.Inverse(interactionPoint.rotation);
            rotationDelta.ToAngleAxis(out angle, out axis);

            if (angle > 180)
            {
                angle -= 360;
            }

            //Apply rotation force
            this.rigidbody.angularVelocity = (Time.fixedDeltaTime * angle * axis) * rotationFactor;
            //Extra adjustment to keep the object oriented in place (standing up)
            this.transform.localRotation = Quaternion.Euler(0, this.transform.rotation.eulerAngles.y, 0);

            //Size adjustment based on distance to the menu
            if (menuPosition.activeInHierarchy) //Do so if the menu is active
            {
                distance = Vector3.Distance(attachedWand.transform.position, menuPosition.transform.position);
                this.transform.localScale = Vector3.Lerp(startScale, new Vector3(1, 1, 1), Mathf.Clamp((distance * 2), 0.0f, 1));
            }
        }
    }

    public void BeginInteraction(WandController wand)
    {
        if (isNewItem)
        {
            newItemRoutine = StartCoroutine(InstantiateFix(wand));
        }
        else
        {
            attachedWand = wand;
            interactionPoint.position = attachedWand.transform.position;
            interactionPoint.rotation = attachedWand.transform.rotation;

            interactionPoint.SetParent(transform, true);
            currentlyInteracting = true;

            StopCoroutine(stopRoutine);
            this.rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
        
    }

    public void EndInteraction(WandController wand, bool detectDestroy)
    {
        if (wand == attachedWand)
        {
            attachedWand = null;
            currentlyInteracting = false;
            stopRoutine = StartCoroutine(LockPosition(2.0f));
        }
        isNewItem = false;
        this.transform.localScale = new Vector3(1, 1, 1);

        if (detectDestroy)
        {
            Destroy();
        }
    }

    public bool IsInteracting()
    {
        return currentlyInteracting;
    }

    IEnumerator LockPosition(float waitTime)
    {
        this.rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ |
                                     RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        yield return new WaitForSeconds(waitTime);
        this.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    IEnumerator InstantiateFix(WandController wand) //This is needed since BeginInteraction() is ran before Start() is
    {
        yield return new WaitForSeconds(0.001f);
        attachedWand = wand;
        interactionPoint.position = attachedWand.transform.position;
        interactionPoint.rotation = attachedWand.transform.rotation;

        interactionPoint.SetParent(transform, true);
        currentlyInteracting = true;
        this.rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    public void Destroy()
    {
        GameObject.Destroy(gameObject);
    }
}