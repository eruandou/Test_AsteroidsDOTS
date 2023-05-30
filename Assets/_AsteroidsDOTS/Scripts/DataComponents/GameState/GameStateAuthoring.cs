using Unity.Entities;
using UnityEngine;

namespace _AsteroidsDOTS.Scripts.DataComponents.GameState
{
    public class GameStateAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [SerializeField] private int m_defaultPlayerLives;
        [SerializeField] private int m_defaultMultiplier;

        public void Convert(Entity p_entity, EntityManager p_dstManager, GameObjectConversionSystem p_conversionSystem)
        {
            var l_gameStateDataPlayer = new GameStateDataPlayer()
            {
                CurrentPlayerLives = m_defaultPlayerLives, CurrentPoints = 0, PointsMultiplier = m_defaultMultiplier
            };
            p_dstManager.AddComponent<GameStateDataPlayer>(p_entity);
            p_dstManager.SetComponentData(p_entity, l_gameStateDataPlayer);

            p_dstManager.AddComponent<GameStateDataUfo>(p_entity);
            p_dstManager.AddComponent<GameStatePowerUpData>(p_entity);
            p_dstManager.AddComponent<GameStateDataAsteroids>(p_entity);
        }
    }
}