using _AsteroidsDOTS.Scripts.DataComponents;
using _AsteroidsDOTS.Scripts.DataComponents.Asteroids;
using _AsteroidsDOTS.Scripts.DataComponents.Tags;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

namespace _AsteroidsDOTS.Scripts.Systems
{
    public struct GameTriggerJob : ITriggerEventsJob
    {
        public ComponentDataFromEntity<EntityHealthData> EntityHealthCDFE;
        [ReadOnly] public ComponentDataFromEntity<ProjectileData> ProjectileCDFE;
        [ReadOnly] public ComponentDataFromEntity<PlayerTag> PlayerTagCDFE;
        [ReadOnly] public ComponentDataFromEntity<AsteroidData> AsteroidCDFE;
        [ReadOnly] public ComponentDataFromEntity<DumbUfoTag> DumbUfoTagCDFE;
        [ReadOnly] public ComponentDataFromEntity<CleverUfoTag> CleverUfoTagCDFE;
        public EntityCommandBuffer Buffer;

        [BurstCompile]
        public void Execute(TriggerEvent p_triggerEvent)
        {
            Entity l_entityA = p_triggerEvent.EntityA;
            Entity l_entityB = p_triggerEvent.EntityB;

            bool l_entityAHasHealth = EntityHealthCDFE.HasComponent(l_entityA);
            bool l_entityBHasHealth = EntityHealthCDFE.HasComponent(l_entityB);

            //Which means both collided entities doeNOT have health, and we hit maybe an obstacle?
            if (!l_entityAHasHealth && !l_entityBHasHealth)
            {
                return;
            }

            //Check if the collision was due to projectile colliding with something. If so, we found our targets
            if (CheckProjectileCompatibility(l_entityA, l_entityB))
            {
                return;
            }

            if (CheckPowerUpCompatibility(l_entityA, l_entityB))
            {
                return;
            }

            //What remains is Player vs Asteroid, Asteroid vs Enemy, or Player vs Enemy. 
            //In any of those cases, we remove the remaining health of the entities.

            HandleRegularCollisionForEntities(l_entityA, l_entityB, l_entityAHasHealth, l_entityBHasHealth);
        }

        [BurstCompile]
        private void HandleRegularCollisionForEntities(Entity p_entityA, Entity p_entityB, bool p_aHasHealth,
            bool p_bHasHealth)
        {
            if (p_aHasHealth)
            {
                EntityHealthData l_entityAHealth = EntityHealthCDFE[p_entityA];
                l_entityAHealth.PendingHealthModification -= l_entityAHealth.Health;
                Buffer.SetComponent(p_entityA, l_entityAHealth);
            }

            if (p_bHasHealth)
            {
                EntityHealthData l_entityBHealth = EntityHealthCDFE[p_entityB];
                l_entityBHealth.PendingHealthModification -= l_entityBHealth.Health;
                Buffer.SetComponent(p_entityB, l_entityBHealth);
            }
        }

        //TODO: Fill with PowerUp logic on next iteration 
        private bool CheckPowerUpCompatibility(Entity p_entityA, Entity p_entityB)
        {
            return false;
        }

        /// <summary>
        /// Checks if the entities are projectiles, and acts accordingly.
        /// </summary>
        /// <param name="p_entityA"></param>
        /// <param name="p_entityB"></param>
        /// <returns>If the operation was successful, meaning at least one entity was a projectile</returns>
        [BurstCompile]
        private bool CheckProjectileCompatibility(Entity p_entityA, Entity p_entityB)
        {
            var l_entityAIsProjectile = ProjectileCDFE.HasComponent(p_entityA);
            var l_entityBIsProjectile = ProjectileCDFE.HasComponent(p_entityB);


            //Neither of the entities are projectiles
            if (!l_entityAIsProjectile && !l_entityBIsProjectile)
            {
                return false;
            }

            //Both are projectiles. They cancel out and get destroyed
            if (l_entityAIsProjectile && l_entityBIsProjectile)
            {
                Buffer.DestroyEntity(p_entityA);
                Buffer.DestroyEntity(p_entityB);
                return true;
            }

            //Get which entity is the projectile
            Entity l_projectileEntity = l_entityAIsProjectile ? p_entityA : p_entityB;
            ProjectileData l_projectileData = ProjectileCDFE[l_projectileEntity];

            //The other entity has health.
            Entity l_healthEntity = l_entityAIsProjectile ? p_entityB : p_entityA;
            var l_entityHealth = EntityHealthCDFE[l_healthEntity];
            l_entityHealth.PendingHealthModification -= l_projectileData.Damage;

            //Write back to component
            Buffer.SetComponent(l_healthEntity, l_entityHealth);
            Buffer.DestroyEntity(l_projectileEntity);

            return true;
        }
    }

    [UpdateBefore(typeof(EndFramePhysicsSystem))]
    [UpdateAfter(typeof(ExportPhysicsWorld))]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public class CollisionSystem : SystemBase
    {
        private EntityQueryDesc m_projectileEntityQueryDesc;
        private EndSimulationEntityCommandBufferSystem m_endSimulationBuffer;
        private StepPhysicsWorld m_stepPhysicsWorld;
        private BuildPhysicsWorld m_buildPhysicsWorld;

        protected override void OnCreate()
        {
            m_endSimulationBuffer = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

            RequireForUpdate(GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(PhysicsCollider) }
            }));

            m_stepPhysicsWorld = World.GetExistingSystem<StepPhysicsWorld>();
            m_buildPhysicsWorld = World.GetExistingSystem<BuildPhysicsWorld>();
        }

        protected override void OnUpdate()
        {
            var l_collisionsJob = new GameTriggerJob()
            {
                Buffer = m_endSimulationBuffer.CreateCommandBuffer(),
                EntityHealthCDFE = GetComponentDataFromEntity<EntityHealthData>(false),
                ProjectileCDFE = GetComponentDataFromEntity<ProjectileData>(true),
                AsteroidCDFE = GetComponentDataFromEntity<AsteroidData>(true),
                PlayerTagCDFE = GetComponentDataFromEntity<PlayerTag>(true),
                CleverUfoTagCDFE = GetComponentDataFromEntity<CleverUfoTag>(true),
                DumbUfoTagCDFE = GetComponentDataFromEntity<DumbUfoTag>(true),
            };
            Dependency = l_collisionsJob.Schedule(m_stepPhysicsWorld.Simulation, ref m_buildPhysicsWorld.PhysicsWorld,
                Dependency);
        }
    }
}