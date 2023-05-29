using TMPro;
using Unity.Entities;

namespace _AsteroidsDOTS.Scripts.DataComponents.UI
{
    [GenerateAuthoringComponent]
    public class UIUpdater : IComponentData
    {
        public TMP_Text m_currentPointsText;
        public PlayerLivesContainer m_playerLivesContainer;

        public void AssignData(UIData p_data)
        {
            m_currentPointsText.text = p_data.CurrentPoints.ToString();
            m_playerLivesContainer.SetNewTarget(p_data.PlayerLives);
        }
    }
}