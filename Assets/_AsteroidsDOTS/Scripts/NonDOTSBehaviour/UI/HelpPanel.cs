using System.Collections;
using UnityEngine;

namespace _AsteroidsDOTS.Scripts.NonDOTSBehaviour.UI
{
    public class HelpPanel : MonoBehaviour
    {
        [SerializeField] private Animator m_animator;
        private static readonly int Appear = Animator.StringToHash("Appear");
        [SerializeField] private float m_hidingTime;

        private void SetAsAppear(bool p_appear)
        {
            m_animator.SetBool(Appear, p_appear);
        }

        private void HideAfterTime(float p_time)
        {
            StartCoroutine(IEHideCoroutine(p_time));
        }

        private IEnumerator IEHideCoroutine(float p_time)
        {
            yield return new WaitForSeconds(p_time);
            SetAsAppear(false);
        }

        public void Init()
        {
            SetAsAppear(true);
            HideAfterTime(m_hidingTime);
        }
    }
}