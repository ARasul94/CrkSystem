using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core.RequestQueue
{
    public interface IRequestCommand<T>
    {
        string tag { get; }
        object owner { get; }

        UniTask<T> ExecuteAsync(CancellationToken _cancellationToken);
    }
}