using UnityEngine;

public class ResetButtonEventHandler : MonoBehaviour
{
    public GameObject target;

    public void ResetCall()
    {
        target.BroadcastMessage("Reset", SendMessageOptions.DontRequireReceiver);
    }

}