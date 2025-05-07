using UnityEngine;
using UnityEngine.EventSystems;
using System;
using Fusion;
using UnityEngine.UIElements;
public class CupInteractable : NetworkBehaviour, IPointerClickHandler, IPointerEnterHandler,IPointerExitHandler
{
    bool flipped;
    int flipFrameNumber = 0;
    NetworkTransform networkTransform;
    Outline outlineComponent;
    Vector3 originalPosition;
    Quaternion originalRotation;
    Vector3 flippedPosition;

    public static event Action OnGlobalTrigger;
    public bool isHovered = false; 

    void Start()
    {
        // OnGlobalTrigger += Reset;
        networkTransform = GetComponent<NetworkTransform>();
        outlineComponent = gameObject.GetComponent<Outline>();
        outlineComponent.enabled = false;
        outlineComponent.OutlineWidth = 10f;
        outlineComponent.OutlineColor = Color.yellow;
        outlineComponent.OutlineMode = Outline.Mode.OutlineVisible;
        flipped = false;
        
        originalPosition = transform.position;
        flippedPosition = originalPosition + new Vector3(0f, 0.09149997f*2f, 0f);

    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RpcFlipCup()
    {
        Debug.Log($"RpcRequestFlipCup executed on client: {Runner.LocalPlayer}");

        // Toggle the flip state
        flipped = !flipped;

        // Apply the flip transformation
        if (flipped)
        {
            networkTransform.Teleport(flippedPosition, Quaternion.Euler(0, 0, 180f));
            Debug.Log("Flipping cup down");
        }
        else
        {
            networkTransform.Teleport(originalPosition, Quaternion.Euler(0, 0, 0));
            Debug.Log("Flipping cup up");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"OnPointerClick called by: {Runner.LocalPlayer}");
        RpcFlipCup();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        var networkObject = GetComponent<NetworkObject>();
        outlineComponent.enabled = true;
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        outlineComponent.enabled = false;
        isHovered = false;
    }

}
