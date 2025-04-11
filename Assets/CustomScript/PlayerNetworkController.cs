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

    public NetworkInputData getCurrentInput()
    {
        Vector3 moveVec = Vector3.zero;

        Vector3 cameraLook = cameraObj.transform.forward;
        cameraLook.y = 0f;
        cameraLook = cameraLook.normalized;

        Vector3 forwardVec = cameraLook;
        Vector3 rightVec = Vector3.Cross(forwardVec, Vector3.up).normalized * -1;

        moveVec += rightVec * Input.GetAxis("Horizontal");
        moveVec += forwardVec * Input.GetAxis("Vertical");

        return new NetworkInputData()
        {
            direction = moveVec
        };
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            _cc.Move(data.direction);
        }
    }
}
