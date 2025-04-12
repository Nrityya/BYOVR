
using System.Collections;
using Fusion;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NetworkTransform))]
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

    bool throwCoroutineRunning = false;
    float throwForceFromCoroutine = 0;

    int droppedFrameNumber = 0;

    void Start()
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

    public override ControlledObjectState GetNetworkState()
    {
        var state = new ControlledObjectState()
        {
            targetPosition = playerCamera.transform.position + playerCamera.transform.forward * grabOffset,
            releaseForce = -1 // Negative number means no throwing
        };

        if (throwForceFromCoroutine > 0)
        {
            state.releaseForce = throwForceFromCoroutine;
            throwForceFromCoroutine = 0;
        }
        else if (IsGrabButtonDown())
        {
            state.releaseForce = 0; // Just drop it
        }
        else if (IsThrowButtonDown() && throwable && !throwCoroutineRunning)
        {
            StartCoroutine(ThrowCoroutine());
        }

        if (IsRotateButtonPressed() && rotatable)
        {
            state.axisX = Input.GetAxis("Mouse X");
            state.axisY = Input.GetAxis("Mouse Y");
        }

        return state;
    }

    public override void UpdateFromNetworkState(NetworkInputData data)
    {
        var state = data.controlledObjectState;
        var offset = state.targetPosition - transform.position;
        rigidbodyComponent.linearVelocity = offset * grabForce;

        if (rotatable && (state.axisX != default || state.axisY != default))
        {
            var rightLook = Vector3.Cross(data.lookDirection, Vector3.up).normalized * -1;
            transform.Rotate(data.lookDirection * -state.axisX * 5f, Space.World);
            transform.Rotate(rightLook * state.axisY * 5f, Space.World);
        }

        if (state.releaseForce >= 0)
        {
            transform.position = state.targetPosition;
            EndGrab();
            if (state.releaseForce > 0)
            {
                rigidbodyComponent.AddForce(data.lookDirection * state.releaseForce, ForceMode.Impulse);
            }
        }
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
        throwCoroutineRunning = true;
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
        throwForceFromCoroutine = throwForce;
        chargeBar.Cleanup();
        throwCoroutineRunning = false;
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

    private bool IsUseButtonDown()
    {
        //TODO: change this to an actual input
        return Input.GetKeyDown(KeyCode.T) || ControllerInputHelper.IsOKPressed();
    }
}