using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace _AsteroidsDOTS.Scripts.DataComponents.Audio
{
    public class ShootSoundAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public string shotString;
        [Range(0, 1)] public float shotVolume;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponent<ShootSoundData>(entity);
            ShootSoundData l_shootSoundData = new ShootSoundData()
            {
                ShotSound = new FixedString32(shotString),
                ShotVolume = shotVolume
            };
            dstManager.AddComponentData(entity, l_shootSoundData);
        }
    }
}