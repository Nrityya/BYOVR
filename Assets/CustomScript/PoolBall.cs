using UnityEngine;
using UnityEngine.EventSystems;

public class PoolBall : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IDynamicSelectable
{
    public void OnPointerClick(PointerEventData eventData)
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }

    public bool ShouldBeSelected(PointerEventData pointerEventData)
    {
        return false;
    }
}