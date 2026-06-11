using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using UnityEngine;
using Zenject;

namespace Core.Http
{
    public class HttpClientTestPresenter : IInitializable, IDisposable
    {
        private readonly IHttpClient m_httpClient;
        private readonly ApiConfig m_apiConfig;
        private readonly CancellationTokenSource m_cts = new();
        
        public HttpClientTestPresenter(IHttpClient _httpClient, ApiConfig _apiConfig)
        {
            m_httpClient = _httpClient;
            m_apiConfig = _apiConfig;
        }

        public void Initialize()
        {
            RunTestAsync(m_cts.Token).Forget();
        }

        public void Dispose()
        {
            m_cts.Cancel();
            m_cts.Dispose();
        }
        
        private async UniTaskVoid RunTestAsync(CancellationToken _cancellationToken)
        {
            Debug.Log("[HttpClientTest] Test started");

            await TestSuccessRequest("Weather", m_apiConfig.weatherForecastUrl, _cancellationToken);
            await TestSuccessRequest("Dog Breeds", m_apiConfig.dogBreedsUrl, _cancellationToken);
            await TestHttpError(_cancellationToken);
            await TestCancellation();

            Debug.Log("[HttpClientTest] Test finished");
        }
        
        private async UniTask TestSuccessRequest(
            string _requestName,
            string _url,
            CancellationToken _cancellationToken)
        {
            try
            {
                string json = await m_httpClient.GetAsync(_url, _cancellationToken);

                Debug.Log(
                    $"[HttpClientTest] {_requestName} success. " +
                    $"Preview: {GetPreview(json)}");
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    $"[HttpClientTest] {_requestName} failed: {exception}");
            }
        }
        
        private async UniTask TestHttpError(CancellationToken _cancellationToken)
        {
            try
            {
                string invalidUrl = "https://dogapi.dog/api/v2/not-existing-endpoint";

                await m_httpClient.GetAsync(invalidUrl, _cancellationToken);

                Debug.LogError("[HttpClientTest] HTTP error test failed: request unexpectedly succeeded");
            }
            catch (HttpRequestFailedException exception)
            {
                Debug.Log(
                    $"[HttpClientTest] HTTP error handled correctly. " +
                    $"StatusCode: {exception.statusCode}, Url: {exception.url}");
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    $"[HttpClientTest] Unexpected exception in HTTP error test: {exception}");
            }
        }
        
        private async UniTask TestCancellation()
        {
            using var cancellationTokenSource = new CancellationTokenSource();

            try
            {
                UniTask<string> requestTask = m_httpClient.GetAsync(
                    m_apiConfig.weatherForecastUrl,
                    cancellationTokenSource.Token);

                cancellationTokenSource.Cancel();

                await requestTask;

                Debug.LogError("[HttpClientTest] Cancellation test failed: request unexpectedly completed");
            }
            catch (OperationCanceledException)
            {
                Debug.Log("[HttpClientTest] Cancellation handled correctly");
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    $"[HttpClientTest] Unexpected exception in cancellation test: {exception}");
            }
        }
        
        private static string GetPreview(string _value)
        {
            if (string.IsNullOrEmpty(_value))
                return string.Empty;

            const int maxLength = 300;

            return _value.Length <= maxLength
                ? _value
                : _value.Substring(0, maxLength);
        }
    }
}