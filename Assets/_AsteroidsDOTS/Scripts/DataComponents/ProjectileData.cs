using Unity.Entities;
using UnityEngine;

namespace _AsteroidsDOTS.Scripts.DataComponents
{
    [GenerateAuthoringComponent]
    public struct ProjectileData : IComponentData
    {
        /// <summary>
        /// Speed property of the projectile
        /// </summary>
        public float Speed;

        /// <summary>
        /// Damage property for the projectile
        /// </summary>
        public float Damage;

        /// <summary>
        /// Intended max life time of this entity
        /// </summary>
        public float MaxLifetime;

        /// <summary>
        /// Current lifetime to clean up entity
        /// </summary>
        [HideInInspector] public float Lifetime;

        /// <summary>
        /// Is this entity ready to be disposed of?
        /// </summary>
        [HideInInspector]
        public bool ShouldBeDisposed => Lifetime <= 0;
    }
}