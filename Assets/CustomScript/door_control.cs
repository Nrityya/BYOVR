using Fusion;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(NetworkObject))]
public class DoorControl : NetworkBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    Vector3 close_position = new Vector3(2f, 0.0f, -1f);
    Vector3 close_rotation = new Vector3(0.0f, -90.0f, 0.0f);
    Vector3 open_position = new Vector3(2.5f, 0.0f, -1.4f);
    Vector3 open_rotation = new Vector3(0.0f, 0.0f, 0.0f);

    bool door_open = false;

    void OpenDoor()
    {
        transform.position = open_position;
        transform.rotation = Quaternion.Euler(open_rotation);
        door_open = true;
    }
    void CloseDoor()
    {
        transform.position = close_position;
        transform.rotation = Quaternion.Euler(close_rotation);
        door_open = false;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RpcToggleDoor()
    {
        if (door_open) CloseDoor();
        else OpenDoor();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var offset = eventData.enterEventCamera.transform.position - transform.position;
        if (offset.magnitude < 4)
        {
            RpcToggleDoor();
        }
    }
}
