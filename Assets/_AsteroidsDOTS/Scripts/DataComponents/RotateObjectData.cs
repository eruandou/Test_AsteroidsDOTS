using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct RotateObjectData : IComponentData
{
    public float RotateSpeed;
}