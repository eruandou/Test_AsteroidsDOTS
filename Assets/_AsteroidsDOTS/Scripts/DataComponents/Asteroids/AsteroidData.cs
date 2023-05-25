using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _AsteroidsDOTS.Scripts.DataComponents.Asteroids
{
    [GenerateAuthoringComponent]
    public struct AsteroidData : IComponentData
    {
        [Header("Asteroid Death")] public Entity SmallerAsteroidsToSpawn;
        public int PiecesBrokenIntoOnDestroy;

        [Header("Fixed data")] public float DamageOnContact;
        public float2 RandomMinMaxVelocity;
    }
}