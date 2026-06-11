using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Core.Http
{
    public class RestClient: IHttpClient
    {
        public async UniTask<string> GetAsync(string _url, CancellationToken _cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_url))
                throw new ArgumentException("Url is null or empty.", nameof(_url));

            using UnityWebRequest request = UnityWebRequest.Get(_url);

            using CancellationTokenRegistration cancellationRegistration =
                _cancellationToken.Register(() =>
                {
                    if (!request.isDone)
                    {
                        Debug.Log($"[HttpClient] Abort request: {_url}");
                        request.Abort();
                    }
                });

            try
            {
                Debug.Log($"[HttpClient] GET started: {_url}");

                await request
                    .SendWebRequest()
                    .ToUniTask(cancellationToken: _cancellationToken);

                ThrowIfRequestFailed(request, _url);

                string response = request.downloadHandler.text;

                Debug.Log($"[HttpClient] GET success: {_url}");

                return response;
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"[HttpClient] GET canceled: {_url}");
                throw;
            }
        }
        
        private static void ThrowIfRequestFailed(UnityWebRequest _request, string _url)
        {
            switch (_request.result)
            {
                case UnityWebRequest.Result.Success:
                    return;

                case UnityWebRequest.Result.ConnectionError:
                    throw new HttpRequestFailedException(
                        $"Network error: {_request.error}",
                        _url,
                        _request.responseCode);

                case UnityWebRequest.Result.ProtocolError:
                    throw new HttpRequestFailedException(
                        $"HTTP error {_request.responseCode}: {_request.error}",
                        _url,
                        _request.responseCode);

                case UnityWebRequest.Result.DataProcessingError:
                    throw new HttpRequestFailedException(
                        $"Data processing error: {_request.error}",
                        _url,
                        _request.responseCode);

                default:
                    throw new HttpRequestFailedException(
                        $"Unknown request error: {_request.error}",
                        _url,
                        _request.responseCode);
            }
        }
    }
}