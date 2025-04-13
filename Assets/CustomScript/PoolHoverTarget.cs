using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

class PoolHoverTarget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Outline outline;
    bool outlineAlreadyExisted = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        SwitchHighlight(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SwitchHighlight(false);
    }

    public void OnDestroy()
    {
        if (!outline.IsDestroyed()) Destroy(outline);
    }

    public void SwitchHighlight(bool enabled)
    {
        SetupOutline();
        if (outlineAlreadyExisted) return;
        outline.enabled = enabled;
    }

    private void SetupOutline()
    {
        if (!outline && GetComponent<Outline>())
        {
            outlineAlreadyExisted = true;
            return;
        }

        if (!outline)
        {
            outline = gameObject.AddComponent<Outline>();
            outline.enabled = false;
            outline.OutlineWidth = 10f;
            outline.OutlineColor = Color.yellow;
            outline.OutlineMode = Outline.Mode.OutlineVisible;
        }
    }
}