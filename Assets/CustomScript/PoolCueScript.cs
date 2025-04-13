using UnityEngine;
using static Interactable;

public class PoolCueScript : MonoBehaviour
{
    CapsuleCollider capsuleCollider;
    Camera heldPlayerCamera;
    Rigidbody rigidbodyComponent;
    GameObject HitSelected;

    public void Used(Camera c)
    {
        heldPlayerCamera = c;
        heldPlayerCamera.GetComponent<PlayerControl>().SetPlayerState(PlayerControl.PlayerState.HoldingCue);
        HittingCollider();
        transform.parent = heldPlayerCamera.transform;
        rigidbodyComponent.useGravity = false;
        rigidbodyComponent.isKinematic = true;
    }

    public void Hit(GameObject o)
    {
        Vector3 center = capsuleCollider.center;
        Debug.Log("Hit: " + o.name);
        HitSelected = o;
        HitSelected.layer = LayerMask.NameToLayer("Selected");
        heldPlayerCamera.GetComponent<PlayerControl>().SetPlayerState(PlayerControl.PlayerState.HittingCue);
    }

    public void HitReleased(Vector3 aim)
    {
        Debug.Log("Hit: " + aim);
        HitSelected.GetComponent<Rigidbody>().AddForce(aim * -10f, ForceMode.Impulse);
        heldPlayerCamera.GetComponent<PlayerControl>().SetPlayerState(PlayerControl.PlayerState.HoldingCue);
        HitSelected.layer = LayerMask.NameToLayer("Default");
        HitSelected = null;
    }
    void HittingCollider()
    {
        Debug.Log("HittingCollider");
        capsuleCollider.radius = 0.005f;
        capsuleCollider.height = 0.15f;
        capsuleCollider.center = new Vector3(0.677f, 0, 0);
        gameObject.layer = LayerMask.NameToLayer("Pool Cue");

    }

    void DefaultCollider()
    {
        capsuleCollider.radius = 0.01f;
        capsuleCollider.height = 1.505f;
        capsuleCollider.center = new Vector3(0, 0, 0);
        gameObject.layer = LayerMask.NameToLayer("Default");
    }

    void Update()
    {
        if (heldPlayerCamera)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                DefaultCollider();
                heldPlayerCamera.GetComponent<PlayerControl>().SetPlayerState(PlayerControl.PlayerState.Idle);
                transform.parent = null;
                rigidbodyComponent.useGravity = true;
                rigidbodyComponent.isKinematic = false;
                heldPlayerCamera = null;
            }
            else
            if (HitSelected)
            {
                float horComp = Input.GetAxis("Horizontal");
                float vertComp = Input.GetAxis("Vertical");

                Vector3 aimVect = Vector3.zero;

                Vector3 cameraLook = heldPlayerCamera.transform.forward;
                cameraLook.y = 0f;
                cameraLook = cameraLook.normalized;

                Vector3 forwardVect = cameraLook;
                Vector3 rightVect = Vector3.Cross(forwardVect, Vector3.up).normalized * -1;

                aimVect += rightVect * horComp + forwardVect * vertComp;
                if (aimVect.magnitude < 0.1f)
                {
                    aimVect = cameraLook * -1f;
                }
                transform.position = HitSelected.transform.position + 1.5f * aimVect.normalized;
                transform.rotation = Quaternion.LookRotation(HitSelected.transform.position - transform.position, Vector3.up) * Quaternion.Euler(0, -90, 0);
                if (Input.GetButtonDown("Submit"))
                {
                    HitReleased(aimVect);

                }
                Debug.Log("Aim Vector: " + aimVect);
            }
            else
            {
                transform.position = heldPlayerCamera.transform.position + heldPlayerCamera.transform.forward * 0.5f + heldPlayerCamera.transform.right * 0.5f;
                transform.rotation = heldPlayerCamera.transform.rotation * Quaternion.Euler(0, 90, 90);
            }

        }
    }
    void Start()
    {
        rigidbodyComponent = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

}
