using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace _AsteroidsDOTS.Scripts.DataComponents
{
    public enum EnemyTypes
    {
        SmallUfo,
        BigUfo
    }

    [GenerateAuthoringComponent]
    public struct GameData : IComponentData
    {
        [Header("Enemies")] public float2 SpawnEnemyTime;
        [HideInInspector] public float NextEnemySpawnTime;
        public Entity SmallUfo;
        public Entity BigUfo;
    }
}