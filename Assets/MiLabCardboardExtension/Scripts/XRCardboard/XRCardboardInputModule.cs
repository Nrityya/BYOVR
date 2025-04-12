using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;
using UnityEngine.UI;
using System;
#if !UNITY_EDITOR
using UnityEngine.XR;
#endif

public class XRCardboardInputModule : PointerInputModule
{
    public PlayerNetworkController playerController;

    [SerializeField]
    XRCardboardInputSettings settings = default;
    [SerializeField]
    UnityFloatEvent onStartHover = default;
    [SerializeField]
    UnityEvent onEndHover = default;
    [SerializeField]
    UnityEvent onClick = default;

    PointerEventData pointerEventData;
    GameObject currentTarget;
    float currentTargetClickTime = float.MaxValue;
    bool hovering;

    public override void Process()
    {
        HandleLook();
        HandleSelection();
    }

    void HandleLook()
    {
        if (pointerEventData == null)
            pointerEventData = new PointerEventData(eventSystem);
#if UNITY_EDITOR
        pointerEventData.position = new Vector2(Screen.width / 2, Screen.height / 2);
#else
        pointerEventData.position = new Vector2(XRSettings.eyeTextureWidth / 2, XRSettings.eyeTextureHeight / 2);
#endif
        pointerEventData.delta = Vector2.zero;
        var raycastResults = new List<RaycastResult>();
        eventSystem.RaycastAll(pointerEventData, raycastResults);
        raycastResults = raycastResults.OrderBy(r => !r.module.GetComponent<GraphicRaycaster>()).ToList();
        pointerEventData.pointerCurrentRaycast = FindFirstRaycast(raycastResults);
        ProcessMove(pointerEventData);
    }

    void HandleSelection()
    {
        GameObject handler;
        try
        {
            handler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(pointerEventData.pointerEnter);
            var selectable = handler.GetComponent<Selectable>();
            if (selectable && selectable.interactable == false)
                throw new NullReferenceException();
        }
        catch (NullReferenceException)
        {
            StopHovering(currentTarget);
            currentTarget = null;
            return;
        }

        if (currentTarget != handler)
        {
            if (hovering)
                StopHovering(currentTarget);
            var gazeTime = settings.GazeTime;
            currentTarget = handler;
            currentTargetClickTime = Time.realtimeSinceStartup + gazeTime;
            hovering = true;
            onStartHover?.Invoke(gazeTime);
            if (playerController) playerController.OnObjectStartHover(currentTarget);
        }

        var dynamicallySelected = IsDynamicallySelected(currentTarget, pointerEventData);
        var traditionallySelected = (Time.realtimeSinceStartup > currentTargetClickTime && settings.ClickOnHover) || Input.GetButtonDown(settings.ClickInput);
        if ((dynamicallySelected == null && traditionallySelected) || dynamicallySelected == true)
        {
            ExecuteEvents.ExecuteHierarchy(currentTarget, pointerEventData, ExecuteEvents.pointerClickHandler);
            currentTargetClickTime = float.MaxValue;
            onClick?.Invoke();
            StopHovering(currentTarget);
            if (playerController) playerController.OnObjectSelection(currentTarget);
        }
    }

    void StopHovering(GameObject target)
    {
        if (!hovering)
            return;
        if (playerController) playerController.OnObjectEndHover(target);
        hovering = false;
        onEndHover?.Invoke();
    }

    bool? IsDynamicallySelected(GameObject selectedObj, PointerEventData pointerEventData)
    {
        if (selectedObj == null) return null;
        IDynamicSelectable selectable;
        try
        {
            selectable = selectedObj.GetComponent<IDynamicSelectable>();
        }
        catch (NullReferenceException)
        {
            return null;
        }
        if (selectable == null) return null;
        return selectable.ShouldBeSelected(pointerEventData);
    }
}

public interface IDynamicSelectable
{
    bool ShouldBeSelected(PointerEventData pointerEventData);
}