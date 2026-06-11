using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core.Http
{
    public interface IHttpClient
    {
        UniTask<string> GetAsync(string _url, CancellationToken _cancellationToken);
    }
}