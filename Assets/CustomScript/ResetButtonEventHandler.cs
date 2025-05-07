using UnityEngine;

public class ResetButtonEventHandler : MonoBehaviour
{
    public GameObject target;

    public void ResetCall()
    {
        target.BroadcastMessage("RpcReset", SendMessageOptions.DontRequireReceiver);
    }

}