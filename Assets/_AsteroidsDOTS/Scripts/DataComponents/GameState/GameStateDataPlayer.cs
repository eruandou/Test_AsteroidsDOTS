using Unity.Entities;

namespace _AsteroidsDOTS.Scripts.DataComponents.GameState
{
    public struct GameStateDataPlayer : IComponentData
    {
        public int CurrentPoints;
        public int CurrentPlayerLives;
        public float PointsMultiplier;
        public bool PlayerLost => CurrentPlayerLives <= 0;
    }
}