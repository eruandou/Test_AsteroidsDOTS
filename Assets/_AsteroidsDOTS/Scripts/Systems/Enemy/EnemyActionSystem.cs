using _AsteroidsDOTS.Scripts.DataComponents;
using _AsteroidsDOTS.Scripts.DataComponents.Audio;
using _AsteroidsDOTS.Scripts.DataComponents.Tags;
using _AsteroidsDOTS.Scripts.DevelopmentUtilities;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace _AsteroidsDOTS.Scripts.Systems.Enemy
{
    public struct EnemyActionJob : IJobEntityBatch
    {
        public ComponentTypeHandle<IndividualRandomData> RandomDataHandle;
        public ComponentTypeHandle<ShootingData> ShootingDataHandle;
        [ReadOnly] public ComponentTypeHandle<DumbUfoTag> DumbUfoTagHandle;
        [ReadOnly] public ComponentTypeHandle<CleverUfoTag> CleverUfoTagHandle;
        [ReadOnly] public ComponentTypeHandle<LocalToWorld> LocalToWorldHandle;
        [ReadOnly] public ComponentTypeHandle<ShootSoundData> ShootSoundDataHandle;
        [ReadOnly] public EntityTypeHandle EntityType;
        public float3 PlayerPosition;
        public EntityCommandBuffer Buffer;

        [BurstCompile]
        public void Execute(ArchetypeChunk batchInChunk, int batchIndex)
        {
            var l_shootingDataArray = batchInChunk.GetNativeArray(ShootingDataHandle);
            var l_randomDataArray = batchInChunk.GetNativeArray(RandomDataHandle);
            var l_enemyLocalToWorldArray = batchInChunk.GetNativeArray(LocalToWorldHandle);
            var l_enemyShootSoundData = batchInChunk.GetNativeArray(ShootSoundDataHandle);
            var l_entities = batchInChunk.GetNativeArray(EntityType);

            for (int i = 0; i < batchInChunk.Count; i++)
            {
                ShootingData l_shootData = l_shootingDataArray[i];
                if (!l_shootData.ShouldShootProjectile)
                    continue;
                IndividualRandomData l_randomData = l_randomDataArray[i];
                LocalToWorld l_localToWorldData = l_enemyLocalToWorldArray[i];

                var l_shootingDir = float3.zero;

                if (batchInChunk.Has(DumbUfoTagHandle))
                {
                    var l_randomDir = l_randomData.Random.NextFloat2Direction();
                    l_shootingDir = new float3(l_randomDir.x, 0, l_randomDir.y);
                }
                else if (batchInChunk.Has(CleverUfoTagHandle))
                {
                    var l_shootDirection = PlayerPosition - l_localToWorldData.Position;
                    l_shootDirection.y = 0;
                    l_shootingDir = math.normalize(l_shootDirection);
                }


                var l_projectileEntity = Buffer.Instantiate(l_shootData.ProjectilePrefab);

                var l_spawnPosition = l_localToWorldData.Position +
                                      l_shootingDir * l_shootData.ProjectileSpawnForwardOffset;
                var l_translation = new Translation() { Value = l_spawnPosition };
                Buffer.SetComponent(l_projectileEntity, l_translation);


                var l_rotation = new Rotation()
                    { Value = quaternion.LookRotation(l_shootingDir, Float3Constants.Up) };
                Buffer.SetComponent(l_projectileEntity, l_rotation);

                UninitializedProjectileTag l_uninitializedProjectile = new UninitializedProjectileTag()
                    { IntendedForwards = l_shootingDir };
                Buffer.AddComponent(l_projectileEntity, l_uninitializedProjectile);
                l_shootData.ShouldShootProjectile = false;

                var l_playShootSoundData = l_enemyShootSoundData[i];
                var l_audioPetition = new AudioPetition()
                {
                    AudioID = l_playShootSoundData.ShotSound,
                    Volume = l_playShootSoundData.ShotVolume,
                    ShouldLoop = false
                };

                var l_entity = l_entities[i];

                Buffer.AddComponent(l_entity, l_audioPetition);
                //Write neccesary data back

                l_shootingDataArray[i] = l_shootData;
                l_randomDataArray[i] = l_randomData;
            }
        }
    }

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class EnemyActionSystem : SystemBase
    {
        private Entity m_playerEntity;
        private EndSimulationEntityCommandBufferSystem m_endSimulationBuffer;
        private EntityQueryDesc m_ufoQueryDesc;

        protected override void OnCreate()
        {
            m_endSimulationBuffer = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
            m_ufoQueryDesc = new EntityQueryDesc()
            {
                All = new[]
                {
                    ComponentType.ReadWrite<IndividualRandomData>(),
                    ComponentType.ReadWrite<ShootingData>(),
                    ComponentType.ReadOnly<LocalToWorld>()
                },
                Any = new[]
                {
                    ComponentType.ReadOnly<DumbUfoTag>(),
                    ComponentType.ReadOnly<CleverUfoTag>()
                }
            };

            RequireSingletonForUpdate<PlayerTag>();
            RequireForUpdate(GetEntityQuery(m_ufoQueryDesc));
        }

        protected override void OnStartRunning()
        {
            m_playerEntity = GetSingletonEntity<PlayerTag>();
        }

        protected override void OnUpdate()
        {
            //Gather necessary job data

            var l_ufoQuery = GetEntityQuery(m_ufoQueryDesc);
            var l_playerLocalToWorld = EntityManager.GetComponentData<Translation>(m_playerEntity);
            var l_playerPosition = l_playerLocalToWorld.Value;

            //Create job and set dependency. Execute job.
            var l_enemyActionJob = new EnemyActionJob()
            {
                Buffer = m_endSimulationBuffer.CreateCommandBuffer(),
                CleverUfoTagHandle = GetComponentTypeHandle<CleverUfoTag>(true),
                DumbUfoTagHandle = GetComponentTypeHandle<DumbUfoTag>(true),
                LocalToWorldHandle = GetComponentTypeHandle<LocalToWorld>(true),
                PlayerPosition = l_playerPosition,
                RandomDataHandle = GetComponentTypeHandle<IndividualRandomData>(false),
                ShootingDataHandle = GetComponentTypeHandle<ShootingData>(false),
                ShootSoundDataHandle = GetComponentTypeHandle<ShootSoundData>(true),
                EntityType = GetEntityTypeHandle()
            };

            Dependency = l_enemyActionJob.Schedule(l_ufoQuery, Dependency);

            m_endSimulationBuffer.AddJobHandleForProducer(Dependency);
        }
    }
}