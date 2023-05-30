using Unity.Entities;

namespace _AsteroidsDOTS.Scripts.DataComponents.GameState
{
    public struct GameStateDataUfo : IComponentData
    {
        public int SpawnedUfo;
        public float NextEnemySpawnTime;
    }
}