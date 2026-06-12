using System;
using System.Threading;
using Core.Http;
using Core.RequestQueue;
using Cysharp.Threading.Tasks;
using Features.Navigation;
using Features.Navigation.Presenters;
using Features.Weather.Models;
using Features.Weather.Requests;
using Features.Weather.Services;
using Features.Weather.Views;
using UnityEngine;
using Zenject;

namespace Features.Weather.Presenters
{
    public class WeatherTabPresenter : TabPresenterBase, IInitializable, IDisposable
    {
        private const float REFRESH_INTERVAL_SECONDS = 5f;
        
        public override TabType tabType => TabType.WEATHER;
        
        private readonly WeatherTabView m_weatherView;
        private readonly IRequestQueue m_requestQueue;
        private readonly WeatherApiService m_weatherApiService;
        private readonly IHttpClient m_httpClient;

        private CancellationTokenSource m_tabLifetimeCts;
        private bool m_isShown;
        
        public WeatherTabPresenter(WeatherTabView _view,
            IRequestQueue _requestQueue,
            WeatherApiService _weatherApiService,
            IHttpClient _httpClient) : base(_view)
        {
            m_weatherView = _view;
            m_requestQueue = _requestQueue;
            m_weatherApiService = _weatherApiService;
            m_httpClient = _httpClient;
        }
        
        public void Initialize()
        {
            m_weatherView.Hide();
            m_weatherView.Clear();
        }
        
        public void Dispose()
        {
            StopRefreshLoop();

            m_requestQueue.CancelByOwner(this);
        }
        
        public override void Show()
        {
            if (m_isShown)
                return;

            m_isShown = true;

            Debug.Log("[WeatherPresenter] Show");

            base.Show();

            StartRefreshLoop();
        }

        public override void Hide()
        {
            if (!m_isShown)
                return;

            m_isShown = false;

            Debug.Log("[WeatherPresenter] Hide");

            StopRefreshLoop();

            m_requestQueue.CancelByOwner(this);

            m_weatherView.Clear();
            m_weatherView.Hide();
        }
        
        private void StartRefreshLoop()
        {
            StopRefreshLoop();

            m_tabLifetimeCts = new CancellationTokenSource();

            RefreshLoopAsync(m_tabLifetimeCts.Token).Forget();
        }

        private void StopRefreshLoop()
        {
            if (m_tabLifetimeCts == null)
                return;

            m_tabLifetimeCts.Cancel();
            m_tabLifetimeCts.Dispose();
            m_tabLifetimeCts = null;
        }
        
        private async UniTaskVoid RefreshLoopAsync(CancellationToken _cancellationToken)
        {
            var interval = TimeSpan.FromSeconds(REFRESH_INTERVAL_SECONDS);
            while (!_cancellationToken.IsCancellationRequested)
            {
                await LoadForecastAsync(_cancellationToken);

                await UniTask.Delay(
                    interval,
                    cancellationToken: _cancellationToken);
            }
        }
        
        private async UniTask LoadForecastAsync(CancellationToken _cancellationToken)
        {
            try
            {
                m_weatherView.ShowLoading();

                var request = new WeatherForecastRequest(
                    _owner: this,
                    _weatherApiService: m_weatherApiService);

                RequestHandle<WeatherViewData> handle =
                    m_requestQueue.Enqueue(request);

                WeatherViewData weather = await handle.task;

                if (_cancellationToken.IsCancellationRequested)
                    return;

                Sprite icon = null;

                if (!string.IsNullOrWhiteSpace(weather.m_iconUrl))
                {
                    var iconRequest = new WeatherIconRequest(
                        _owner: this,
                        _httpClient: m_httpClient,
                        _iconUrl: weather.m_iconUrl);

                    RequestHandle<Sprite> iconHandle = m_requestQueue.Enqueue(iconRequest);

                    icon = await iconHandle.task;
                }
                
                if (_cancellationToken.IsCancellationRequested)
                    return;

                m_weatherView.ShowWeather(
                    icon,
                    weather.GetFormattedText());

                Debug.Log($"[WeatherPresenter] Weather loaded: {weather.GetFormattedText()}");
            }
            catch (OperationCanceledException)
            {
                Debug.Log("[WeatherPresenter] Weather request canceled");
            }
            catch (HttpRequestFailedException exception)
            {
                Debug.LogError($"[WeatherPresenter] HTTP error: {exception.Message}");

                if (!_cancellationToken.IsCancellationRequested)
                    m_weatherView.ShowError("Не удалось загрузить погоду");
            }
            catch (Exception exception)
            {
                Debug.LogError($"[WeatherPresenter] Unexpected error: {exception}");

                if (!_cancellationToken.IsCancellationRequested)
                    m_weatherView.ShowError("Ошибка обработки погоды");
            }
        }
    }
}