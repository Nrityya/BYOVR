using System.Collections.Generic;
using Fusion;
using Photon.Realtime;
using UnityEngine;

public class Spawner : NetworkBehaviour
{
    public NetworkPrefabRef prefabRef; // Reference to the network prefab
    public int limit = 10;
    public Vector3 position;
    public Quaternion rotation;
    public GameObject parent;
    private Queue<NetworkObject> spawned = new Queue<NetworkObject>();

    public void Spawn()
    {
        RpcSpawn();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RpcSpawn()
    {
        // Spawn a new networked object
        NetworkObject spawnedObject = Runner.Spawn(prefabRef, transform.position + position, rotation);
        if (parent)
        {
            spawnedObject.transform.SetParent(parent.transform);
        }
        spawned.Enqueue(spawnedObject);
        if (spawned.Count > limit)
        {
            // Remove the oldest object from the queue and despawn it
            NetworkObject oldestObject = spawned.Dequeue();
            Runner.Despawn(oldestObject);
        }
    }
}