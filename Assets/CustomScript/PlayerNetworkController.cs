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

    public Rigidbody itemRigidbodyTarget;

    private CharacterController cc;
    private Interactable controlledObject;
    public GameObject avatar;

    public bool IsLocal => Object.HasStateAuthority;

    public bool movementEnabled = true;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        // avatar = transform.Find("Avatar").gameObject;
        GetComponentInChildren<SkinnedMeshRenderer>().material.color = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);
        var netTransform = GetComponent<NetworkTransform>();
        netTransform.DisableSharedModeInterpolation = true;
    }

    private void Start()
    {
        serverGroup.SetActive(!IsLocal);
        clientGroup.SetActive(IsLocal);
    }

    public void FixedUpdate()
    {
        Vector3 cameraLook = cameraObj.transform.forward;
        cameraLook.y = 0f;
        cameraLook = cameraLook.normalized;
        avatar.transform.forward = cameraLook;
    }
    public void Update()
    {
        if (!IsLocal || !movementEnabled) return;

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
        


        if (ControllerInputHelper.IsBDown() || Input.GetKeyDown(KeyCode.Q))
        {
            if (Physics.Raycast(cameraObj.transform.position, cameraObj.transform.forward, out RaycastHit hit))
            {
                cc.enabled = false;
                transform.position = hit.point + Vector3.up * cc.height * 0.5f;
                cc.enabled = true;
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
