using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core.RequestQueue
{
    public class QueuedRequest<T> : IQueuedRequest
    {
        public object owner => m_command.owner;
        public string tag => m_command.tag;
        
        private readonly IRequestCommand<T> m_command;
        private readonly RequestHandle<T> m_handle;
        
        public QueuedRequest(IRequestCommand<T> _command, RequestHandle<T> _handle)
        {
            m_command = _command;
            m_handle = _handle;
        }
        
        public async UniTask ExecuteAsync(CancellationToken _cancellationToken)
        {
            try
            {
                T result = await m_command.ExecuteAsync(_cancellationToken);
                m_handle.SetResult(result);
            }
            catch (OperationCanceledException)
            {
                m_handle.SetCanceled();
            }
            catch (Exception exception)
            {
                m_handle.SetException(exception);
            }
        }

        public void Cancel()
        {
            m_handle.SetCanceled();
        }
    }
}