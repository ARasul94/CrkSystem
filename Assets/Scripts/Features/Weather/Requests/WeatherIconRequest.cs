using System.Threading;
using Core.Http;
using Core.RequestQueue;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Features.Weather.Requests
{
    public class WeatherIconRequest : IRequestCommand<Sprite>
    {
        private readonly IHttpClient m_httpClient;
        private readonly string m_iconUrl;

        public object owner { get; }
        public string tag => WeatherRequestTags.ICON;

        public WeatherIconRequest(
            object _owner,
            IHttpClient _httpClient,
            string _iconUrl)
        {
            owner = _owner;
            m_httpClient = _httpClient;
            m_iconUrl = _iconUrl;
        }

        public UniTask<Sprite> ExecuteAsync(CancellationToken _cancellationToken)
        {
            return m_httpClient.LoadSpriteAsync(m_iconUrl, _cancellationToken);
        }
    }
}