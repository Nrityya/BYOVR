using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Interactable : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    GameObject heldPlayer;
    Camera playerCamera;
    public GameObject temp;
    Outline outlineCompenent;
    public float grabForce = 15.0f;
    public float grabOffset = 1.0f;
    Rigidbody rigidbodyComponent;
    bool isGrabbed = false;
    bool isHovered = false;
    public float throwForceMax = 20f;
    public float throwForceMin = 5f;
    public float throwMaxChargeTime = 2f;
    public void OnPointerClick(PointerEventData eventData)
    {

    }

    public void Grabbed(GameObject player)
    {
        player.SendMessage("Grabbed", gameObject, SendMessageOptions.DontRequireReceiver);
        isGrabbed = true;
        rigidbodyComponent.useGravity = false;
        rigidbodyComponent.linearVelocity = Vector3.zero;
        heldPlayer = player;
        playerCamera = player.GetComponentInChildren<Camera>();

    }
    public void Released()
    {
        heldPlayer.SendMessage("Released", gameObject, SendMessageOptions.DontRequireReceiver);
        rigidbodyComponent.useGravity = true;
        rigidbodyComponent.constraints = RigidbodyConstraints.None;
        isGrabbed = false;
        heldPlayer = null;
        playerCamera = null;

    }
    
    IEnumerator ThrowCoroutine()
    {
        float throwForce = Time.time;
        //start ui animation coroutine here

        yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.F));
        throwForce = Mathf.Min((Time.time - throwForce)*(throwForceMax - throwForceMin)/throwMaxChargeTime + throwForceMin, throwForceMax);
        // Debug.Log("Throwing " + gameObject.name + " with force: " + throwForce);
        rigidbodyComponent.AddForce(playerCamera.transform.forward * throwForce, ForceMode.Impulse);
        Released();
    }
    public void UpdateGrab()
    {
        if (heldPlayer != null)
        {
            var target = playerCamera.transform.position + playerCamera.transform.forward * grabOffset;
            var offset = target - transform.position;
            rigidbodyComponent.linearVelocity = offset * grabForce;

            // make sure the keys are right later
            if (Input.GetKey(KeyCode.R))
            {
                transform.Rotate(playerCamera.transform.forward * -Input.GetAxis("Mouse X") * 5f, Space.World);
                transform.Rotate(playerCamera.transform.right * Input.GetAxis("Mouse Y") * 5f, Space.World);
            }
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        outlineCompenent.enabled = true;
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        outlineCompenent.enabled = false;
        isHovered = false;

    }


// CONTROL METHODS
    void Start()
    {
        outlineCompenent = gameObject.GetComponent<Outline>();
        outlineCompenent.enabled = false;
        outlineCompenent.OutlineWidth = 10f;
        outlineCompenent.OutlineColor = Color.cyan;
        rigidbodyComponent = gameObject.GetComponent<Rigidbody>();
        rigidbodyComponent.interpolation = RigidbodyInterpolation.Interpolate;
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isGrabbed)
            {
                Released();
            }
            else if (isHovered)
            {
                Grabbed(temp);
            }
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(ThrowCoroutine());
        }
    }
    

    void FixedUpdate()
    {
        if (isGrabbed)
        {
            UpdateGrab();
        }
    }

}
