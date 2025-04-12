
using System.Collections;
using Fusion;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NetworkTransform))]
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

    GameObject chargeBarPrefab;

    bool throwCoroutineRunning = false;
    float throwForceFromCoroutine = 0;

    void Start()
    {
        SetupInteractable();

        rigidbodyComponent = gameObject.GetComponent<Rigidbody>();
        rigidbodyComponent.interpolation = RigidbodyInterpolation.Interpolate;

        chargeBarPrefab = Resources.Load<GameObject>("Prefabs/Charge Bar");
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
            releaseForce = throwForceFromCoroutine
        };

        if (IsThrowButtonDown() && throwable && !throwCoroutineRunning)
        {
            StartCoroutine(ThrowCoroutine());
        }

        if (throwForceFromCoroutine > 0)
        {
            throwForceFromCoroutine = 0;
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

        if (state.releaseForce > 0)
        {
            transform.position = state.targetPosition;
            EndGrab();
            rigidbodyComponent.AddForce(data.lookDirection * state.releaseForce, ForceMode.Impulse);
        }
    }

    public override bool ShouldBeSelected(PointerEventData pointerEventData)
    {
        if (IsGrabButtonDown()) return true;
        if (IsControlled) return IsThrowButtonDown() || IsUseButtonDown();
        return false;
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
        RelieveControl();
    }

    private IEnumerator ThrowCoroutine()
    {
        throwCoroutineRunning = true;

        // GameObject bar = (GameObject)Instantiate(chargeBarPrefab, transform.position, Quaternion.identity, transform);
        // bar.transform.localScale = new Vector3(0.004f, 0.004f, 0.004f) / this.transform.localScale.x;
        // UnityEngine.UI.Image chargeBarComponent = bar.transform.GetComponentsInChildren<UnityEngine.UI.Image>()[2];
        // Color c = Color.yellow;

        float i = 0;
        while (!IsThrowButtonUp())
        {
            if (i > throwMaxChargeTime)
            {
                i = throwMaxChargeTime;
            }
            else
            {
                i += Time.deltaTime;
            }
            // float t = Mathf.Lerp(1, 0, i / throwMaxChargeTime);
            // float shakeSpeed = 0.008f * (1 - t);
            // is there a more efficient way to do this?
            // bar.transform.position = this.transform.position + playerCamera.transform.right * Random.Range(-shakeSpeed, shakeSpeed) + playerCamera.transform.up * Random.Range(-shakeSpeed, shakeSpeed);
            // bar.transform.LookAt(playerCamera.transform.position);
            // bar.transform.Rotate(0, 180, 0);
            // c.g = t;
            // chargeBarComponent.color = c;
            // chargeBarComponent.fillAmount = 1 - t;

            yield return null;
        }
        float throwForce = Mathf.Min(i * (throwForceMax - throwForceMin) / throwMaxChargeTime + throwForceMin, throwForceMax);
        throwForceFromCoroutine = throwForce;
        // Destroy(bar);
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