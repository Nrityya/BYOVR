using Fusion.Addons.Physics;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ChargeBar))]
[RequireComponent(typeof(NetworkRigidbody3D))]
[RequireComponent(typeof(CapsuleCollider))]
public class PoolCueInteractable : Interactable
{
    Rigidbody rb;
    ChargeBar chargeBar;
    FixedJoint joint;
    CapsuleCollider capsuleCollider;

    int pickUpFrameNumber = 0;

    public void Start()
    {
        SetupInteractable();

        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        chargeBar = GetComponent<ChargeBar>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    protected override void OnTakeControl(PlayerNetworkController playerNetworkController)
    {
        PickUp();
    }

    public void Update()
    {
        if (!IsControlled || !controllingPlayer.IsLocal) return;

        if (IsGrabButtonDown() && Time.frameCount > pickUpFrameNumber)
        {
            Drop();
        }
    }

    private void PickUp()
    {
        var camera = controllingPlayer.cameraObj;
        var distance = 0.25f;
        var frustumHeight = 2.0f * distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        var frustumWidth = frustumHeight * camera.aspect;
        var offset = camera.transform.forward * distance + camera.transform.right * frustumWidth * 0.7f;
        offset.y -= capsuleCollider.height * 0.8f;

        var target = controllingPlayer.itemRigidbodyTarget;
        transform.position = target.transform.position + offset;

        var rot = camera.transform.rotation.eulerAngles + new Vector3(0, 0, 90);
        transform.rotation = Quaternion.Euler(rot);

        joint ??= gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = target;

        rb.excludeLayers = ~0; // Everything

        pickUpFrameNumber = Time.frameCount;
    }

    private void Drop()
    {
        rb.excludeLayers = 0; // Nothing
        Destroy(joint);
        joint = null;
        // transform.position = controllingPlayer.cameraObj.transform.position;
        // transform.rotation = controllingPlayer.cameraObj.transform.rotation;
        RelieveControl();
    }

    public override bool ShouldBeSelected(PointerEventData pointerEventData)
    {
        return !IsControlled && IsGrabButtonDown();
    }

    private bool IsGrabButtonDown()
    {
        return Input.GetKeyDown(KeyCode.E) || ControllerInputHelper.IsXButtonDown();
    }

    private bool IsHitButtonDown()
    {
        return Input.GetKeyDown(KeyCode.R) || ControllerInputHelper.IsADown();
    }
}