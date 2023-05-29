using System;
using _AsteroidsDOTS.Scripts.DataComponents;
using _AsteroidsDOTS.Scripts.DataComponents.Tags;
using _AsteroidsDOTS.Scripts.Enums;
using _AsteroidsDOTS.Scripts.Globals;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

namespace _AsteroidsDOTS.Scripts.Systems.Enemy
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class EnemySpawnSystem : SystemBase
    {
        private EndInitializationEntityCommandBufferSystem m_endInitializationBuffer;

        protected override void OnCreate()
        {
            m_endInitializationBuffer = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
        }


        //TODO: Jobify this for added B U R S T  C O M P I L I N G 
        protected override void OnUpdate()
        {
            var l_currentTime = (float)Time.ElapsedTime;
            var l_ecb = m_endInitializationBuffer.CreateCommandBuffer();

            Entities.ForEach((ref GameStateData p_gameStateData, ref IndividualRandomData p_randomData,
                in GameData p_gameData) =>
            {
                if (p_gameStateData.NextEnemySpawnTime > l_currentTime || p_gameStateData.SpawnedUfo > 2)
                    return;

                var l_randomNextTime =
                    p_randomData.Random.NextFloat(p_gameData.SpawnEnemyTime.x, p_gameData.SpawnEnemyTime.y);
                p_gameStateData.NextEnemySpawnTime = l_randomNextTime + l_currentTime;

                //Get spawn position
                float3 l_spawnPosition = GetRandomEnemySpawnPosition(ref p_randomData.Random);

                //Get random direction
                float3 l_normalizedDirection = GetRandomMovingDirection(ref p_randomData.Random);

                //Get random enemy

                Entity l_selectedEnemyEntity = GetRandomEnemyEntity(ref p_randomData.Random, in p_gameData);

                var l_enemy = l_ecb.Instantiate(l_selectedEnemyEntity);
                UninitializedUFOTag l_uninitializedUfoTag = new UninitializedUFOTag()
                    { IntendedDirection = l_normalizedDirection };
                l_ecb.AddComponent(l_enemy, l_uninitializedUfoTag);
                l_ecb.SetComponent(l_enemy, new Translation() { Value = l_spawnPosition });
                p_gameStateData.SpawnedUfo++;
            }).Schedule();


            m_endInitializationBuffer.AddJobHandleForProducer(Dependency);
        }

        private static Entity GetRandomEnemyEntity(ref Random p_randomDataRandom, in GameData p_gameData)
        {
            var l_nextEnemyInt = p_randomDataRandom.NextInt(0, p_gameData.EnemyEnumAmount);
            var l_selectedEnemy = (EnemyTypes)l_nextEnemyInt;
            Entity l_selectedEnemyEntity = l_selectedEnemy switch
            {
                EnemyTypes.SmallUfo => p_gameData.SmallUfo,
                EnemyTypes.BigUfo => p_gameData.BigUfo,
                _ => throw new ArgumentOutOfRangeException()
            };
            return l_selectedEnemyEntity;
        }

        private static float3 GetRandomMovingDirection(ref Random p_randomDataRandom)
        {
            var l_randomDir = p_randomDataRandom.NextFloat2(float2.zero, 1);
            var l_randomDirectionX = l_randomDir.x;
            var l_randomDirectionZ = l_randomDir.y;

            if (l_randomDirectionX == 0 && l_randomDirectionZ == 0)
            {
                l_randomDirectionX = 1;
            }

            var l_direction = new float3(l_randomDirectionX, 0, l_randomDirectionZ);
            return math.normalize(l_direction);
        }

        private static float3 GetRandomEnemySpawnPosition(ref Random p_random)
        {
            var l_spawnOnLeft = p_random.NextBool();
            var l_randomXPosition = l_spawnOnLeft
                ? GameplayStaticGlobals.HorizontalLimits.x
                : GameplayStaticGlobals.HorizontalLimits.y;
            var l_randomZPosition = p_random.NextFloat(GameplayStaticGlobals.VerticalLimits.x,
                GameplayStaticGlobals.VerticalLimits.y);
            return new float3(l_randomXPosition, 0, l_randomZPosition);
        }
    }
}