using System;
using Unity.Entities;

namespace _AsteroidsDOTS.Scripts.DataComponents.Player
{
    
    public struct HyperSpaceState : IComponentData
    {
        public bool IsHyperSpacing;
        public float RemainingHyperSpaceTime;
        public bool ShouldReturnFromHyperSpace => RemainingHyperSpaceTime <= 0;
    }
}