using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

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
    public NetworkRigidbody3D networkRigidbody3D; 
    Vector3 startingPos;
    Vector3 cueStartingPos;
    public GameObject ShownBall;
    public override void Spawned()
    {  
        networkRigidbody3D = GetComponent<NetworkRigidbody3D>();
        ShownBall = GameObject.Find(name+ " (1)");
        outline = GetComponent<Outline>();
        outline.enabled = false;
        outline.OutlineWidth = 10f;
        outline.OutlineColor = Color.yellow;
        outline.OutlineMode = Outline.Mode.OutlineVisible;
        startingPos = transform.position;
        cueStartingPos = transform.position + new Vector3(0, 0.3f, -0.96f);
        rb = GetComponent<Rigidbody>();
        RpcReset();  
        
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

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcReset()
    {
        networkRigidbody3D.transform.position = startingPos;
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        if(ShownBall) ShownBall.SetActive(false);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcPocketed()
    {
        if (name.Contains("cueball"))
        {
            networkRigidbody3D.transform.position = cueStartingPos;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        } else {
            if(ShownBall) ShownBall.SetActive(true);
            Destroy(gameObject);
        }
    }
}