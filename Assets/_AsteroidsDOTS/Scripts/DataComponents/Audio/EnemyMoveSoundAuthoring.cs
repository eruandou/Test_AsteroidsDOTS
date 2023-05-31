using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace _AsteroidsDOTS.Scripts.DataComponents.Audio
{
    public class EnemyMoveSoundAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public string enemyMoveSound;
        public float moveVolume;
        public bool shouldLoop;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponent<ShootSoundData>(entity);
            EnemyMoveSoundData l_enemyMoveSoundData = new EnemyMoveSoundData()
            {
                EnemyMoveSound = new FixedString32(enemyMoveSound),
                MoveVolume = moveVolume,
                ShouldLoop = shouldLoop
            };
            dstManager.AddComponentData(entity, l_enemyMoveSoundData);
        }
    }
}