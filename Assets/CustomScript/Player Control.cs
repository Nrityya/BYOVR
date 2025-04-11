using System.Collections;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{

    GameObject CharObject;
    public enum PlayerState
    {
        Idle,
        HoldingCue,
        HittingCue,
        HoldingCards,
        
    }

    public PlayerState playerState = PlayerState.Idle;

    public void SetPlayerState(PlayerState newState)
    {
        playerState = newState;
    }

    public PlayerState GetPlayerState()
    {
        return playerState;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CharObject = GameObject.Find("Character");
    }

    // Update is called once per frame
    void Update()
    {
        if(playerState == PlayerState.HittingCue)
        {
            CharObject.GetComponent<CharacterMovement>().enabled = false;
            // float horComp = Input.GetAxis("Horizontal");
            // float vertComp = Input.GetAxis("Vertical");

            // Vector3 aimVect = Vector3.zero;

            // Vector3 cameraLook = this.transform.forward;
            // cameraLook.y = 0f;
            // cameraLook = cameraLook.normalized;

            // Vector3 forwardVect = cameraLook;
            // Vector3 rightVect = Vector3.Cross(forwardVect, Vector3.up).normalized * -1;

            // aimVect += rightVect * horComp;
            // aimVect += forwardVect * vertComp; 

        }
        else
        {
            CharObject.GetComponent<CharacterMovement>().enabled = true;
        }
        
    }
}
