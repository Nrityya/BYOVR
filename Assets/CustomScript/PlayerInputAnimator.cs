using UnityEngine;
using Fusion;

public class PlayerInputAnimator : NetworkBehaviour
{
    private NetworkMecanimAnimator networkAnimator;

    void Start()
    {
        networkAnimator = GetComponent<NetworkMecanimAnimator>();
    }

    void Update()
    {
        if (!HasInputAuthority) return;
        // A in controlelr
        if (ControllerInputHelper.IsADown())
        {
            Debug.Log("P pressed");
            networkAnimator.SetTrigger("Point", true);
        }
        // Y in controller
        if (ControllerInputHelper.IsYButtonDown())
        {
            Debug.Log("N pressed");
            networkAnimator.SetTrigger("Wave", true);
        }
    }
}
