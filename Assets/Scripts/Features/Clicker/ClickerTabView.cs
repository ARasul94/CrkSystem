using System;
using Features.Navigation.Tabs;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Clicker
{
    public class ClickerTabView : TabView
    {
        [SerializeField] private Button m_clickButton;
        [SerializeField] private TextMeshProUGUI m_currencyText;
        [SerializeField] private TextMeshProUGUI m_energyText;
        [SerializeField] private ClickerClickVfx m_clickVfx;

        public IObservable<Unit> clicked => m_clickButton.OnClickAsObservable();

        public void SetCurrency(int _value)
        {
            m_currencyText.text = _value.ToString();
        }

        public void SetEnergy(int _value)
        {
            m_energyText.text = _value.ToString();
        }

        public void SetClickAvailable(bool _isAvailable)
        {
            m_clickButton.interactable = _isAvailable;
        }

        public void PlayClickFeedback()
        {
            m_clickVfx.Play();
        }

#if UNITY_EDITOR
        private void Reset()
        {
            m_clickButton = GetComponentInChildren<Button>();
            m_clickVfx = GetComponentInChildren<ClickerClickVfx>();
        }
#endif
    }
}