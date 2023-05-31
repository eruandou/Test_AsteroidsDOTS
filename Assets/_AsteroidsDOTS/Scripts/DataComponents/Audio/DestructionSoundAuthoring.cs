using Unity.Entities;
using UnityEngine;

namespace _AsteroidsDOTS.Scripts.DataComponents.Audio
{
    public class DestructionSoundAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public string destructionSound;
        public float destructionVolume;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponent<DestructionSoundData>(entity);
            var l_destructionSoundData = new DestructionSoundData()
            {
                DestructionSound = destructionSound,
                Volume = destructionVolume
            };
            dstManager.AddComponentData(entity, l_destructionSoundData);
        }
    }
}