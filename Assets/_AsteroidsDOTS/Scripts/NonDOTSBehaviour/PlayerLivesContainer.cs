using System.Collections.Generic;
using UnityEngine;

namespace _AsteroidsDOTS.Scripts.DataComponents.UI
{
    public class PlayerLivesContainer : MonoBehaviour
    {
        private int m_currentInstancedLives;
        [SerializeField] private GameObject m_playerLifeIconPrefab;
        private List<GameObject> m_instantiatedLiveIcons = new List<GameObject>();


        [ContextMenu("test")]
        private void SetNewTarget()
        {
            SetNewTarget(3);
        }
        public void SetNewTarget(int p_target)
        {
            if (m_currentInstancedLives == p_target)
                return;

            if (p_target > m_currentInstancedLives)
            {
                var l_iconsToCreate = p_target - m_currentInstancedLives;
                for (int i = 0; i < l_iconsToCreate; i++)
                {
                    GameObject l_newIcon = Instantiate(m_playerLifeIconPrefab, transform);
                    m_instantiatedLiveIcons.Add(l_newIcon);
                    l_newIcon.SetActive(true);
                }
            }
            else
            {
                var l_iconsToDisable = m_currentInstancedLives - p_target;
                for (int i = 0; i < l_iconsToDisable; i++)
                {
                    var l_lastIndex = m_instantiatedLiveIcons.Count - 1 - i;
                    GameObject l_iconToDisable = m_instantiatedLiveIcons[l_lastIndex];
                    l_iconToDisable.SetActive(false);
                }
            }

            m_currentInstancedLives = p_target;

            if (m_currentInstancedLives != 0) return;

            //Dead
            for (int i = m_instantiatedLiveIcons.Count - 1; i >= 0; i--)
            {
                Destroy(m_instantiatedLiveIcons[i]);
            }
        }
    }
}