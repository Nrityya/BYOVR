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
        if (Input.GetKeyDown(KeyCode.JoystickButton10))
        {
            Debug.Log("P pressed");
            networkAnimator.SetTrigger("Point", true);
        }
        // Y in controller
        if (Input.GetKeyDown(KeyCode.JoystickButton3))
        {
            Debug.Log("N pressed");
            networkAnimator.SetTrigger("Wave", true);
        }
    }
}
