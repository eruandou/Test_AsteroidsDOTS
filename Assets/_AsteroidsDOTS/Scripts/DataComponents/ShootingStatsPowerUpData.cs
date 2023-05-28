using Unity.Entities;

namespace _AsteroidsDOTS.Scripts.DataComponents
{
    [GenerateAuthoringComponent]
    public struct ShootingStatsPowerUpData : IComponentData
    {
        public float BulletsToShoot;
        public float BulletAngleDifference;
    }
}