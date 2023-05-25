using Unity.Entities;
using Unity.Mathematics;

namespace _AsteroidsDOTS.Scripts.DataComponents.Tags
{
    public struct UninitializedProjectileTag : IComponentData
    {
        public float3 IntendedForwards;
    }
}