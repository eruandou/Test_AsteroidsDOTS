using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace _AsteroidsDOTS.Scripts.DataComponents
{
    [GenerateAuthoringComponent]
    public struct IndividualRandomData : IComponentData
    {
        [HideInInspector] public Random Random;
    }
}