using _AsteroidsDOTS.Scripts.DataComponents;
using _AsteroidsDOTS.Scripts.DataComponents.Tags;
using Unity.Entities;

namespace _AsteroidsDOTS.Scripts.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public class PointsSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem m_endSimulationBuffer;
        private Entity m_gameStateEntity;

        protected override void OnCreate()
        {
            m_endSimulationBuffer = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnStartRunning()
        {
            m_gameStateEntity = GetSingletonEntity<GameStateData>();
        }

        protected override void OnUpdate()
        {
            var l_ecb = m_endSimulationBuffer.CreateCommandBuffer();
            var l_gameStateData = GetSingleton<GameStateData>();
            var l_gameStateEntity = m_gameStateEntity;
            Entities.WithAll<DeadPointsEntityTag>()
                .ForEach((Entity p_entity, in PointAwardData p_pointAwardData) =>
                {
                    //Add points here.
                    l_gameStateData.CurrentPoints +=
                        p_pointAwardData.PointsToGiveOut * l_gameStateData.PointsMultiplier;
                    l_ecb.DestroyEntity(p_entity);
                    l_ecb.SetComponent(l_gameStateEntity, l_gameStateData);
                })
                .Schedule();

            m_endSimulationBuffer.AddJobHandleForProducer(Dependency);
        }
    }
}