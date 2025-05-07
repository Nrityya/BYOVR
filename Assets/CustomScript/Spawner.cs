using System.Collections.Generic;
using Fusion;
using Photon.Realtime;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public NetworkRunner runner; // Reference to the NetworkRunner
    public NetworkPrefabRef prefabRef; // Reference to the network prefab
    public int limit = 10;
    private Queue<NetworkObject> spawned = new Queue<NetworkObject>();

    public void Spawn()
    {
        if (runner == null)
        {
            Debug.LogError("NetworkRunner is not assigned!");
            return;
        }

        // Spawn a new networked object
        NetworkObject spawnedObject = runner.Spawn(prefabRef, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);

        // Add the new object to the queue
        spawned.Enqueue(spawnedObject);

        // Check if the queue exceeds the limit
        if (spawned.Count > limit)
        {
            // Remove the oldest object from the queue and despawn it
            NetworkObject oldestObject = spawned.Dequeue();
            runner.Despawn(oldestObject);
        }
    }
}