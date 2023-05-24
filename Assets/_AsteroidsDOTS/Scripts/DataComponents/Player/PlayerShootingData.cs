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

        [HideInInspector] public float LastShootingTime;

        [HideInInspector] public bool CanShoot => LastShootingTime <= Time.time;
    }
}