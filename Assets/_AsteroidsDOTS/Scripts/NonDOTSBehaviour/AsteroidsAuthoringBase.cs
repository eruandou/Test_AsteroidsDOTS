using Unity.Entities;
using UnityEngine;

namespace _AsteroidsDOTS.Scripts.NonDOTSBehaviour
{
    public abstract class AsteroidsAuthoringBase : MonoBehaviour, IConvertGameObjectToEntity
    {
        public abstract void Convert(Entity entity, EntityManager dstManager,
            GameObjectConversionSystem conversionSystem);
    }
}