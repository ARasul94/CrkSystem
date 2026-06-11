using System;
using System.Collections.Generic;
using System.Linq;
using Features.Navigation.Presenters;
using UniRx;
using UnityEngine;
using Zenject;

namespace Features.Navigation
{
    public class NavigationPresenter : IInitializable, IDisposable
    {
        private readonly NavigationView m_view;
        private readonly Dictionary<TabType, ITabPresenter> m_tabs;
        private readonly CompositeDisposable m_disposables = new();

        private TabType? m_currentTab;
        
        public NavigationPresenter(
            NavigationView _view,
            List<ITabPresenter> _tabPresenters)
        {
            m_view = _view;
            m_tabs = _tabPresenters.ToDictionary(_tab => _tab.tabType);
        }
        
        public void Initialize()
        {
            ValidateTabs();

            foreach (ITabPresenter tab in m_tabs.Values)
            {
                tab.Hide();
            }

            m_view.tabSelected
                .Subscribe(SwitchTo)
                .AddTo(m_disposables);

            SwitchTo(TabType.CLICKER);
        }

        public void Dispose()
        {
            m_disposables.Dispose();
        }
        
        private void SwitchTo(TabType _targetTab)
        {
            if (m_currentTab == _targetTab)
                return;

            if (m_currentTab.HasValue)
            {
                m_tabs[m_currentTab.Value].Hide();
            }

            Debug.Log($"[NavigationPresenter] Switch to {_targetTab}");

            m_tabs[_targetTab].Show();
            m_view.SetSelected(_targetTab);

            m_currentTab = _targetTab;
        }
        
        private void ValidateTabs()
        {
            foreach (TabType tabType in (TabType[])Enum.GetValues(typeof(TabType)))
            {
                if (!m_tabs.ContainsKey(tabType))
                {
                    throw new InvalidOperationException(
                        $"Missing tab presenter for tab type: {tabType}");
                }
            }
        }
    }
}