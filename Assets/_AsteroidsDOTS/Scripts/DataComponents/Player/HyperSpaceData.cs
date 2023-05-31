using Unity.Entities;
using UnityEngine;

namespace _AsteroidsDOTS.Scripts.DataComponents.Player
{
    [GenerateAuthoringComponent]
    public struct HyperSpaceData : IComponentData
    {
        public float HyperSpaceTime;
        [Range(0, 100)] public float HyperSpaceFailChance;
    }
}