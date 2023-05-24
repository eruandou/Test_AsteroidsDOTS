using Unity.Entities;
using UnityEngine;

namespace _AsteroidsDOTS.Scripts.DataComponents
{
    [GenerateAuthoringComponent]
    public struct EntityHealthData : IComponentData
    {
        [Header("Health")] public float MaxHealth;
        public Entity DeadParticlesPrefab;
        public float CurrentHealth;
        [HideInInspector] public bool IsInvincible;
        
    }
}