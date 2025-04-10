using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonLauncher : MonoBehaviourPunCallbacks
{
    [Header("Room Settings")]
    public byte maxPlayersPerRoom = 4;
    public string roomName = "SampleRoom";

    // Called when the script instance is loaded.
    void Start()
    {
        // Connect to Photon using the settings from PhotonServerSettings
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Connecting to Photon...");
    }

    // Callback for when we are connected to the Photon Master Server.
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server");
        // Join a room, or create one if it doesn't exist
        RoomOptions roomOptions = new RoomOptions() { MaxPlayers = maxPlayersPerRoom };
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    // Callback for when we successfully join a room.
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room: " + PhotonNetwork.CurrentRoom.Name);
        // Instantiate the player prefab if you have one.
        // Ensure your Player prefab is located in a folder named "Resources".
        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
    }

    // Callback for when joining a room fails.
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Join room failed: " + message);
    }

    // Callback for when disconnected from Photon.
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning("Disconnected from Photon: " + cause.ToString());
    }
}
