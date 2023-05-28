using _AsteroidsDOTS.Scripts.Attributes;
using Unity.Entities;

namespace _AsteroidsDOTS.Scripts.DataComponents
{
    [GenerateAuthoringComponent]
    public struct GameStateData : IComponentData
    {
        [ReadOnlyInspector] public int SpawnedUfo;
        [ReadOnlyInspector] public int TotalSpawnedAsteroids;
        [ReadOnlyInspector] public float NextEnemySpawnTime;
        [ReadOnlyInspector] public float NextPowerUpSpawnTime;
        [ReadOnlyInspector] public bool PowerUpAlreadySpawned;
        public float PointsMultiplier;
        [ReadOnlyInspector] public float CurrentPoints;
    }
}