using Unity.Entities;

namespace _AsteroidsDOTS.Scripts.DataComponents.GameState
{
    public struct GameStateDataPlayer : IComponentData
    {
        private const int PointsPerLife = 10000;
        public int CurrentPoints;
        public int CurrentPlayerLives;
        public float PointsMultiplier;
        public bool PlayerLost => CurrentPlayerLives <= 0;
        public int LifesObtained;
        public bool PlayerShouldGetLife => CurrentPoints > PointsPerLife + PointsPerLife * LifesObtained;
    }
}