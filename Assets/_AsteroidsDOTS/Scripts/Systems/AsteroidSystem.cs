using _AsteroidsDOTS.Scripts.DataComponents;
using _AsteroidsDOTS.Scripts.DataComponents.Asteroids;
using _AsteroidsDOTS.Scripts.DataComponents.Audio;
using _AsteroidsDOTS.Scripts.DataComponents.GameState;
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

        private Entity m_gameStateDataEntity;


        protected override void OnCreate()
        {
            m_endSimulationBuffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            RequireSingletonForUpdate<GameStateDataAsteroids>();
        }

        protected override void OnStartRunning()
        {
            m_gameStateDataEntity = GetSingletonEntity<GameStateDataAsteroids>();
        }

        protected override void OnUpdate()
        {
            var l_ecb = m_endSimulationBuffer.CreateCommandBuffer();

            var l_currentGameState = GetSingleton<GameStateDataAsteroids>();
            var l_gameStateEntity = m_gameStateDataEntity;

            Entities.WithNone<DeadPointsEntityTag>().ForEach((Entity p_entity, ref IndividualRandomData p_randomData,
                in EntityHealthData p_asteroidHealth,
                in AsteroidData p_asteroidData,
                in LocalToWorld p_localToWorld, in DestructionSoundData p_destructionSoundData) =>
            {
                if (!p_asteroidHealth.ShouldDie) return;

                for (int i = 0; i < p_asteroidData.PiecesBrokenIntoOnDestroy; i++)
                {
                    Entity l_asteroidEntity = l_ecb.Instantiate(p_asteroidData.SmallerAsteroidsToSpawn);

                    var l_newAsteroidTranslation = new Translation() { Value = p_localToWorld.Position };
                    l_ecb.SetComponent(l_asteroidEntity, l_newAsteroidTranslation);
                    float2 l_randomDirVector = p_randomData.Random.NextFloat2Direction();
                    float3 l_randomMovingDirection = new float3(l_randomDirVector.x, 0, l_randomDirVector.y);
                    UninitializedAsteroid l_uninitializedAsteroidTag = new UninitializedAsteroid
                        { IntendedDirection = l_randomMovingDirection };
                    l_ecb.AddComponent(l_asteroidEntity, l_uninitializedAsteroidTag);
                }
                
                l_currentGameState.TotalSpawnedAsteroids += p_asteroidData.PiecesBrokenIntoOnDestroy - 1;
                l_ecb.AddComponent<DeadPointsEntityTag>(p_entity);
                l_ecb.SetComponent(l_gameStateEntity, l_currentGameState);
                var l_deathSound = new AudioPetition()
                {
                    AudioID = p_destructionSoundData.DestructionSound,
                    Volume = p_destructionSoundData.Volume,
                    ShouldLoop = false
                };
                l_ecb.AddComponent(p_entity, l_deathSound);
            }).Schedule();

            m_endSimulationBuffer.AddJobHandleForProducer(Dependency);
        }
    }
}