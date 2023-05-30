using System;
using _AsteroidsDOTS.Scripts.DataComponents;
using _AsteroidsDOTS.Scripts.DataComponents.GameState;
using _AsteroidsDOTS.Scripts.DataComponents.Powerups;
using _AsteroidsDOTS.Scripts.DataComponents.Tags;
using _AsteroidsDOTS.Scripts.Globals;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

namespace _AsteroidsDOTS.Scripts.Systems.Powerups
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class PowerUpSpawnSystem : SystemBase
    {
        private EndInitializationEntityCommandBufferSystem m_endInitializationBuffer;

        protected override void OnCreate()
        {
            m_endInitializationBuffer = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
            RequireSingletonForUpdate<PlayerTag>();
        }

        protected override void OnUpdate()
        {
            var l_ecb = m_endInitializationBuffer.CreateCommandBuffer();
            var l_currentTime = (float)Time.ElapsedTime;
            Entities.ForEach((ref GameStatePowerUpData p_gameStateData, ref IndividualRandomData p_randomData,
                in GameData p_gameData) =>
            {
                if (p_gameStateData.NextPowerUpSpawnTime > l_currentTime || p_gameStateData.PowerUpAlreadySpawned)
                    return;

                var l_randomNextTime =
                    p_randomData.Random.NextFloat(p_gameData.SpawnPowerUpTime.x, p_gameData.SpawnPowerUpTime.y);
                p_gameStateData.NextPowerUpSpawnTime = l_randomNextTime + l_currentTime;

                //Get spawn position
                float3 l_spawnPosition = GetRandomSpawnPosition(ref p_randomData.Random);

                //Get random powerup
                Entity l_selectedPowerUpEntity = GetRandomEnemyEntity(ref p_randomData.Random, in p_gameData);

                var l_powerup = l_ecb.Instantiate(l_selectedPowerUpEntity);
                l_ecb.SetComponent(l_powerup, new Translation() { Value = l_spawnPosition });
                p_gameStateData.PowerUpAlreadySpawned = true;
            }).Schedule();

            m_endInitializationBuffer.AddJobHandleForProducer(Dependency);
        }

        private static Entity GetRandomEnemyEntity(ref Random p_randomDataRandom, in GameData p_gameData)
        {
            var l_nextEnemyInt = p_randomDataRandom.NextInt(0, p_gameData.PowerUpEnumAmount);
            var l_selectedEnemy = (PowerUpType)l_nextEnemyInt;
            Entity l_selectedEnemyEntity = l_selectedEnemy switch
            {
                PowerUpType.DoublePoints => p_gameData.DoublePointsPU,
                PowerUpType.DoubleShot => p_gameData.DoubleShotPU,
                PowerUpType.RecoverHealth => p_gameData.HealthPU,
                PowerUpType.InvulnerabilityShield => p_gameData.InvulnerablePU,
                PowerUpType.SuperBomb => p_gameData.SuperBombPU,
                _ => throw new ArgumentOutOfRangeException()
            };
            return l_selectedEnemyEntity;
        }

        private static float3 GetRandomSpawnPosition(ref Random p_random)
        {
            var l_randomXPosition = p_random.NextFloat(GameplayStaticGlobals.HorizontalLimits.x,
                GameplayStaticGlobals.HorizontalLimits.y);
            var l_randomZPosition = p_random.NextFloat(GameplayStaticGlobals.VerticalLimits.x,
                GameplayStaticGlobals.VerticalLimits.y);
            return new float3(l_randomXPosition, 0, l_randomZPosition);
        }
    }
}