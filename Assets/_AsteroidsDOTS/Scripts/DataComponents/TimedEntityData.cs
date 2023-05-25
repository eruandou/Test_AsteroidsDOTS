using Unity.Entities;

namespace _AsteroidsDOTS.Scripts.DataComponents
{
    [GenerateAuthoringComponent]
    public struct TimedEntityData : IComponentData
    {
        public float Lifetime;

        public bool ShouldBeKilled => Lifetime <= 0;
    }
}