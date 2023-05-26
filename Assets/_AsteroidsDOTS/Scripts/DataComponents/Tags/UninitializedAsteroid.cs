using Unity.Entities;
using Unity.Mathematics;

namespace _AsteroidsDOTS.Scripts.DataComponents.Tags
{
    public struct UninitializedAsteroid: IComponentData
    {
        public float3 IntendedDirection;
    }
}