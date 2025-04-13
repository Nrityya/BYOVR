using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(NetworkRigidbody3D))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ChargeBar))]
class PoolTarget : NetworkBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool highlightingEnabled = false;
    public bool HighlightingEnabled
    {
        get => highlightingEnabled;
        set
        {
            if (!value && outline) outline.enabled = false;
            highlightingEnabled = value;
        }
    }

    Outline outline;
    Rigidbody rb;

    void Start()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
        outline.OutlineWidth = 10f;
        outline.OutlineColor = Color.yellow;
        outline.OutlineMode = Outline.Mode.OutlineVisible;

        rb = GetComponent<Rigidbody>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SwitchHighlight(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SwitchHighlight(false);
    }

    public Rigidbody GetRigidbody()
    {
        return rb;
    }

    public void SwitchHighlight(bool enabled)
    {
        if (HighlightingEnabled) outline.enabled = enabled;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcGetTargeted()
    {
        rb.isKinematic = true;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcHit(Vector3 force)
    {
        rb.isKinematic = false;
        rb.AddForce(force, ForceMode.Impulse);
    }
}