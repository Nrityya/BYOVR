using Fusion;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(NetworkObject))]
public class Interactable : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IDynamicSelectable
{
    protected bool isHovered = false;
    protected Camera playerCamera;

    Outline outlineComponent;
    NetworkObject networkObject;
    public NetworkId NetworkId => networkObject.Id;

    PlayerNetworkController controllingPlayer = null;
    public bool IsControlled => controllingPlayer != null;

    private bool initialized = false;

    protected void SetupInteractable()
    {
        initialized = true;

        outlineComponent = gameObject.GetComponent<Outline>();
        outlineComponent.enabled = false;
        outlineComponent.OutlineWidth = 10f;
        outlineComponent.OutlineColor = Color.cyan;
        outlineComponent.OutlineMode = Outline.Mode.OutlineVisible;

        networkObject = GetComponent<NetworkObject>();
    }

    public void TakeControl(PlayerNetworkController playerController)
    {
        if (!initialized) SetupInteractable();
        controllingPlayer = playerController;
        playerController.OnObjectTakeControl(this);
        OnTakeControl(playerController);
    }

    public void RelieveControl()
    {
        controllingPlayer.OnObjectRelieveControl(this);
        controllingPlayer = null;
    }

    protected virtual void OnTakeControl(PlayerNetworkController playerNetworkController)
    {
    }

    public virtual ControlledObjectState GetNetworkState()
    {
        return new ControlledObjectState();
    }

    public virtual void UpdateFromNetworkState(NetworkInputData data)
    {
    }

    public virtual bool ShouldBeSelected(PointerEventData pointerEventData)
    {
        return false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        playerCamera ??= eventData.enterEventCamera ?? eventData.pressEventCamera;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!initialized) SetupInteractable();
        outlineComponent.enabled = true;
        playerCamera = eventData.enterEventCamera ?? eventData.pressEventCamera;
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!initialized) SetupInteractable();
        outlineComponent.enabled = false;
        isHovered = false;
    }

    public void OnDestroy()
    {
        if (IsControlled) RelieveControl();
    }
}

