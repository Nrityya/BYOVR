using Fusion;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(NetworkObject))]
public class Interactable : NetworkBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IDynamicSelectable
{
    protected bool isHovered = false;
    protected Camera playerCamera;

    Outline outlineComponent;
    NetworkObject networkObject;
    public NetworkId NetworkId => networkObject.Id;

    protected PlayerNetworkController controllingPlayer = null;
    public bool IsControlled => controllingPlayer != null;

    private bool initialized = false;

    protected virtual string TooltipText { get => null; }

    protected void SetupInteractable()
    {
        initialized = true;

        outlineComponent = gameObject.GetComponent<Outline>();
        outlineComponent.enabled = false;
        outlineComponent.OutlineWidth = 10f;
        outlineComponent.OutlineColor = Color.cyan;
        outlineComponent.OutlineMode = Outline.Mode.OutlineVisible;

        networkObject = GetComponent<NetworkObject>();
        networkObject.Flags = NetworkObjectFlags.V1 | NetworkObjectFlags.AllowStateAuthorityOverride;
    }

    public void TakeControl(PlayerNetworkController playerController)
    {
        if (!initialized) SetupInteractable();
        controllingPlayer = playerController;
        playerController.OnObjectTakeControl(this);
        OnTakeControl(playerController);
        if (playerController.IsLocal) networkObject.RequestStateAuthority();
        if (!string.IsNullOrEmpty(TooltipText)) ToolTipController.PushTooltip(TooltipText);
    }

    public void RelieveControl()
    {
        controllingPlayer.OnObjectRelieveControl(this);
        controllingPlayer = null;
        if (!string.IsNullOrEmpty(TooltipText)) ToolTipController.PopTooltip(TooltipText);
    }

    protected virtual void OnTakeControl(PlayerNetworkController playerNetworkController)
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

