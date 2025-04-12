using Fusion;
using UnityEngine;

[RequireComponent(typeof(NetworkCharacterController))]
public class PlayerNetworkController : NetworkBehaviour
{
    public static PlayerNetworkController localPlayer;

    public Camera cameraObj;

    public GameObject serverGroup;

    public GameObject clientGroup;

    private NetworkCharacterController _cc;
    private Interactable controlledObject;

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterController>();
    }

    private void Start()
    {
        serverGroup.SetActive(!Object.HasInputAuthority);
        clientGroup.SetActive(Object.HasInputAuthority);
        if (Object.HasInputAuthority) localPlayer = this;
    }

    public NetworkInputData GetNewNetworkInput()
    {
        Vector3 moveVec = Vector3.zero;

        Vector3 cameraLook = cameraObj.transform.forward;
        cameraLook.y = 0f;
        cameraLook = cameraLook.normalized;

        Vector3 forwardVec = cameraLook;
        Vector3 rightVec = Vector3.Cross(forwardVec, Vector3.up).normalized * -1;

        moveVec += rightVec * Input.GetAxis("Horizontal");
        moveVec += forwardVec * Input.GetAxis("Vertical");

        var data = new NetworkInputData()
        {
            moveDirection = moveVec,
            lookDirection = cameraObj.transform.forward,
            controlledObjectId = default
        };

        if (controlledObject)
        {
            data.controlledObjectId = controlledObject.NetworkId;
            data.controlledObjectState = controlledObject.GetNetworkState();
            data.controlledObjectState.magic = true;
        }
        else
        {
            data.controlledObjectState = new ControlledObjectState()
            {
                magic = false
            };
        }

        return data;
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            _cc.Move(data.moveDirection);

            if (data.controlledObjectId.IsValid)
            {
                if (controlledObject == null || !data.controlledObjectId.Equals(controlledObject.NetworkId))
                {
                    if (Runner.TryFindObject(data.controlledObjectId, out NetworkObject obj))
                    {
                        controlledObject = obj.GetComponent<Interactable>();
                        controlledObject.TakeControl(this);
                    }
                    else
                    {
                        Debug.Log("Controlled network object does not have interactable component");
                    }
                }
            }
            else if (controlledObject)
            {
                controlledObject.RelieveControl();
            }

            if (controlledObject && data.controlledObjectState.magic)
            {
                controlledObject.UpdateFromNetworkState(data);
            }
        }
    }

    public void OnObjectSelection(GameObject obj)
    {
        if (controlledObject) return;

        Interactable interactable = obj.GetComponent<Interactable>();
        if (interactable == null || interactable.IsControlled) return;

        interactable.TakeControl(this);
    }

    public void OnObjectStartHover(GameObject obj)
    {
    }

    public void OnObjectEndHover(GameObject obj)
    {
    }

    public void OnObjectTakeControl(Interactable obj)
    {
        controlledObject = obj;
    }

    public void OnObjectRelieveControl(Interactable obj)
    {
        controlledObject = null;
    }

    public void OnDestroy()
    {
        if (controlledObject) controlledObject.RelieveControl();
    }
}
