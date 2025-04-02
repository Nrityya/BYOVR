using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IDynamicSelectable
{
    public float grabForce = 15.0f;
    public float grabOffset = 1.0f;
    public float throwForceMax = 20f;
    public float throwForceMin = 5f;
    public float throwMaxChargeTime = 2f;

    public bool throwable = true;
    public bool grabbable = true;
    public bool rotatable = true;

    bool isGrabbed = false;
    bool isHovered = false;

    bool RigidbodyConstrained => rigidbodyComponent.constraints != RigidbodyConstraints.None;

    Camera playerCamera;
    Outline outlineComponent;
    Rigidbody rigidbodyComponent;

    void Start()
    {
        outlineComponent = gameObject.GetComponent<Outline>();
        outlineComponent.enabled = false;
        outlineComponent.OutlineWidth = 10f;
        outlineComponent.OutlineColor = Color.cyan;
        outlineComponent.OutlineMode = Outline.Mode.OutlineVisible;

        rigidbodyComponent = gameObject.GetComponent<Rigidbody>();
        rigidbodyComponent.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Update()
    {
        // Need to check every frame in case the camera is moving quickly
        if (isGrabbed && !isHovered && ShouldBeSelected(null))
        {
            OnPointerClick(null);
        }
    }


    void FixedUpdate()
    {
        if (isGrabbed)
        {
            UpdateGrab();
        }
    }

    public bool ShouldBeSelected(PointerEventData pointerEventData)
    {
        if (IsGrabButtonDown()) return true;
        if (isGrabbed) return IsThrowButtonDown() || IsThrowButtonUp();
        return false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // We can assume one of the desired buttons is pressed
        if (isGrabbed)
        {
            if (IsGrabButtonDown())
            {
                Released();
            }
            else if (throwable)
            {
                StartCoroutine(ThrowCoroutine());
            }
        }
        else if (grabbable)
        {
            Grabbed();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        outlineComponent.enabled = true;
        playerCamera = eventData.enterEventCamera ?? eventData.pressEventCamera;
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        outlineComponent.enabled = false;
        isHovered = false;
    }

    public void Grabbed()
    {
        isGrabbed = true;
        rigidbodyComponent.useGravity = false;
        rigidbodyComponent.linearVelocity = Vector3.zero;
        rigidbodyComponent.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void Released()
    {
        rigidbodyComponent.useGravity = true;
        rigidbodyComponent.constraints = RigidbodyConstraints.None;
        isGrabbed = false;
    }

    public void UpdateGrab()
    {
        var target = playerCamera.transform.position + playerCamera.transform.forward * grabOffset;
        var offset = target - transform.position;
        rigidbodyComponent.linearVelocity = offset * grabForce;

        if (IsRotateButtonPressed() && rotatable)
        {
            if (RigidbodyConstrained) rigidbodyComponent.constraints = RigidbodyConstraints.None;

            // TODO: Joystick
            transform.Rotate(playerCamera.transform.forward * -Input.GetAxis("Mouse X") * 5f, Space.World);
            transform.Rotate(playerCamera.transform.right * Input.GetAxis("Mouse Y") * 5f, Space.World);
        }
        else if (!RigidbodyConstrained)
        {
            rigidbodyComponent.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    private IEnumerator ThrowCoroutine()
    {
        float throwForce = Time.time;
        // TODO: start ui animation coroutine here

        yield return new WaitUntil(IsThrowButtonUp);
        throwForce = Mathf.Min((Time.time - throwForce) * (throwForceMax - throwForceMin) / throwMaxChargeTime + throwForceMin, throwForceMax);
        // Debug.Log("Throwing " + gameObject.name + " with force: " + throwForce);
        rigidbodyComponent.AddForce(playerCamera.transform.forward * throwForce, ForceMode.Impulse);
        Released();
    }

    private bool IsGrabButtonDown()
    {
        return Input.GetKeyDown(KeyCode.E) || ControllerInputHelper.IsXButtonDown();
    }

    private bool IsThrowButtonDown()
    {
        return Input.GetKeyDown(KeyCode.F) || ControllerInputHelper.IsYButtonDown();
    }

    private bool IsThrowButtonUp()
    {
        return Input.GetKeyUp(KeyCode.F) || ControllerInputHelper.IsYButtonUp();
    }

    private bool IsRotateButtonPressed()
    {
        return Input.GetKey(KeyCode.R) || ControllerInputHelper.IsOKPressed();
    }
}

