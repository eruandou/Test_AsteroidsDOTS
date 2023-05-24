using Unity.Entities;
using UnityEngine;

namespace _AsteroidsDOTS.Scripts.DataComponents
{
    [GenerateAuthoringComponent]
    public struct ProjectileData : IComponentData
    {
        public float Speed;
        public float Damage;
        
        /// <summary>
        /// Intended max life time of this entity
        /// </summary>
        public float MaxLifetime;
        /// <summary>
        /// Current lifetime 
        /// </summary>
        [HideInInspector] public float CurrentLifetime;
        
        /// <summary>
        /// Is this entity ready to be disposed of?
        /// </summary>
        public bool ShouldBeKilled => CurrentLifetime >= MaxLifetime;
    }
}