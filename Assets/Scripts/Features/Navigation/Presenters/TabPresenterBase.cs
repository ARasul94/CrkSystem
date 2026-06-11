using Features.Navigation.Tabs;
using UnityEngine;

namespace Features.Navigation.Presenters
{
    public abstract class TabPresenterBase: ITabPresenter
    {
        public abstract TabType tabType { get; }
        
        private readonly TabView m_view;

        protected TabPresenterBase(TabView _view)
        {
            m_view = _view;
        }
        
        public virtual void Show()
        {
            Debug.Log($"[{GetType().Name}] Show");
            m_view.Show();
        }

        public virtual void Hide()
        {
            Debug.Log($"[{GetType().Name}] Hide");
            m_view.Hide();
        }
    }
}