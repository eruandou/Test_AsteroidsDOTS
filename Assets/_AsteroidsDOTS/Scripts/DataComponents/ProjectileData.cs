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
    }
}