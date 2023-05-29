using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _AsteroidsDOTS.Scripts.DataComponents
{
    [GenerateAuthoringComponent]
    public struct GameData : IComponentData
    {
        [Header("Enemies")] public float2 SpawnEnemyTime;
        public Entity SmallUfo;
        public Entity BigUfo;
        public int EnemyEnumAmount;
        [Header("Powerups")]
        public Entity HealthPU;
        public Entity DoublePointsPU;
        public Entity InvulnerablePU;
        public Entity SuperBombPU;
        public Entity DoubleShotPU;
        public float2 SpawnPowerUpTime;
        public int PowerUpEnumAmount;

        [Header("Others)")] public float PlayerRespawnTime;
        [Header("Prefabs")] public Entity ShieldEntity;
        public Entity PlayerShip;
        
        
        
    }
}