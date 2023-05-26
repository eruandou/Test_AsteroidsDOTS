using _AsteroidsDOTS.Scripts.DataComponents;
using _AsteroidsDOTS.Scripts.DataComponents.Asteroids;
using _AsteroidsDOTS.Scripts.DataComponents.Tags;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace _AsteroidsDOTS.Scripts.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class AsteroidSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem m_endSimulationBuffer;

        private Entity m_asteroidSpawnDataEntity;


        protected override void OnCreate()
        {
            m_endSimulationBuffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            if (m_asteroidSpawnDataEntity == Entity.Null)
            {
                m_asteroidSpawnDataEntity = GetSingletonEntity<InitialAsteroidSpawnData>();
            }

            if (!EntityManager.Exists(m_asteroidSpawnDataEntity))
                return;

            var l_ecb = m_endSimulationBuffer.CreateCommandBuffer();


            Entities.ForEach((Entity p_entity, ref IndividualRandomData p_randomData,
                in EntityHealthData p_asteroidHealth,
                in AsteroidData p_asteroidData,
                in LocalToWorld p_localToWorld) =>
            {
                if (p_asteroidHealth.ShouldDie)
                {
                    for (int i = 0; i < p_asteroidData.PiecesBrokenIntoOnDestroy; i++)
                    {
                        Entity l_asteroidEntity = l_ecb.Instantiate(p_asteroidData.SmallerAsteroidsToSpawn);

                        var l_newAsteroidTranslation = new Translation() { Value = p_localToWorld.Position };
                        l_ecb.SetComponent(l_asteroidEntity, l_newAsteroidTranslation);

                        var l_randomMovingDirection = p_randomData.Random.NextFloat3();
                        l_randomMovingDirection.y = 0;
                        l_randomMovingDirection = math.normalize(l_randomMovingDirection);
                        var l_uninitializedAsteroidTag = new UninitializedAsteroid()
                            { IntendedDirection = l_randomMovingDirection };
                        l_ecb.AddComponent<UninitializedAsteroid>(l_asteroidEntity);
                        l_ecb.SetComponent(l_asteroidEntity, l_uninitializedAsteroidTag);
                    }

                    if (EntityManager.Exists(m_asteroidSpawnDataEntity))
                    {
                        int l_asteroidBalance = 0;
                        l_asteroidBalance--;
                        l_asteroidBalance += p_asteroidData.PiecesBrokenIntoOnDestroy;
                        InitialAsteroidSpawnData l_spawnAsteroidData =
                            GetComponent<InitialAsteroidSpawnData>(m_asteroidSpawnDataEntity);
                        l_spawnAsteroidData.TotalSpawnedAsteroids += l_asteroidBalance;
                        l_ecb.SetComponent(m_asteroidSpawnDataEntity, l_spawnAsteroidData);
                    }

                    l_ecb.AddComponent<DeadPointsEntityTag>(p_entity);
                }
            }).WithoutBurst().Run();
            m_endSimulationBuffer.AddJobHandleForProducer(Dependency);
        }
    }
}