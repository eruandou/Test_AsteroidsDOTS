using Unity.Entities;

namespace _AsteroidsDOTS.Scripts.DataComponents
{
    [GenerateAuthoringComponent]
    public struct PointsCounterData : IComponentData
    {
        public float CurrentPoints;
    }
}