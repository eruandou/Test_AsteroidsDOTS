using Unity.Entities;
using UnityEngine;

namespace _AsteroidsDOTS.Scripts.DataComponents
{
    [GenerateAuthoringComponent]
    public struct PlayerShootingData : IComponentData
    {
        [Header("Shooting info")] public float ShootRate;
        public Entity ProjectilePrefab;
        public float ProjectileSpawnForwardOffset;
        [HideInInspector] public bool ShouldShootProjectile;
        private float MinimumShootTime => 1f / ShootRate;
        [HideInInspector] public float LastShootingTime;
        public float NextShootingTime => LastShootingTime + MinimumShootTime;
    }
}