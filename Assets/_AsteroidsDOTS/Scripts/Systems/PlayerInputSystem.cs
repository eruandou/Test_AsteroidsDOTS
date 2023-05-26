using _AsteroidsDOTS.Scripts.DataComponents;
using _AsteroidsDOTS.Scripts.Globals;
using Unity.Entities;
using UnityEngine;

namespace _AsteroidsDOTS.Scripts.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class PlayerInputSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            //Wait for game start to operate system
            if (!AsteroidsDOTS.GameIsStarted)
                return;


            //Query for the player input
            float l_currentTime = (float)Time.ElapsedTime;

            Entities.ForEach((ref PlayerMovementData p_playerMovementData, ref ShootingData p_playerShootingData,
                in InputConfigurationData p_inputConfiguration) =>
            {
                float l_vertical = 0;
                l_vertical += Input.GetKey(p_inputConfiguration.ThrustForwardsKey) ? 1 : 0;
                l_vertical -= Input.GetKey(p_inputConfiguration.ThrustBackwardsKey) ? 1 : 0;

                float l_horizontal = 0;
                l_horizontal += Input.GetKey(p_inputConfiguration.RotateRightKey) ? 1 : 0;
                l_horizontal -= Input.GetKey(p_inputConfiguration.RotateLeftKey) ? 1 : 0;

                bool l_shootPetition = Input.GetKey(p_inputConfiguration.ShootKey);

                //Set data for current acceleration frame
                p_playerMovementData.ThrustImpulse = l_vertical * p_playerMovementData.ThrustAccelerationRate;
                p_playerMovementData.TorqueForce = l_horizontal * p_playerMovementData.RotationAcceleration;

                if (l_shootPetition && p_playerShootingData.NextShootingTime <= l_currentTime)
                {
                    p_playerShootingData.LastShootingTime = l_currentTime;
                    p_playerShootingData.ShouldShootProjectile = true;
                }
            }).Run();
        }
    }
}