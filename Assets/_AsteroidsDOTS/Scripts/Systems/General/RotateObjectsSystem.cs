using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace _AsteroidsDOTS.Scripts.Systems.General
{
    public class RotateObjectsSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate(GetEntityQuery(ComponentType.ReadWrite<RotateObjectData>()));
        }

        protected override void OnUpdate()
        {
            var l_deltaTime = Time.DeltaTime;
            Entities.ForEach((ref Rotation p_rotation, ref RotateObjectData p_rotationData) =>
            {
                var l_mulRotationQuaternion =
                    quaternion.RotateZ(math.radians(p_rotationData.RotateSpeed * l_deltaTime));
                p_rotation.Value = math.mul(p_rotation.Value, l_mulRotationQuaternion);
            }).ScheduleParallel();
        }
    }
}