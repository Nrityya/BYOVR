using UnityEngine;
using System;

public class ResetButtonEventHandler : MonoBehaviour
{
    public static event Action resetCups1, resetCups2, resetPool;

    public static void ResetCups1()
    {
        resetCups1?.Invoke();
    }
    public static void ResetCups2()
    {
        resetCups2?.Invoke();
    }
    public static void ResetPool()
    {
        resetPool?.Invoke();
    }
}
