using System.Collections.Generic;
using Fusion;
using Photon.Realtime;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public NetworkRunner runner; // Reference to the NetworkRunner
    public NetworkPrefabRef prefabRef; // Reference to the network prefab
    public int limit = 10;
    public Vector3 position;
    public Quaternion rotation;
    public GameObject parent;
    private Queue<NetworkObject> spawned = new Queue<NetworkObject>();

    public void Spawn()
    {
        if (runner == null)
        {
            Debug.LogError("NetworkRunner is not assigned!");
            return;
        }

        // Spawn a new networked object
        NetworkObject spawnedObject = runner.Spawn(prefabRef, transform.position + position, rotation);
        if(parent) 
        {
            spawnedObject.transform.SetParent(parent.transform);
        }
        spawned.Enqueue(spawnedObject);
        if (spawned.Count > limit)
        {
            // Remove the oldest object from the queue and despawn it
            NetworkObject oldestObject = spawned.Dequeue();
            runner.Despawn(oldestObject);
        }
    }

}