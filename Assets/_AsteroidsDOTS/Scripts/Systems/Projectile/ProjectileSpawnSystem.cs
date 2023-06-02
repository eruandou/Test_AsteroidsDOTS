using _AsteroidsDOTS.Scripts.DataComponents;
using _AsteroidsDOTS.Scripts.DataComponents.Audio;
using _AsteroidsDOTS.Scripts.DataComponents.Tags;
using _AsteroidsDOTS.Scripts.DevelopmentUtilities;
using _AsteroidsDOTS.Scripts.Globals;
using _AsteroidsDOTS.Scripts.NonDOTSBehaviour;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace _AsteroidsDOTS.Scripts.Systems.Projectile
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class ProjectileSpawnSystem : SystemBase
    {
        private EndInitializationEntityCommandBufferSystem m_endInitializationBuffer;

        protected override void OnCreate()
        {
            m_endInitializationBuffer = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            if (!AsteroidsDOTS.GameIsStarted)
            {
                return;
            }

            var l_ecb = m_endInitializationBuffer.CreateCommandBuffer();
            Entities.ForEach(
                (Entity p_entity, ref ShootingData p_shootingData, in LocalToWorld p_localToWorld,
                    in Rotation p_rotation,
                    in ShootingStatsPowerUpData p_shootingStats, in ShootSoundData p_shootSoundData) =>
                {
                    if (!p_shootingData.ShouldShootProjectile) return;

                    //TODO: Add shooting modificator here

                    for (int i = 0; i < p_shootingStats.BulletsToShoot; i++)
                    {
                        //Instantiate the projectile entity and set its initial position and 
                        Entity l_projectileEntity = l_ecb.Instantiate(p_shootingData.ProjectilePrefab);
                        var l_deviationVector =
                            math.mul(p_rotation.Value,
                                quaternion.AxisAngle(Float3Constants.Up, i * p_shootingStats.BulletAngleDifference));
                        var l_intendedForwards = math.forward(l_deviationVector);
                        UninitializedProjectileTag l_projectileTag = new UninitializedProjectileTag()
                            { IntendedForwards = l_intendedForwards };
                        l_ecb.AddComponent(l_projectileEntity, l_projectileTag);
                        var l_spawnPosition = p_localToWorld.Position + p_localToWorld.Forward *
                            p_shootingData.ProjectileSpawnForwardOffset;
                        var l_projectileTranslation = new Translation() { Value = l_spawnPosition };
                        l_ecb.SetComponent(l_projectileEntity, l_projectileTranslation);
                        var l_rotation = new Rotation()
                        {
                            Value = l_deviationVector
                        };
                        // { Value = quaternion.LookRotation(l_intendedForwards, Float3Constants.Up) };
                        l_ecb.SetComponent(l_projectileEntity, l_rotation);
                    }

                    p_shootingData.ShouldShootProjectile = false;

                    var l_audioPetition = new AudioPetition()
                    {
                        AudioID = p_shootSoundData.ShotSound,
                        ShouldLoop = false,
                        Volume = p_shootSoundData.ShotVolume
                    };
                    l_ecb.AddComponent(p_entity, l_audioPetition);
                }
            ).Schedule();

            m_endInitializationBuffer.AddJobHandleForProducer(Dependency);
        }
    }
}