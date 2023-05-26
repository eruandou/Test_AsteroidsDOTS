using System;
using _AsteroidsDOTS.Scripts.DataComponents;
using _AsteroidsDOTS.Scripts.DataComponents.Tags;
using _AsteroidsDOTS.Scripts.Globals;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace _AsteroidsDOTS.Scripts.Systems.Enemy
{
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
    public class EnemySpawnSystem : SystemBase
    {
        private EndInitializationEntityCommandBufferSystem m_endInitializationBuffer;
        private GameData m_gameData;

        protected override void OnCreate()
        {
            m_endInitializationBuffer = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
        }

        protected override void OnStartRunning()
        {
            RequireSingletonForUpdate<GameData>();
            m_gameData = GetSingleton<GameData>();
        }

        protected override void OnUpdate()
        {
            var l_currentTime = (float)Time.ElapsedTime;
            var l_ecb = m_endInitializationBuffer.CreateCommandBuffer();

            if (m_gameData.NextEnemySpawnTime <= l_currentTime)
            {
                m_gameData.NextEnemySpawnTime = GetNextEnemyTime() + l_currentTime;
                SpawnEnemy(ref l_ecb);
            }

            CompleteDependency();
        }

        private float GetNextEnemyTime()
        {
            var l_randomNextTime = UnityEngine.Random.Range(m_gameData.SpawnEnemyTime.x, m_gameData.SpawnEnemyTime.y);
            return l_randomNextTime;
        }

        private float3 GetSpawnPosition()
        {
            var l_spawnOnLeft = UnityEngine.Random.value >= 0.5f;
            var l_randomXPosition = l_spawnOnLeft
                ? GameplayStaticGlobals.HorizontalLimits.x
                : GameplayStaticGlobals.HorizontalLimits.y;
            var l_randomZPosition = UnityEngine.Random.Range(GameplayStaticGlobals.VerticalLimits.x,
                GameplayStaticGlobals.VerticalLimits.y);
            return new float3(l_randomXPosition, 0, l_randomZPosition);
        }

        private float3 GetRandomDirection()
        {
            var l_randomDirectionX = UnityEngine.Random.value;
            var l_randomDirectionZ = UnityEngine.Random.value;

            if (l_randomDirectionX == 0 && l_randomDirectionZ == 0)
            {
                l_randomDirectionX = 1;
            }

            var l_direction = new float3(l_randomDirectionX, 0, l_randomDirectionZ);
            var l_normalizedDirection = math.normalize(l_direction);
            return l_normalizedDirection;
        }


        private void SpawnEnemy(ref EntityCommandBuffer p_entityBuffer)
        {
            float3 l_randomPosition = GetSpawnPosition();
            float3 l_direction = GetRandomDirection();
            var l_enumAmount = typeof(EnemyTypes).GetEnumValues();
            var l_nextEnemyInt = UnityEngine.Random.Range(0, l_enumAmount.Length);
            var l_selectedEnemy = (EnemyTypes)l_enumAmount.GetValue(l_nextEnemyInt);
            Entity l_selectedEnemyEntity = l_selectedEnemy switch
            {
                EnemyTypes.SmallUfo => m_gameData.SmallUfo,
                EnemyTypes.BigUfo => m_gameData.BigUfo,
                _ => throw new ArgumentOutOfRangeException()
            };

            var l_enemy = p_entityBuffer.Instantiate(l_selectedEnemyEntity);
            p_entityBuffer.AddComponent<UninitializedUFOTag>(l_enemy);
            var l_uninitializedUfoTag = new UninitializedUFOTag() { IntendedDirection = l_direction };
            p_entityBuffer.SetComponent(l_enemy, l_uninitializedUfoTag);
            var l_enemyTranslation = new Translation() { Value = l_randomPosition };
            p_entityBuffer.SetComponent(l_enemy, l_enemyTranslation);
        }
    }
}