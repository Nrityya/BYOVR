using Fusion;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NetworkTransform))]
public class PlayerNetworkController : NetworkBehaviour
{
    public float moveSpeed = 5;

    public Camera cameraObj;

    public GameObject serverGroup;

    public GameObject clientGroup;

    private CharacterController cc;
    private Interactable controlledObject;

    public bool IsLocal => Object.HasStateAuthority;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();

        var netTransform = GetComponent<NetworkTransform>();
        netTransform.DisableSharedModeInterpolation = true;
    }

    private void Start()
    {
        serverGroup.SetActive(!IsLocal);
        clientGroup.SetActive(IsLocal);
    }

    public void Update()
    {
        if (!IsLocal) return;

        Vector3 moveVec = Vector3.zero;

        Vector3 cameraLook = cameraObj.transform.forward;
        cameraLook.y = 0f;
        cameraLook = cameraLook.normalized;

        Vector3 forwardVec = cameraLook;
        Vector3 rightVec = Vector3.Cross(forwardVec, Vector3.up).normalized * -1;

        moveVec += rightVec * Input.GetAxis("Horizontal");
        moveVec += forwardVec * Input.GetAxis("Vertical");

        moveVec *= moveSpeed;

        cc.SimpleMove(moveVec);
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
        if (controlledObject && controlledObject != obj) controlledObject.RelieveControl();
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
