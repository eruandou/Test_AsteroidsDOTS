using System;
using _AsteroidsDOTS.Scripts.DataComponents;
using _AsteroidsDOTS.Scripts.DataComponents.Asteroids;
using _AsteroidsDOTS.Scripts.DataComponents.GameState;
using _AsteroidsDOTS.Scripts.DataComponents.Tags;
using _AsteroidsDOTS.Scripts.Globals;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace _AsteroidsDOTS.Scripts.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class SpawnAsteroidsSystem : SystemBase
    {
        private EndInitializationEntityCommandBufferSystem m_endInitializationBuffer;

        protected override void OnCreate()
        {
            m_endInitializationBuffer = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
        }

        protected override void OnStartRunning()
        {
            uint l_dateTimeSeed = (uint)DateTime.UtcNow.Millisecond;
            Entities.WithAll<InitialAsteroidSpawnData>()
                .ForEach((ref IndividualRandomData p_randomData) => { p_randomData.Random.InitState(l_dateTimeSeed); })
                .Schedule();
        }

        protected override void OnUpdate()
        {
            if (!AsteroidsDOTS.GameIsStarted)
                return;

            var l_ecb = m_endInitializationBuffer.CreateCommandBuffer();
            var l_horizontalLimit = GameplayStaticGlobals.HorizontalLimits;
            var l_verticalLimit = GameplayStaticGlobals.VerticalLimits;


            Entities.ForEach((ref IndividualRandomData p_randomData, ref GameStateDataAsteroids p_gameStateData,
                in InitialAsteroidSpawnData p_spawnData) =>
            {
                if (p_gameStateData.TotalSpawnedAsteroids > 0)
                    return;
                //Note: If not used, the first integer is predictable, fsr.
                p_randomData.Random.NextInt();

                var l_randomAmount = p_randomData.Random.NextInt(p_spawnData.MinMaxAmountOfInitialAsteroids.x,
                    p_spawnData.MinMaxAmountOfInitialAsteroids.y);

                for (int i = 0; i < l_randomAmount; i++)
                {
                    Entity l_newAsteroidEntity = l_ecb.Instantiate(p_spawnData.BigAsteroidPrefab);


                    float l_maxXSafePosition = p_spawnData.ExcludedMinMaxXLocations.y;
                    float l_minXSafePosition = p_spawnData.ExcludedMinMaxXLocations.x;
                    float l_medianXSafeLocation = p_spawnData.MedianExcludedXLocation;

                    float l_maxZSafePosition = p_spawnData.ExcludedMinMaxZLocations.y;
                    float l_minZSafePosition = p_spawnData.ExcludedMinMaxZLocations.x;
                    float l_medianZSafeLocation = p_spawnData.MedianExcludedZLocation;

                    float2 l_randomDir = p_randomData.Random.NextFloat2Direction();
                    float3 l_randomDirection = new float3(l_randomDir.x, 0, l_randomDir.y);
                    UninitializedAsteroid l_uninitializedAsteroidData = new UninitializedAsteroid
                        { IntendedDirection = l_randomDirection };

                    l_ecb.AddComponent(l_newAsteroidEntity, l_uninitializedAsteroidData);

                    var l_randomNewPositionX = p_randomData.Random.NextFloat(l_horizontalLimit.x,
                        l_horizontalLimit.y);


                    //We are spawning in the safe zone! We correct the asteroids position
                    if (l_randomNewPositionX < l_maxXSafePosition && l_randomNewPositionX > l_minXSafePosition)
                    {
                        var l_isToRightOfSafeZone =
                            l_randomNewPositionX > l_medianXSafeLocation;
                        l_randomNewPositionX +=
                            l_isToRightOfSafeZone ? l_medianXSafeLocation : -l_medianXSafeLocation;
                    }

                    var l_randomPositionZ = p_randomData.Random.NextFloat(l_verticalLimit.x,
                        l_verticalLimit.y);

                    //We are spawning in the safe zone! We correct the asteroids position
                    if (l_randomPositionZ < l_maxZSafePosition && l_randomPositionZ > l_minZSafePosition)
                    {
                        var l_isUpFromSafeZone =
                            l_randomPositionZ > l_medianZSafeLocation;
                        l_randomPositionZ +=
                            l_isUpFromSafeZone ? l_medianZSafeLocation : -l_medianZSafeLocation;
                    }

                    var l_spawnLocation = new float3(l_randomNewPositionX, 0, l_randomPositionZ);
                    var l_translation = new Translation() { Value = l_spawnLocation };
                    l_ecb.SetComponent(l_newAsteroidEntity, l_translation);
                }

                p_gameStateData.TotalSpawnedAsteroids += l_randomAmount;
            }).Schedule();


            m_endInitializationBuffer.AddJobHandleForProducer(Dependency);
        }
    }
}