using Cysharp.Threading.Tasks;

namespace Core.RequestQueue
{
    public class RequestHandle<T>
    {
        private readonly UniTaskCompletionSource<T> m_completionSource = new();

        public object owner { get; }
        public string tag { get; }

        public UniTask<T> task => m_completionSource.Task;
        
        public RequestHandle(object _owner, string _tag)
        {
            owner = _owner;
            tag = _tag;
        }
        
        internal void SetResult(T _result)
        {
            m_completionSource.TrySetResult(_result);
        }

        internal void SetException(System.Exception _exception)
        {
            m_completionSource.TrySetException(_exception);
        }

        internal void SetCanceled()
        {
            m_completionSource.TrySetCanceled();
        }
    }
}