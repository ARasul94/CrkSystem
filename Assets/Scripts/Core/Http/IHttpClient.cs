using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Http
{
    public interface IHttpClient
    {
        UniTask<string> GetAsync(string _url, CancellationToken _cancellationToken);
        UniTask<Sprite> LoadSpriteAsync(string _url, CancellationToken _cancellationToken);
    }
}