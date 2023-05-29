using _AsteroidsDOTS.Scripts.DataComponents;
using Unity.Burst;
using Unity.Entities;

namespace _AsteroidsDOTS.Scripts.Systems
{
    public struct RespawnPlayerJob : IJobEntityBatch
    {
        public ComponentTypeHandle<RespawnPlayerData> PlayerNeedsRespawnHandle;
        public Entity PlayerEntity;
        public EntityCommandBuffer Buffer;
        public Entity GameDataEntity;
        public float DeltaTime;

        [BurstCompile]
        public void Execute(ArchetypeChunk batchInChunk, int batchIndex)
        {
            var l_playerNeedsRespawnArray = batchInChunk.GetNativeArray(PlayerNeedsRespawnHandle);

            for (int i = 0; i < batchInChunk.Count; i++)
            {
                var l_playerRespawnData = l_playerNeedsRespawnArray[i];
                l_playerRespawnData.RespawnTimeLeft -= DeltaTime;
                if (l_playerRespawnData.RespawnPlayer)
                {
                    Buffer.Instantiate(PlayerEntity);
                    Buffer.RemoveComponent<RespawnPlayerData>(GameDataEntity);
                    continue;
                }

                //Write data back
                l_playerNeedsRespawnArray[i] = l_playerRespawnData;
            }
        }
    }

    public class RespawnSystem : SystemBase
    {
        private EndInitializationEntityCommandBufferSystem m_endInitializationBuffer;

        private EntityQueryDesc m_queryDesc;

        protected override void OnCreate()
        {
            m_endInitializationBuffer = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
            m_queryDesc = new EntityQueryDesc()
            {
                All = new[]
                {
                    ComponentType.ReadOnly<RespawnPlayerData>()
                }
            };
            RequireForUpdate(GetEntityQuery(m_queryDesc));
        }


        protected override void OnUpdate()
        {
            var l_gameData = GetSingleton<GameData>();
            var l_gameDataEntity = GetSingletonEntity<GameData>();
            var l_respawnPlayerJob = new RespawnPlayerJob()
            {
                PlayerEntity = l_gameData.PlayerShip,
                PlayerNeedsRespawnHandle = GetComponentTypeHandle<RespawnPlayerData>(false),
                Buffer = m_endInitializationBuffer.CreateCommandBuffer(),
                GameDataEntity = l_gameDataEntity,
                DeltaTime = Time.DeltaTime
            };


            Dependency = l_respawnPlayerJob.Schedule(GetEntityQuery(m_queryDesc), Dependency);

            m_endInitializationBuffer.AddJobHandleForProducer(Dependency);
        }
    }
}