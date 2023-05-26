using _AsteroidsDOTS.Scripts.Attributes;
using Unity.Entities;
using Unity.Mathematics;

namespace _AsteroidsDOTS.Scripts.DataComponents.Asteroids
{
    [GenerateAuthoringComponent]
    public struct InitialAsteroidSpawnData : IComponentData
    {
        public int2 MinMaxAmountOfInitialAsteroids;
        public float2 ExcludedMinMaxXLocations;
        public float2 ExcludedMinMaxZLocations;
        public Entity BigAsteroidPrefab;

        //We need to separate game data from authored data
    

        public float MedianExcludedXLocation => (ExcludedMinMaxXLocations.x + ExcludedMinMaxXLocations.y) / 2;
        public float MedianExcludedZLocation => (ExcludedMinMaxZLocations.x + ExcludedMinMaxZLocations.y) / 2;
    }
}