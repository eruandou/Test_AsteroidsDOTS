using _AsteroidsDOTS.Scripts.DataComponents;
using _AsteroidsDOTS.Scripts.DataComponents.Enemies;
using _AsteroidsDOTS.Scripts.DataComponents.Tags;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace _AsteroidsDOTS.Scripts.Systems.Enemy
{
    public class EnemyInitializationSystem : SystemBase
    {
        private EndInitializationEntityCommandBufferSystem m_endInitializationBuffer;

        protected override void OnCreate()
        {
            m_endInitializationBuffer = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var l_ecb = m_endInitializationBuffer.CreateCommandBuffer();
            Entities.ForEach((Entity p_enemyEntity, int entityInQueryIndex, ref IndividualRandomData p_randomData,
                in UninitializedUFOTag p_uninitializedUfo,
                in EnemyMovementData p_movementData) =>
            {
                p_randomData.Random = Random.CreateFromIndex((uint)entityInQueryIndex);
                var l_linear = p_movementData.MovementSpeed * p_uninitializedUfo.IntendedDirection;
                var l_physicsVelocity = new PhysicsVelocity() { Angular = float3.zero, Linear = l_linear };
                l_ecb.SetComponent(p_enemyEntity, l_physicsVelocity);
                l_ecb.RemoveComponent<UninitializedUFOTag>(p_enemyEntity);
            }).Schedule();

            m_endInitializationBuffer.AddJobHandleForProducer(Dependency);
        }
    }
}