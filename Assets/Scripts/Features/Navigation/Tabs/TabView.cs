using UnityEngine;

namespace Features.Navigation.Tabs
{
    public abstract class TabView : MonoBehaviour
    {
        [SerializeField] private GameObject m_root;

        private GameObject root => m_root != null ? m_root : gameObject;

        public void Show()
        {
            root.SetActive(true);
        }

        public void Hide()
        {
            root.SetActive(false);
        }

#if UNITY_EDITOR
        private void Reset()
        {
            m_root = gameObject;
        }
#endif
    }
}