using _AsteroidsDOTS.Scripts.DataComponents;
using _AsteroidsDOTS.Scripts.DataComponents.UI;
using Unity.Entities;

namespace _AsteroidsDOTS.Scripts.Systems
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class UpdateUISystem : SystemBase
    {
        private BeginPresentationEntityCommandBufferSystem m_beginPresentationBuffer;

        protected override void OnCreate()
        {
            m_beginPresentationBuffer = World.GetExistingSystem<BeginPresentationEntityCommandBufferSystem>();
            RequireForUpdate(GetEntityQuery(ComponentType.ReadOnly<DirtyUITag>()));
        }

        protected override void OnUpdate()
        {
            var l_ecb = m_beginPresentationBuffer.CreateCommandBuffer();
            var l_gameStateData = GetSingleton<GameStateData>();

            Entities.WithAll<DirtyUITag>().ForEach((Entity p_entity, UIUpdater p_uiUpdater) =>
            {
                p_uiUpdater.AssignData(new UIData()
                {
                    CurrentPoints = l_gameStateData.CurrentPoints,
                    PlayerLives = l_gameStateData.CurrentPlayerLives
                });
                l_ecb.RemoveComponent<DirtyUITag>(p_entity);
            }).WithoutBurst().Run();


            m_beginPresentationBuffer.AddJobHandleForProducer(Dependency);
        }
    }
}