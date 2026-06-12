using System;
using System.Threading;
using Core.Http;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using Features.Weather.Models;
using UnityEngine;

namespace Features.Weather.Services
{
    public class WeatherApiService
    {
        private readonly IHttpClient m_httpClient;
        private readonly ApiConfig m_apiConfig;

        public WeatherApiService(
            IHttpClient _httpClient,
            ApiConfig _apiConfig)
        {
            m_httpClient = _httpClient;
            m_apiConfig = _apiConfig;
        }

        public async UniTask<WeatherViewData> GetForecastAsync(
            CancellationToken _cancellationToken)
        {
            string json = await m_httpClient.GetAsync(
                m_apiConfig.weatherForecastUrl,
                _cancellationToken);

            WeatherForecastResponse response =
                JsonUtility.FromJson<WeatherForecastResponse>(json);

            if (response?.properties?.periods == null ||
                response.properties.periods.Length == 0)
            {
                throw new InvalidOperationException("Weather forecast response does not contain periods.");
            }

            WeatherPeriod today = response.properties.periods[0];

            return new WeatherViewData(
                today.temperature,
                today.temperatureUnit,
                today.shortForecast,
                today.icon);
        }
    }
}