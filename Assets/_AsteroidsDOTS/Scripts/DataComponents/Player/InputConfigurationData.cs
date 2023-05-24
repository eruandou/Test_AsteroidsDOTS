using Unity.Entities;
using UnityEngine;

namespace _AsteroidsDOTS.Scripts.DataComponents
{
    [GenerateAuthoringComponent]
    public struct InputConfigurationData : IComponentData
    {
        public KeyCode ThrustForwardsKey;
        public KeyCode ThrustBackwardsKey;
        public KeyCode RotateRightKey;
        public KeyCode RotateLeftKey;
        public KeyCode ShootKey;
    }
}