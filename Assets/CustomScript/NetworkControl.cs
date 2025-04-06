using Unity.Netcode;
using UnityEngine;

public class NetworkControl : MonoBehaviour
{
    void Update()
    {
        // Press H to start as Host (server + client)
        if (Input.GetKeyDown(KeyCode.H))
        {
            NetworkManager.Singleton.StartHost();
            Debug.Log("Host started");
        }

        // Press C to start as Client
        if (Input.GetKeyDown(KeyCode.C))
        {
            NetworkManager.Singleton.StartClient();
            Debug.Log("Client started");
        }

        // Press S to start as Server only
        if (Input.GetKeyDown(KeyCode.S))
        {
            NetworkManager.Singleton.StartServer();
            Debug.Log("Server started");
        }
    }
}
