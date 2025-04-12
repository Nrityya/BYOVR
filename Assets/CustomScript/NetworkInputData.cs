using UnityEngine;
using Fusion;

public struct NetworkInputData : INetworkInput
{
    public Vector3 moveDirection;
    public Vector3 lookDirection;

    public NetworkId controlledObjectId;
    public ControlledObjectState controlledObjectState;
}

public struct ControlledObjectState : INetworkStruct
{
    public NetworkBool magic;
    public Vector3 targetPosition;
    public float axisX;
    public float axisY;
    public float releaseForce;
}