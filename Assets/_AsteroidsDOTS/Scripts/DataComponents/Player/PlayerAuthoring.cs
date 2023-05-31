using _AsteroidsDOTS.Scripts.DataComponents.Tags;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace _AsteroidsDOTS.Scripts.DataComponents.Player
{
    public class PlayerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponent<HyperSpaceState>(entity);
            dstManager.AddComponent<PlayerTag>(entity);
            dstManager.AddComponent<WrappingEntityTag>(entity);
        }
    }
}