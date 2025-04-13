
using System.Collections;
using Fusion.Addons.Physics;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NetworkRigidbody3D))]
[RequireComponent(typeof(ChargeBar))]
public class GrabInteractable : Interactable
{
    public float grabForce = 15.0f;
    public float grabOffset = 1.0f;
    public float throwForceMax = 20f;
    public float throwForceMin = 5f;
    public float throwMaxChargeTime = 2f;
    public bool rotatable = true;
    public bool throwable = true;

    Rigidbody rigidbodyComponent;
    ChargeBar chargeBar;

    int grabbedFrameNumber = 0;
    int droppedFrameNumber = 0;

    public void Start()
    {
        SetupInteractable();

        rigidbodyComponent = GetComponent<Rigidbody>();
        rigidbodyComponent.interpolation = RigidbodyInterpolation.Interpolate;

        chargeBar = GetComponent<ChargeBar>();
    }

    protected override void OnTakeControl(PlayerNetworkController playerNetworkController)
    {
        StartGrab();
    }

    public void Update()
    {
        if (!IsControlled || !controllingPlayer.IsLocal) return;

        // Wait for state authority
        if (rigidbodyComponent.isKinematic) return;

        var targetPosition = playerCamera.transform.position + playerCamera.transform.forward * grabOffset;

        if (IsGrabButtonDown() && Time.frameCount > grabbedFrameNumber)
        {
            EndGrab();
        }
        else if (IsThrowButtonDown() && throwable)
        {
            StartCoroutine(ThrowCoroutine());
        }

        if (IsRotateButtonPressed() && rotatable)
        {
            controllingPlayer.movementEnabled = false;

            var axisX = Input.GetAxis("Mouse X") + Input.GetAxis("Horizontal");
            var axisY = Input.GetAxis("Mouse Y") + Input.GetAxis("Vertical");

            transform.Rotate(playerCamera.transform.forward * -axisX * 5f, Space.World);
            transform.Rotate(playerCamera.transform.right * axisY * 5f, Space.World);
        }
        else if (!controllingPlayer.movementEnabled)
        {
            controllingPlayer.movementEnabled = true;
        }

        var offset = targetPosition - transform.position;
        rigidbodyComponent.linearVelocity = offset * grabForce;
    }

    public override bool ShouldBeSelected(PointerEventData pointerEventData)
    {
        // Don't allow grabbing the same frame that it was dropped
        return !IsControlled && IsGrabButtonDown() && droppedFrameNumber < Time.frameCount;
    }

    public void StartGrab()
    {
        rigidbodyComponent.useGravity = false;
        rigidbodyComponent.linearVelocity = Vector3.zero;
        rigidbodyComponent.angularDamping = 15f;
        grabbedFrameNumber = Time.frameCount;
    }

    public void EndGrab()
    {
        rigidbodyComponent.useGravity = true;
        rigidbodyComponent.constraints = RigidbodyConstraints.None;
        rigidbodyComponent.angularDamping = 0.05f;
        droppedFrameNumber = Time.frameCount;
        RelieveControl();
    }

    private IEnumerator ThrowCoroutine()
    {
        chargeBar.Setup(playerCamera);

        float start = Time.realtimeSinceStartup;
        float chargeTime = 0;
        while (!IsThrowButtonUp())
        {
            chargeTime = Mathf.Min(Time.realtimeSinceStartup - start, throwMaxChargeTime);
            chargeBar.UpdateCharge(chargeTime / throwMaxChargeTime);
            yield return null;
        }
        float throwForce = Mathf.Min(chargeTime * (throwForceMax - throwForceMin) / throwMaxChargeTime + throwForceMin, throwForceMax);
        rigidbodyComponent.AddForce(playerCamera.transform.forward * throwForce, ForceMode.Impulse);
        chargeBar.Cleanup();
        EndGrab();
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