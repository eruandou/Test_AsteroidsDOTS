using System;
using _AsteroidsDOTS.Scripts.DataComponents;
using _AsteroidsDOTS.Scripts.DataComponents.Powerups;
using _AsteroidsDOTS.Scripts.DataComponents.Tags;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace _AsteroidsDOTS.Scripts.Systems
{
    public struct PowerUpJob : IJobEntityBatch
    {
        [ReadOnly] public ComponentTypeHandle<PowerUpEntityData> PowerUpEntityDataHandle;
        public Entity PlayerEntity;
        public Entity GameStateEntity;
        public GameStateData GameStateData;
        [ReadOnly, DeallocateOnJobCompletion] public NativeArray<Entity> PowerUpEntities;
        public EntityCommandBuffer Buffer;

        [BurstCompile]
        public void Execute(ArchetypeChunk batchInChunk, int batchIndex)
        {
            var l_powerUpEntityArray = batchInChunk.GetNativeArray(PowerUpEntityDataHandle);

            for (int i = 0; i < batchInChunk.Count; i++)
            {
                var l_powerup = l_powerUpEntityArray[i];
                if (!l_powerup.AlreadyPickedUp)
                    continue;
                GameStateData.PowerUpAlreadySpawned = false;
                Buffer.DestroyEntity(PowerUpEntities[i]);
                SpawnCorrespondingData(l_powerup.PowerUpType);
                Buffer.SetComponent(GameStateEntity, GameStateData);
            }
        }

        //TODO: Consider migrating from Enum to Authoring component + GetComponentDataFromEntity
        [BurstCompile]
        private void SpawnCorrespondingData(PowerUpType p_powerUpType)
        {
            Entity l_entityToWhichToApplyPowerUp;
            switch (p_powerUpType)
            {
                case PowerUpType.DoublePoints:
                    var l_doublePoint = new DoublePointsPowerUpData()
                    {
                        Duration = 10,
                        IsInUse = false
                    };
                    l_entityToWhichToApplyPowerUp = GameStateEntity;
                    Buffer.AddComponent(l_entityToWhichToApplyPowerUp, l_doublePoint);
                    break;
                case PowerUpType.DoubleShot:
                    var l_doubleShoot = new DoubleShotPowerUpData()
                    {
                        Duration = 10,
                        IsInUse = false
                    };
                    l_entityToWhichToApplyPowerUp = PlayerEntity;
                    Buffer.AddComponent(l_entityToWhichToApplyPowerUp, l_doubleShoot);
                    break;
                case PowerUpType.RecoverHealth:
                    var l_fullRecovery = new FullRecoveryPowerUpData();
                    l_entityToWhichToApplyPowerUp = PlayerEntity;
                    Buffer.AddComponent(l_entityToWhichToApplyPowerUp, l_fullRecovery);
                    break;
                case PowerUpType.InvulnerabilityShield:
                    var l_invulnerability = new InvulnerabilityShieldPowerUpData()
                    {
                        Duration = 10,
                        IsInUse = false,
                        ShieldEntity = Entity.Null
                    };
                    l_entityToWhichToApplyPowerUp = PlayerEntity;
                    Buffer.AddComponent(l_entityToWhichToApplyPowerUp, l_invulnerability);
                    break;
                case PowerUpType.SuperBomb:
                    var l_superBomb = new SuperBombPowerUpData();
                    l_entityToWhichToApplyPowerUp = GameStateEntity;
                    Buffer.AddComponent(l_entityToWhichToApplyPowerUp, l_superBomb);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(p_powerUpType), p_powerUpType, null);
            }
        }
    }

    public class PowerUpSystem : SystemBase
    {
        private EndInitializationEntityCommandBufferSystem m_endInitializationBuffer;
        private EntityQueryDesc m_powerupQueryDesc;

        protected override void OnCreate()
        {
            m_endInitializationBuffer = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
            m_powerupQueryDesc = new EntityQueryDesc()
            {
                All = new[]
                {
                    ComponentType.ReadOnly<PowerUpEntityData>(),
                }
            };

            RequireForUpdate(GetEntityQuery(m_powerupQueryDesc));
            RequireSingletonForUpdate<PlayerTag>();
        }

        protected override void OnUpdate()
        {
            var l_playerEntity = GetSingletonEntity<PlayerTag>();
            var l_query = GetEntityQuery(m_powerupQueryDesc);
            var l_gameStateEntity = GetSingletonEntity<GameStateData>();
            var l_gameStateData = GetSingleton<GameStateData>();
            var l_powerUpJob = new PowerUpJob()
            {
                PowerUpEntityDataHandle = GetComponentTypeHandle<PowerUpEntityData>(),
                Buffer = m_endInitializationBuffer.CreateCommandBuffer(),
                PlayerEntity = l_playerEntity,
                GameStateEntity = l_gameStateEntity,
                PowerUpEntities = l_query.ToEntityArray(Allocator.TempJob),
                GameStateData = l_gameStateData
            };

            Dependency = l_powerUpJob.Schedule(l_query, Dependency);
            m_endInitializationBuffer.AddJobHandleForProducer(Dependency);
        }
    }
}