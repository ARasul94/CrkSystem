using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Features.Loading
{
    public class LoadingView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_label;
        [SerializeField] private string m_baseText = "Loading";
        [SerializeField] private float m_intervalSeconds = 0.35f;

        private CancellationTokenSource m_cts;

        private void OnEnable()
        {
            StartAnimation();
        }

        private void OnDisable()
        {
            StopAnimation();
        }

        private void StartAnimation()
        {
            StopAnimation();

            m_cts = new CancellationTokenSource();
            AnimateAsync(m_cts.Token).Forget();
        }

        private void StopAnimation()
        {
            if (m_cts == null)
                return;

            m_cts.Cancel();
            m_cts.Dispose();
            m_cts = null;
        }

        private async UniTaskVoid AnimateAsync(CancellationToken _cancellationToken)
        {
            try
            {
                var dotsCount = 1;
                var interval =  TimeSpan.FromSeconds(m_intervalSeconds);

                while (!_cancellationToken.IsCancellationRequested)
                {
                    m_label.text = $"{m_baseText}{new string('.', dotsCount)}";

                    dotsCount++;

                    if (dotsCount > 3)
                        dotsCount = 1;

                    await UniTask.Delay(
                        interval,
                        cancellationToken: _cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}