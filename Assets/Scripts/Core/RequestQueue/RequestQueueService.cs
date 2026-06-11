using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.RequestQueue
{
    public class RequestQueueService : IRequestQueue
    {
        private readonly Queue<IQueuedRequest> m_pendingRequests = new();

        private IQueuedRequest m_activeRequest;
        private CancellationTokenSource m_activeRequestCts;

        private bool m_isProcessing;
        
        public RequestHandle<T> Enqueue<T>(IRequestCommand<T> _command)
        {
            var handle = new RequestHandle<T>(_command.owner, _command.tag);
            var queuedRequest = new QueuedRequest<T>(_command, handle);

            m_pendingRequests.Enqueue(queuedRequest);

            Debug.Log($"[RequestQueue] Enqueued. Tag: {_command.tag}, Owner: {_command.owner}");

            ProcessQueueAsync().Forget();

            return handle;
        }

        public void CancelByOwner(object _owner)
        {
            CancelPending(_request => ReferenceEquals(_request.owner, _owner));
            CancelActive(_request => ReferenceEquals(_request.owner, _owner));
        }

        public void CancelByTag(string _tag)
        {
            CancelPending(_request => _request.tag == _tag);
            CancelActive(_request => _request.tag == _tag);
        }

        public void CancelByOwnerAndTag(object _owner, string _tag)
        {
            CancelPending(_request =>
                ReferenceEquals(_request.owner, _owner) &&
                _request.tag == _tag);

            CancelActive(_request =>
                ReferenceEquals(_request.owner, _owner) &&
                _request.tag == _tag);
        }
        
        private async UniTaskVoid ProcessQueueAsync()
        {
            if (m_isProcessing)
                return;

            m_isProcessing = true;

            try
            {
                while (m_pendingRequests.Count > 0)
                {
                    m_activeRequest = m_pendingRequests.Dequeue();
                    m_activeRequestCts = new CancellationTokenSource();

                    Debug.Log($"[RequestQueue] Started. Tag: {m_activeRequest.tag}, Owner: {m_activeRequest.owner}");

                    await m_activeRequest.ExecuteAsync(m_activeRequestCts.Token);

                    Debug.Log($"[RequestQueue] Finished. Tag: {m_activeRequest.tag}, Owner: {m_activeRequest.owner}");

                    m_activeRequestCts.Dispose();
                    m_activeRequestCts = null;
                    m_activeRequest = null;
                }
            }
            finally
            {
                m_isProcessing = false;
            }
        }
        
        private void CancelPending(System.Predicate<IQueuedRequest> _predicate)
        {
            if (m_pendingRequests.Count == 0)
                return;

            List<IQueuedRequest> remainingRequests = new();

            while (m_pendingRequests.Count > 0)
            {
                IQueuedRequest request = m_pendingRequests.Dequeue();

                if (_predicate(request))
                {
                    Debug.Log($"[RequestQueue] Canceled pending. Tag: {request.tag}, Owner: {request.owner}");
                    request.Cancel();
                }
                else
                {
                    remainingRequests.Add(request);
                }
            }

            foreach (IQueuedRequest request in remainingRequests)
            {
                m_pendingRequests.Enqueue(request);
            }
        }
        
        private void CancelActive(System.Predicate<IQueuedRequest> _predicate)
        {
            if (m_activeRequest == null)
                return;

            if (!_predicate(m_activeRequest))
                return;

            Debug.Log($"[RequestQueue] Cancel active. Tag: {m_activeRequest.tag}, Owner: {m_activeRequest.owner}");

            m_activeRequestCts?.Cancel();
        }
    }
}