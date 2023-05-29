using Unity.Entities;

namespace _AsteroidsDOTS.Scripts.DataComponents
{
    public struct RespawnPlayerData : IComponentData
    {
        public float RespawnTimeLeft;
        public bool RespawnPlayer => RespawnTimeLeft <= 0;
    }
}