using _AsteroidsDOTS.Scripts.DataComponents;
using _AsteroidsDOTS.Scripts.DataComponents.Tags;
using _AsteroidsDOTS.Scripts.DataComponents.UI;
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
            
            var l_ecb = m_endSimulationBuffer.CreateCommandBuffer();
            var l_uiUpdateEntity = GetSingletonEntity<UIUpdater>();
            Entities.ForEach((in GameStateData p_gameStateData) =>
            {
                l_ecb.AddComponent<DirtyUITag>(l_uiUpdateEntity);
            }).Schedule();

            m_endSimulationBuffer.AddJobHandleForProducer(Dependency);
        }

        protected override void OnUpdate()
        {
            var l_ecb = m_endSimulationBuffer.CreateCommandBuffer().AsParallelWriter();
            var l_gameStateData = GetSingleton<GameStateData>();
            var l_uiUpdateEntity = GetSingletonEntity<UIUpdater>();
            var l_gameStateEntity = m_gameStateEntity;
            Entities.WithAll<DeadPointsEntityTag>()
                .ForEach((Entity p_entity, int entityInQueryIndex, in PointAwardData p_pointAwardData) =>
                {
                    //Add points here.
                    l_gameStateData.CurrentPoints +=
                        (int)(p_pointAwardData.PointsToGiveOut * l_gameStateData.PointsMultiplier);
                    l_ecb.DestroyEntity(entityInQueryIndex, p_entity);
                    l_ecb.SetComponent(l_gameStateEntity.Index, l_gameStateEntity, l_gameStateData);
                    l_ecb.AddComponent<DirtyUITag>(l_uiUpdateEntity.Index, l_uiUpdateEntity);
                })
                .ScheduleParallel();

            m_endSimulationBuffer.AddJobHandleForProducer(Dependency);
        }
    }
}