using Unity.Entities;

namespace _AsteroidsDOTS.Scripts.DataComponents.GameState
{
    public struct GameStatePowerUpData : IComponentData
    {
        public float NextPowerUpSpawnTime;
        public bool PowerUpAlreadySpawned;
    }
}