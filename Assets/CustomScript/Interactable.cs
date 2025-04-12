using System.Collections;
using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NetworkObject))]
public class Interactable : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IDynamicSelectable
{
    public float grabForce = 15.0f;
    public float grabOffset = 1.0f;
    public float throwForceMax = 20f;
    public float throwForceMin = 5f;
    public float throwMaxChargeTime = 2f;
    public GameObject chargeBarPrefab;

    public bool throwable = true;
    public bool grabbable = true;
    public bool rotatable = true;
    bool isHovered = false;

    bool RigidbodyConstrained => rigidbodyComponent.constraints != RigidbodyConstraints.None;

    Camera playerCamera;
    Outline outlineComponent;
    Rigidbody rigidbodyComponent;
    NetworkObject networkObject;

    public NetworkId NetworkId => networkObject.Id;

    PlayerNetworkController controllingPlayer = null;
    public bool IsControlled => controllingPlayer != null;

    void Start()
    {
        outlineComponent = gameObject.GetComponent<Outline>();
        outlineComponent.enabled = false;
        outlineComponent.OutlineWidth = 10f;
        outlineComponent.OutlineColor = Color.cyan;
        outlineComponent.OutlineMode = Outline.Mode.OutlineVisible;

        rigidbodyComponent = gameObject.GetComponent<Rigidbody>();
        rigidbodyComponent.interpolation = RigidbodyInterpolation.Interpolate;

        networkObject = GetComponent<NetworkObject>();

        chargeBarPrefab = Resources.Load<GameObject>("Prefabs/Charge Bar");
    }

    public void TakeControl(PlayerNetworkController playerController)
    {
        controllingPlayer = playerController;
        Grabbed();
    }

    public virtual ControlledObjectState GetNetworkState()
    {
        return new ControlledObjectState()
        {
            targetPosition = playerCamera.transform.position + playerCamera.transform.forward * grabOffset
        };
    }

    public virtual void UpdateFromNetworkState(ControlledObjectState state)
    {
        var offset = state.targetPosition - transform.position;
        rigidbodyComponent.linearVelocity = offset * grabForce;
    }

    public void RelieveControl()
    {
        controllingPlayer.OnObjectRelieveControl(this);
        controllingPlayer = null;
    }

    // void Update()
    // {
    //     // Need to check every frame in case the camera is moving quickly
    //     // if (IsControlled && !isHovered && ShouldBeSelected(null))
    //     // {
    //     //     OnPointerClick(null);
    //     // }
    // }


    // void FixedUpdate()
    // {
    //     if (IsControlled)
    //     {
    //         // UpdateGrab();
    //     }
    // }

    public bool ShouldBeSelected(PointerEventData pointerEventData)
    {
        if (IsGrabButtonDown()) return true;
        if (IsControlled) return IsThrowButtonDown() || IsUseButtonDown();
        return false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // We can assume one of the desired buttons is pressed

        // if (IsControlled)
        // {
        //     if (IsGrabButtonDown())
        //     {
        //         Released();
        //     }
        //     else if (IsThrowButtonDown() && throwable)
        //     {
        //         StartCoroutine(ThrowCoroutine());
        //     }
        //     else if (IsUseButtonDown())
        //     {
        //         Released();
        //         BroadcastMessage("Used", playerCamera, SendMessageOptions.DontRequireReceiver);
        //     }
        // }
        // else if (grabbable)
        // {
        //     if (playerCamera.GetComponent<PlayerControl>().GetPlayerState() == PlayerControl.PlayerState.HoldingCue || playerCamera.GetComponent<PlayerControl>().GetPlayerState() == PlayerControl.PlayerState.HittingCue)
        //     {
        //         playerCamera.BroadcastMessage("Hit", this.gameObject);
        //     }
        //     else
        //     {
        //         Grabbed();
        //     }
        // }
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
        rigidbodyComponent.useGravity = false;
        rigidbodyComponent.linearVelocity = Vector3.zero;
        rigidbodyComponent.angularDamping = 15f;
    }

    public void Released()
    {
        rigidbodyComponent.useGravity = true;
        rigidbodyComponent.constraints = RigidbodyConstraints.None;
        rigidbodyComponent.angularDamping = 0.05f;
        RelieveControl();
    }

    // public void UpdateGrab()
    // {
    //     var target = playerCamera.transform.position + playerCamera.transform.forward * grabOffset;
    //     var offset = target - transform.position;
    //     rigidbodyComponent.linearVelocity = offset * grabForce;

    //     if (IsRotateButtonPressed() && rotatable)
    //     {
    //         if (RigidbodyConstrained) rigidbodyComponent.constraints = RigidbodyConstraints.None;

    //         // TODO: Joystick
    //         transform.Rotate(playerCamera.transform.forward * -Input.GetAxis("Mouse X") * 5f, Space.World);
    //         transform.Rotate(playerCamera.transform.right * Input.GetAxis("Mouse Y") * 5f, Space.World);
    //     }
    //     else if (!RigidbodyConstrained)
    //     {
    //         // rigidbodyComponent.constraints = RigidbodyConstraints.FreezeRotation;
    //         //unsure if this is ever used now            
    //     }
    // }
    // private IEnumerator ThrowCoroutine()
    // {
    //     GameObject bar = (GameObject)Instantiate(chargeBarPrefab, transform.position, Quaternion.identity, transform);
    //     bar.transform.localScale = new Vector3(0.004f, 0.004f, 0.004f) / this.transform.localScale.x;
    //     UnityEngine.UI.Image chargeBarComponent = bar.transform.GetComponentsInChildren<UnityEngine.UI.Image>()[2];
    //     Color c = Color.yellow;

    //     float i = 0;
    //     while (!IsThrowButtonUp())
    //     {
    //         if (i > throwMaxChargeTime)
    //         {
    //             i = throwMaxChargeTime;
    //         }
    //         else
    //         {
    //             i += Time.deltaTime;
    //         }
    //         float t = Mathf.Lerp(1, 0, i / throwMaxChargeTime);
    //         float shakeSpeed = 0.008f * (1 - t);
    //         // is there a more efficient way to do this?
    //         bar.transform.position = this.transform.position + playerCamera.transform.right * Random.Range(-shakeSpeed, shakeSpeed) + playerCamera.transform.up * Random.Range(-shakeSpeed, shakeSpeed);
    //         bar.transform.LookAt(playerCamera.transform.position);
    //         bar.transform.Rotate(0, 180, 0);
    //         c.g = t;
    //         chargeBarComponent.color = c;
    //         chargeBarComponent.fillAmount = 1 - t;

    //         yield return null;
    //     }
    //     float throwForce = Mathf.Min(i * (throwForceMax - throwForceMin) / throwMaxChargeTime + throwForceMin, throwForceMax);
    //     rigidbodyComponent.AddForce(playerCamera.transform.forward * throwForce, ForceMode.Impulse);
    //     Released();
    //     Destroy(bar);
    // }

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

    public interface IInteractable
    {
        void Used(Camera c);
    }
}

