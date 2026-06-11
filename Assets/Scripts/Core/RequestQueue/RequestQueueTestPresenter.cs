using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Core.RequestQueue
{
    public class RequestQueueTestPresenter : IInitializable
    {
        private readonly IRequestQueue m_requestQueue;

        private readonly object m_ownerA = new();
        private readonly object m_ownerB = new();

        public RequestQueueTestPresenter(IRequestQueue _requestQueue)
        {
            m_requestQueue = _requestQueue;
        }
        
        public void Initialize()
        {
            RunTestAsync().Forget();
        }
        
        private async UniTaskVoid RunTestAsync()
        {
            Debug.Log("[RequestQueueTest] Test started");

            RequestHandle<string> requestA1 = m_requestQueue.Enqueue(
                new FakeRequestCommand(m_ownerA, "test_a_1", "A1", 2f));

            RequestHandle<string> requestA2 = m_requestQueue.Enqueue(
                new FakeRequestCommand(m_ownerA, "test_a_2", "A2", 2f));

            RequestHandle<string> requestB1 = m_requestQueue.Enqueue(
                new FakeRequestCommand(m_ownerB, "test_b_1", "B1", 1f));

            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

            Debug.Log("[RequestQueueTest] Cancel owner A");
            m_requestQueue.CancelByOwner(m_ownerA);

            await AwaitHandle("A1", requestA1);
            await AwaitHandle("A2", requestA2);
            await AwaitHandle("B1", requestB1);

            Debug.Log("[RequestQueueTest] Test finished");
        }
        
        private async UniTask AwaitHandle(string _name, RequestHandle<string> _handle)
        {
            try
            {
                string result = await _handle.task;
                Debug.Log($"[RequestQueueTest] {_name} result: {result}");
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"[RequestQueueTest] {_name} canceled");
            }
            catch (Exception exception)
            {
                Debug.LogError($"[RequestQueueTest] {_name} failed: {exception}");
            }
        }
    }
}