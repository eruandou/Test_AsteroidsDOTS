using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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

            //TODO: This if a post test fix


            if (p_target > m_currentInstancedLives)
            {
                var l_currentCount = m_instantiatedLiveIcons.Count;
                //We add or activate more health components

                if (l_currentCount >= p_target)
                {
                    //We just enable the needed elements

                    for (int i = m_currentInstancedLives; i < p_target - 1; i++)
                    {
                        m_instantiatedLiveIcons[i].SetActive(true);
                    }
                }
                else
                {
                    //We need to add icons
                    var l_lifesToInstantiate = p_target - m_currentInstancedLives;
                    for (int i = 0; i < l_lifesToInstantiate; i++)
                    {
                        GameObject l_newIcon = Instantiate(m_playerLifeIconPrefab, transform);
                        m_instantiatedLiveIcons.Add(l_newIcon);
                        l_newIcon.SetActive(true);
                    }
                }
            }
            else
            {
                for (int i = m_currentInstancedLives - 1; i >= p_target; i--)
                {
                    m_instantiatedLiveIcons[i].SetActive(false);
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