using UnityEngine;
using Fusion;
using UnityEngine.EventSystems;

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
        if (ControllerInputHelper.IsADown()|| Input.GetKeyDown(KeyCode.P))
        {
            // Debug.Log("P pressed");
            networkAnimator.SetTrigger("Point", true);
        }
        // Y in controller
        if (ControllerInputHelper.IsYButtonDown() || Input.GetKeyDown(KeyCode.N))
        {
            // Debug.Log("N pressed");
            networkAnimator.SetTrigger("Wave", true);
        } 
        if(Mathf.Abs(Input.GetAxis("Horizontal"))>0.1f || Mathf.Abs(Input.GetAxis("Vertical"))>0.1f)
        {   
            // Debug.Log("Moving");
            networkAnimator.SetTrigger("Walk", true);
        }else{
            // Debug.Log("Not Moving");
            networkAnimator.SetTrigger("WalkOff", true);
        }
    }
}
