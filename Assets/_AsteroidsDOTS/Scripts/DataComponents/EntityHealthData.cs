using Unity.Entities;
using UnityEngine;

namespace _AsteroidsDOTS.Scripts.DataComponents
{
    [GenerateAuthoringComponent]
    public struct EntityHealthData : IComponentData
    {
        public Entity DeadParticlesPrefab;
        public float Health;
        public float MaxHealth;
        public float InvincibilityTime;
        [HideInInspector] public float CurrentInvincibilityTime;
        [HideInInspector] public bool IsInvincible => CurrentInvincibilityTime <= 0;

        public bool ShouldDie => Health <= 0;

        /// <summary>
        /// Used to queue pending health modifications to this component
        /// </summary>
        [HideInInspector] public float PendingHealthModification;
    }
}