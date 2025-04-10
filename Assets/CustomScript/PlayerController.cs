using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviourPun
{
    public float moveSpeed = 5f;

    void Update()
    {
        // If this PhotonView isn't ours, skip input & movement.
        if (!photonView.IsMine)
            return;

        // Only the local player reaches this point:
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(x, 0, z) * moveSpeed * Time.deltaTime;
        transform.Translate(move);
    }
}
