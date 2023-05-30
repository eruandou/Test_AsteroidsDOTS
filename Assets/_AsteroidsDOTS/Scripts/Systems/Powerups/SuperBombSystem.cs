using _AsteroidsDOTS.Scripts.DataComponents;
using _AsteroidsDOTS.Scripts.DataComponents.GameState;
using _AsteroidsDOTS.Scripts.DataComponents.Powerups;
using _AsteroidsDOTS.Scripts.DataComponents.Tags;
using Unity.Burst;
using Unity.Entities;

namespace _AsteroidsDOTS.Scripts.Systems.Powerups
{
    public struct KillEverythingOnScreenJob : IJobEntityBatch
    {
        public ComponentTypeHandle<EntityHealthData> HealthDataHandle;
        public EntityCommandBuffer Buffer;
        public Entity GameStateDataEntity;

        [BurstCompile]
        public void Execute(ArchetypeChunk batchInChunk, int batchIndex)
        {
            var l_healthDataArray = batchInChunk.GetNativeArray(HealthDataHandle);

            for (int i = 0; i < batchInChunk.Count; i++)
            {
                var l_killable = l_healthDataArray[i];
                l_killable.PendingHealthModification -= l_killable.Health;

                //Write necessary data back to components

                l_healthDataArray[i] = l_killable;
            }

            Buffer.RemoveComponent<SuperBombPowerUpData>(GameStateDataEntity);
        }
    }

    public class SuperBombSystem : SystemBase
    {
        private EndInitializationEntityCommandBufferSystem m_endInitilizationBuffer;

        private EntityQueryDesc m_killableEntityQueryDesc;

        protected override void OnCreate()
        {
            m_endInitilizationBuffer = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
            RequireSingletonForUpdate<SuperBombPowerUpData>();
            m_killableEntityQueryDesc = new EntityQueryDesc()
            {
                All = new[]
                {
                    ComponentType.ReadWrite<EntityHealthData>()
                },
                None = new[]
                {
                    ComponentType.ReadOnly<PlayerTag>(),
                }
            };
        }

        protected override void OnUpdate()
        {
            var l_killablesQuery = GetEntityQuery(m_killableEntityQueryDesc);
            var l_killEverythingJob = new KillEverythingOnScreenJob()
            {
                Buffer = m_endInitilizationBuffer.CreateCommandBuffer(),
                HealthDataHandle = GetComponentTypeHandle<EntityHealthData>(false),
                GameStateDataEntity = GetSingletonEntity<GameStatePowerUpData>()
            };

            Dependency = l_killEverythingJob.Schedule(l_killablesQuery, Dependency);
            m_endInitilizationBuffer.AddJobHandleForProducer(Dependency);
        }
    }
}