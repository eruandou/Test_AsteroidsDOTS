using _AsteroidsDOTS.Scripts.Globals;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace _AsteroidsDOTS.Scripts.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class PlayerMovementSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            if (!AsteroidsDOTS.GameIsStarted)
                return;

            float l_deltaTime = Time.DeltaTime;
            float2 verticalEdges = GameplayGlobals.VerticalLimits;
            float2 horizontalEdges = GameplayGlobals.HorizontalLimits;

            Entities.ForEach((ref PhysicsVelocity p_physicsVelocity, ref Translation p_translation,
                ref Rotation p_rotation,
                ref PlayerMovementData p_movementData, in PhysicsMass p_physicsMass) =>
            {
                //Check direction and add to current player speed
                float3 l_direction = math.mul(p_rotation.Value, new float3(0f, 0f, 1f));
                p_physicsVelocity.Linear += l_direction * p_movementData.ThrustImpulse * l_deltaTime;

                //If speed exceeds max limit, clamp it
                p_physicsVelocity.Linear =
                    DOTSMathUtils.ClampFloat3(p_physicsVelocity.Linear, p_movementData.MaxSpeed);

                //Reset data for next frame
                p_movementData.ThrustImpulse = 0f;

                float3 l_angularVelocity =
                    new float3(0f, 1f, 0f) * math.radians(p_movementData.TorqueForce * l_deltaTime);
                quaternion l_inertiaOrientationInWorldSpace =
                    math.mul(p_rotation.Value, p_physicsMass.InertiaOrientation);
                float3 l_angularVelocityInertiaSpace =
                    math.rotate(math.inverse(l_inertiaOrientationInWorldSpace), l_angularVelocity);
                p_physicsVelocity.Angular += l_angularVelocityInertiaSpace;
                p_movementData.TorqueForce = 0;

                p_physicsVelocity.Angular =
                    DOTSMathUtils.ClampFloat3(p_physicsVelocity.Angular, p_movementData.MaxAngularSpeed);

                p_translation.Value.y = 0;
                p_rotation.Value.value.x = 0f;
                p_rotation.Value.value.z = 0f;
            }).Schedule();
        }
    }
}