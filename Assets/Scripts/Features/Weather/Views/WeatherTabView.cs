using Features.Navigation.Tabs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Weather.Views
{
    public class WeatherTabView: TabView
    {
        [Header("States")]
        [SerializeField] private GameObject m_loadingRoot;
        [SerializeField] private GameObject m_contentRoot;
        [SerializeField] private GameObject m_errorRoot;

        [Header("Content")]
        [SerializeField] private Image m_iconImage;
        [SerializeField] private TMP_Text m_weatherText;
        [SerializeField] private TMP_Text m_errorText;

        public void ShowLoading()
        {
            SetState(_isLoading: true, _hasContent: false, _hasError: false);
        }

        public void ShowWeather(Sprite _icon, string _text)
        {
            m_iconImage.sprite = _icon;
            m_iconImage.enabled = _icon != null;
            m_weatherText.text = _text;

            SetState(_isLoading: false, _hasContent: true, _hasError: false);
        }

        public void ShowError(string _message)
        {
            m_errorText.text = _message;

            SetState(_isLoading: false, _hasContent: false, _hasError: true);
        }

        public void Clear()
        {
            m_iconImage.sprite = null;
            m_iconImage.enabled = false;
            m_weatherText.text = string.Empty;
            m_errorText.text = string.Empty;

            SetState(_isLoading: false, _hasContent: false, _hasError: false);
        }

        private void SetState(bool _isLoading, bool _hasContent, bool _hasError)
        {
            m_loadingRoot.SetActive(_isLoading);
            m_contentRoot.SetActive(_hasContent);
            m_errorRoot.SetActive(_hasError);
        }
    }
}