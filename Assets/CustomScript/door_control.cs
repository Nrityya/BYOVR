using Fusion;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(NetworkTransform))]
public class DoorControl : NetworkBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    Vector3 close_position = new Vector3(2f, 0.0f, -1f);
    Vector3 close_rotation = new Vector3(0.0f, -90.0f, 0.0f);
    Vector3 open_position = new Vector3(2.5f, 0.0f, -1.4f);
    Vector3 open_rotation = new Vector3(0.0f, 0.0f, 0.0f);

    bool door_open = false;

    NetworkTransform networkTransform;

    void Start()
    {
        networkTransform = GetComponent<NetworkTransform>();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RpcToggleDoor()
    {
        if (door_open) networkTransform.Teleport(close_position, Quaternion.Euler(close_rotation));
        else networkTransform.Teleport(open_position, Quaternion.Euler(open_rotation));
        door_open = !door_open;
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
        if (offset.magnitude < 8)
        {
            RpcToggleDoor();
        }
    }
}
