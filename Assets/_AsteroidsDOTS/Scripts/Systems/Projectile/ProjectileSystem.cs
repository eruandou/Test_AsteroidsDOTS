using _AsteroidsDOTS.Scripts.DataComponents;
using Unity.Entities;

namespace _AsteroidsDOTS.Scripts.Systems.Projectile
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class ProjectileSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem m_endSimulationBuffer;

        protected override void OnCreate()
        {
            m_endSimulationBuffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var l_deltaTime = Time.DeltaTime;
            var l_ecb = m_endSimulationBuffer.CreateCommandBuffer();
            Entities.ForEach((Entity p_entity, ref ProjectileData p_projectileData) =>
            {
                p_projectileData.Lifetime -= l_deltaTime;

                if (p_projectileData.ShouldBeDisposed)
                {
                    l_ecb.DestroyEntity(p_entity);
                }
            }).Schedule();

            m_endSimulationBuffer.AddJobHandleForProducer(Dependency);
        }
    }
}