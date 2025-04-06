using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed = 5f;


     void Start()
    {
        Debug.Log($"{gameObject.name} spawned - IsOwner: {IsOwner}");
    }

    void Update()
    {
        if (!IsOwner)
            return;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Debug.Log($"Input detected: moveX = {moveX}, moveZ = {moveZ}");

        Vector3 movement = new Vector3(moveX, 0, moveZ) * moveSpeed * Time.deltaTime;
        transform.Translate(movement);
    }

}
