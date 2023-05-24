using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct PlayerMovementData : IComponentData
{
    /// <summary>
    /// Used to move the player. Used by move system, set by Input
    /// </summary>
    [HideInInspector] public float ThrustImpulse;

    /// <summary>
    /// Used to rotate the player. Used by move system, set by Input
    /// </summary>
    [HideInInspector] public float TorqueForce;


    [Header("Movement")] public float ThrustAccelerationRate;
    public float RotationAcceleration;
    public float MaxAngularSpeed;
    public float MaxSpeed;
}