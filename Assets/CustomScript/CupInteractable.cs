using UnityEngine;
using UnityEngine.EventSystems;
using System;
using Fusion;
using UnityEngine.UIElements;
public class CupInteractable : NetworkBehaviour, IPointerClickHandler, IPointerEnterHandler,IPointerExitHandler
{
    bool flipped;
    NetworkTransform networkTransform;
    Outline outlineComponent;
    Vector3 originalPosition;
    Vector3 flippedPosition;
    public bool isHovered = false; 
    SphereCollider checkerCollider;

    void Start()
    {
        checkerCollider = GetComponentInChildren<SphereCollider>();
        networkTransform = GetComponent<NetworkTransform>();
        outlineComponent = gameObject.GetComponent<Outline>();
        outlineComponent.enabled = false;
        outlineComponent.OutlineWidth = 10f;
        outlineComponent.OutlineColor = Color.yellow;
        outlineComponent.OutlineMode = Outline.Mode.OutlineVisible;
        flipped = false;
        
        originalPosition = transform.position;
        flippedPosition = originalPosition + new Vector3(0f, 0.09149997f*2f, 0f);
        RpcReset();

    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RpcFlipCup()
    {
        if (!flipped)
        {
            networkTransform.Teleport(flippedPosition, Quaternion.Euler(0, 0, 180f));
            
        }
        else
        {
            networkTransform.Teleport(originalPosition, Quaternion.Euler(0, 0, 0));
        }
        checkerCollider.enabled = flipped;
        flipped = !flipped;
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Debug.Log($"OnPointerClick called by: {Runner.LocalPlayer}");
        RpcFlipCup();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        outlineComponent.enabled = true;
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        outlineComponent.enabled = false;
        isHovered = false;
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RpcReset()
    {
        if (flipped)
        {
            RpcFlipCup();
        }
    }

}
