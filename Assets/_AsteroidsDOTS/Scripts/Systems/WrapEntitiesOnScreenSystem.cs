using _AsteroidsDOTS.Scripts.DataComponents;
using _AsteroidsDOTS.Scripts.Globals;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace _AsteroidsDOTS.Scripts.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class WrapEntitiesOnScreenSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            //TODO: Avoid checking against static state class
            if (!AsteroidsDOTS.GameIsStarted)
                return;

            float2 l_verticalLimits = GameplayStaticGlobals.VerticalLimits;
            float2 l_horizontalLimits = GameplayStaticGlobals.HorizontalLimits;
            float l_offset = GameplayStaticGlobals.ScreenLimitOffset;
            Entities.WithAll<WrappingEntityTag>().ForEach(
                (ref Translation p_translation, ref PhysicsVelocity p_physicsVelocity) =>
                {
                    if (p_translation.Value.z > l_verticalLimits.y)
                    {
                        p_translation.Value.z = l_verticalLimits.x + l_offset;
                    }
                    else if (p_translation.Value.z < l_verticalLimits.x)
                    {
                        p_translation.Value.z = l_verticalLimits.y - l_offset;
                    }


                    if (p_translation.Value.x > l_horizontalLimits.y)
                    {
                        p_translation.Value.x = l_horizontalLimits.x + l_offset;
                    }
                    else if (p_translation.Value.x < l_horizontalLimits.x)
                    {
                        p_translation.Value.x = l_horizontalLimits.y - l_offset;
                    }

                    //Hard hook Y axis to 0 not to allow weird behaviours
                    p_translation.Value.y = 0;
                }).Schedule();
        }
    }
}