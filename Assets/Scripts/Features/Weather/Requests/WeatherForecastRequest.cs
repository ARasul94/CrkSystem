using System.Threading;
using Core.RequestQueue;
using Cysharp.Threading.Tasks;
using Features.Weather.Models;
using Features.Weather.Services;

namespace Features.Weather.Requests
{
    public class WeatherForecastRequest : IRequestCommand<WeatherViewData>
    {
        private readonly WeatherApiService m_weatherApiService;

        public object owner { get; }
        public string tag => WeatherRequestTags.FORECAST;

        public WeatherForecastRequest(
            object _owner,
            WeatherApiService _weatherApiService)
        {
            owner = _owner;
            m_weatherApiService = _weatherApiService;
        }

        public UniTask<WeatherViewData> ExecuteAsync(
            CancellationToken _cancellationToken)
        {
            return m_weatherApiService.GetForecastAsync(_cancellationToken);
        }
    }
}