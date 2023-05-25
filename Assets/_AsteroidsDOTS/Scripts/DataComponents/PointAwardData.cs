using Unity.Entities;

namespace _AsteroidsDOTS.Scripts.DataComponents
{
    [GenerateAuthoringComponent]
    public struct PointAwardData: IComponentData
    {
        public int PointsToGiveOut;
    }
}