using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.RequestQueue
{
    public class FakeRequestCommand : IRequestCommand<string>
    {
        private readonly string m_name;
        private readonly float m_durationSeconds;

        public object owner { get; }
        public string tag { get; }

        public FakeRequestCommand(
            object _owner,
            string _tag,
            string _name,
            float _durationSeconds)
        {
            owner = _owner;
            tag = _tag;
            m_name = _name;
            m_durationSeconds = _durationSeconds;
        }

        public async UniTask<string> ExecuteAsync(CancellationToken _cancellationToken)
        {
            Debug.Log($"[FakeRequest] Execute started: {m_name}");

            await UniTask.Delay(
                TimeSpan.FromSeconds(m_durationSeconds),
                cancellationToken: _cancellationToken);

            Debug.Log($"[FakeRequest] Execute completed: {m_name}");

            return $"Result from {m_name}";
        }
    }
}