using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Navigation
{
    public class NavigationView : MonoBehaviour
    {
        [SerializeField] private Button m_clickerButton;
        [SerializeField] private Button m_weatherButton;
        [SerializeField] private Button m_dogsButton;
        
        public IObservable<TabType> tabSelected => Observable.Merge(
            m_clickerButton.OnClickAsObservable().Select(_ => TabType.CLICKER),
            m_weatherButton.OnClickAsObservable().Select(_ => TabType.WEATHER),
            m_dogsButton.OnClickAsObservable().Select(_ => TabType.DOGS)
        );
        
        public void SetSelected(TabType _selectedTab)
        {
            m_clickerButton.interactable = _selectedTab != TabType.CLICKER;
            m_weatherButton.interactable = _selectedTab != TabType.WEATHER;
            m_dogsButton.interactable = _selectedTab != TabType.DOGS;
        }
    }
}