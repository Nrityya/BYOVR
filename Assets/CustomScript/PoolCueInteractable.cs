using Fusion.Addons.Physics;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ChargeBar))]
[RequireComponent(typeof(NetworkRigidbody3D))]
[RequireComponent(typeof(FixedJoint))]
[RequireComponent(typeof(CapsuleCollider))]
public class PoolCueInteractable : Interactable
{
    Rigidbody rb;
    NetworkRigidbody3D networkRigidbody;
    ChargeBar chargeBar;
    FixedJoint joint;
    CapsuleCollider capsuleCollider;

    public void Start()
    {
        SetupInteractable();

        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        chargeBar = GetComponent<ChargeBar>();
        networkRigidbody = GetComponent<NetworkRigidbody3D>();
        joint = GetComponent<FixedJoint>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    protected override void OnTakeControl(PlayerNetworkController playerNetworkController)
    {
        var camera = playerNetworkController.cameraObj;
        var distance = 0.5f;
        var frustumHeight = 2.0f * distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        var frustumWidth = frustumHeight * camera.aspect;
        var offset = camera.transform.forward * distance + camera.transform.right * frustumWidth * 0.4f;
        offset.y -= capsuleCollider.height * 0.75f;

        var target = playerNetworkController.itemRigidbodyTarget;
        transform.position = target.transform.position + offset;
        transform.LookAt(camera.transform.up);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, 0, 90));
        joint.connectedBody = target;

        rb.excludeLayers = ~0; // Everything
    }

    public override bool ShouldBeSelected(PointerEventData pointerEventData)
    {
        return !IsControlled && IsGrabButtonDown();
    }

    private bool IsGrabButtonDown()
    {
        return Input.GetKeyDown(KeyCode.E) || ControllerInputHelper.IsXButtonDown();
    }
}