using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core.RequestQueue
{
    public interface IQueuedRequest
    {
        object owner { get; }
        string tag { get; }

        UniTask ExecuteAsync(CancellationToken _cancellationToken);
        void Cancel();
    }
}