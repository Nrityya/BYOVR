using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Addons.Physics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(NetworkRigidbody3D))]
[RequireComponent(typeof(CapsuleCollider))]
public class PoolCueInteractable : Interactable, IStateAuthorityChanged
{
    public float maxChargeTime = 2;
    public float hitForceMin = 1;
    public float hitForceMax = 15;

    Rigidbody rb;
    Joint joint;
    CapsuleCollider capsuleCollider;

    PoolTarget targetBody = null;

    int pickUpFrameNumber = 0;

    readonly List<PoolTarget> cachedTargets = new();

    public void Start()
    {
        SetupInteractable();

        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;

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
            return;
        }

        if (targetBody == null)
        {
            var cameraTransform = controllingPlayer.cameraObj.transform;
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit) && hit.rigidbody != null)
            {
                if (hit.rigidbody.TryGetComponent(out PoolTarget comp))
                {
                    if (!cachedTargets.Contains(comp))
                    {
                        cachedTargets.Add(comp);
                        comp.HighlightingEnabled = true;
                        comp.SwitchHighlight(true);
                    }
                    if (IsSelectButtonDown())
                    {
                        TargetObject(comp);
                    }
                }
            }
        }
        else
        {
            if (IsHitButtonDown())
            {
                StartCoroutine(HitCoroutine());
            }
            else
            {
                rb.linearVelocity = transform.forward * -Input.GetAxis("Horizontal") * 3;
            }
        }
    }

    private void PickUp()
    {
        targetBody = null;
        if (joint != null)
        {
            Destroy(joint);
            joint = null;
        }
        controllingPlayer.movementEnabled = true;

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

        joint = gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = target;

        rb.excludeLayers = ~0; // Everything

        pickUpFrameNumber = Time.frameCount;
    }

    private void Drop()
    {
        rb.excludeLayers = 0; // Nothing
        Destroy(joint);
        joint = null;
        controllingPlayer.movementEnabled = true;
        foreach (var comp in cachedTargets)
        {
            comp.HighlightingEnabled = false;
        }
        RelieveControl();
    }

    private void TargetObject(PoolTarget body)
    {
        targetBody = body;
        targetBody.RpcGetTargeted();

        transform.position = body.transform.position - new Vector3(capsuleCollider.height, 0, 0);
        // transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.rotation = Quaternion.Euler(0, playerCamera.transform.rotation.y,0);
        

        Destroy(joint);
        var hinge = gameObject.AddComponent<HingeJoint>();
        joint = hinge;
        hinge.anchor = new Vector3(1, 0, 0);
        hinge.axis = Vector3.up;
        hinge.connectedBody = body.GetRigidbody();

        controllingPlayer.movementEnabled = false;
    }

    private IEnumerator HitCoroutine()
    {
        var chargeBar = targetBody.gameObject.GetComponent<ChargeBar>();
        chargeBar.Setup(playerCamera);

        float start = Time.realtimeSinceStartup;
        float chargeTime = 0;
        while (!IsHitButtonUp())
        {
            chargeTime = Mathf.Min(Time.realtimeSinceStartup - start, maxChargeTime);
            chargeBar.UpdateCharge(chargeTime / maxChargeTime);
            yield return null;
        }
        float hitForce = Mathf.Min(chargeTime * (hitForceMax - hitForceMin) / maxChargeTime + hitForceMin, hitForceMax);
        targetBody.RpcHit(transform.right * hitForce);

        chargeBar.Cleanup();
        PickUp();
    }

    public override bool ShouldBeSelected(PointerEventData pointerEventData)
    {
        return !IsControlled && IsGrabButtonDown();
    }

    private bool IsGrabButtonDown()
    {
        return Input.GetKeyDown(KeyCode.E) || ControllerInputHelper.IsXButtonDown();
    }

    private bool IsSelectButtonDown()
    {
        return Input.GetKeyDown(KeyCode.R) || ControllerInputHelper.IsADown();
    }

    private bool IsHitButtonDown()
    {
        return Input.GetKeyDown(KeyCode.T) || ControllerInputHelper.IsYButtonDown();
    }

    private bool IsHitButtonUp()
    {
        return Input.GetKeyUp(KeyCode.T) || ControllerInputHelper.IsYButtonUp();
    }

    public void StateAuthorityChanged()
    {
        rb.excludeLayers = ~0;
    }
}