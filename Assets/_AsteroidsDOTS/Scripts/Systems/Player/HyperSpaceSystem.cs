using System.Security.Cryptography;
using _AsteroidsDOTS.Scripts.DataComponents;
using _AsteroidsDOTS.Scripts.DataComponents.Player;
using _AsteroidsDOTS.Scripts.DataComponents.Tags;
using _AsteroidsDOTS.Scripts.Globals;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace _AsteroidsDOTS.Scripts.Systems.Player
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class HyperSpaceSystem : SystemBase
    {
        private BeginInitializationEntityCommandBufferSystem m_beginInitializationBuffer;

        protected override void OnCreate()
        {
            m_beginInitializationBuffer = World.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>();
            var l_query = new EntityQueryDesc()
            {
                All = new[]
                {
                    ComponentType.ReadOnly<HyperSpaceTag>(),
                }
            };
            RequireForUpdate(GetEntityQuery(l_query));
        }

        protected override void OnStartRunning()
        {
            var l_ecb = m_beginInitializationBuffer.CreateCommandBuffer();
            Entities.WithAll<HyperSpaceTag>().ForEach((Entity p_entity) => { l_ecb.AddComponent<Scale>(p_entity); })
                .Run();
            CompleteDependency();
        }


        protected override void OnUpdate()
        {
            var l_ecb = m_beginInitializationBuffer.CreateCommandBuffer();
            var l_random = new Random((uint)UnityEngine.Random.Range(0, 200000));
            var l_deltaTime = Time.DeltaTime;
            Entities.WithAll<HyperSpaceTag>()
                .ForEach((Entity p_playerEntity, ref HyperSpaceState p_hyperSpaceState,
                    ref EntityHealthData p_entityHealthData,
                    ref Scale p_scale, ref Translation p_translation, in HyperSpaceData p_hyperSpaceData) =>
                {
                    if (!p_hyperSpaceState.IsHyperSpacing)
                    {
                        var l_isDead = l_random.NextFloat(0, 100) < p_hyperSpaceData.HyperSpaceFailChance;
                        if (l_isDead)
                        {
                            p_entityHealthData.PendingHealthModification -= p_entityHealthData.Health;
                            l_ecb.RemoveComponent<HyperSpaceTag>(p_playerEntity);
                            return;
                        }

                        p_hyperSpaceState.IsHyperSpacing = true;
                        p_entityHealthData.InvincibilityTime = p_hyperSpaceData.HyperSpaceTime;
                        p_hyperSpaceState.RemainingHyperSpaceTime = p_hyperSpaceData.HyperSpaceTime;
                        p_scale.Value = 0;
                        //Choose a random new position
                        var l_positionX = l_random.NextFloat(GameplayStaticGlobals.HorizontalLimits.x,
                            GameplayStaticGlobals.HorizontalLimits.y);
                        var l_positionZ = l_random.NextFloat(GameplayStaticGlobals.VerticalLimits.x,
                            GameplayStaticGlobals.VerticalLimits.y);
                        p_translation.Value = new float3(l_positionX, 0, l_positionZ);
                        return;
                    }

                    p_hyperSpaceState.RemainingHyperSpaceTime -= l_deltaTime;

                    if (p_hyperSpaceState.ShouldReturnFromHyperSpace)
                    {
                        p_scale.Value = 1;
                        l_ecb.RemoveComponent<HyperSpaceTag>(p_playerEntity);
                        p_hyperSpaceState.IsHyperSpacing = false;
                    }
                })
                .Schedule();

            m_beginInitializationBuffer.AddJobHandleForProducer(Dependency);
        }
    }
}