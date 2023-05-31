using Unity.Entities;

namespace _AsteroidsDOTS.Scripts.DataComponents.Enemies
{
    [GenerateAuthoringComponent]
    public struct EnemyMovementData: IComponentData
    {
        public float MovementSpeed;
        public bool FollowPlayer;
        public float FollowMinDistance;
    }
}