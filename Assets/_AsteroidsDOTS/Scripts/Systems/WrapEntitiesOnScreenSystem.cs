using _AsteroidsDOTS.Scripts.DataComponents;
using _AsteroidsDOTS.Scripts.Globals;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace _AsteroidsDOTS.Scripts.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup)), UpdateAfter(typeof(PlayerMovementSystem))]
    public class WrapEntitiesOnScreenSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            //TODO: Avoid checking against static state class
            if (!AsteroidsDOTS.GameIsStarted)
                return;

            float2 l_verticalLimits = GameplayGlobals.VerticalLimits;
            float2 l_horizontalLimits = GameplayGlobals.HorizontalLimits;
            float l_offset = GameplayGlobals.ScreenLimitOffset;
            Entities.WithAll<WrappingEntityTag>().ForEach((ref Translation p_translation) =>
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
            }).Schedule();
        }
    }
}