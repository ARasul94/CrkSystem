using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Zenject;

namespace Features.Clicker
{
    public class CurrencyFlyView : MonoBehaviour
    {
        [SerializeField] private RectTransform m_rectTransform;
        [SerializeField] private CanvasGroup m_canvasGroup;
        [SerializeField] private TextMeshProUGUI m_label;

        private Sequence m_sequence;

        public void SetText(string _text)
        {
            m_label.text = _text;
        }

        public async UniTask PlayAsync()
        {
            m_sequence?.Kill();

            m_canvasGroup.alpha = 1f;
            m_rectTransform.anchoredPosition = Vector2.zero;
            m_rectTransform.localScale = Vector3.one;

            m_sequence = DOTween.Sequence();
            m_sequence.Join(m_rectTransform.DOAnchorPosY(120f, 0.6f));
            m_sequence.Join(m_canvasGroup.DOFade(0f, 0.6f));
            m_sequence.Join(m_rectTransform.DOScale(1.25f, 0.3f));

            await m_sequence.AsyncWaitForCompletion().AsUniTask();
        }

        public void Stop()
        {
            m_sequence?.Kill();
            m_sequence = null;
        }

#if UNITY_EDITOR
        private void Reset()
        {
            m_rectTransform = GetComponent<RectTransform>();
            m_canvasGroup = GetComponent<CanvasGroup>();
            m_label = GetComponentInChildren<TextMeshProUGUI>();
        }
#endif
        
        public sealed class Pool : MemoryPool<CurrencyFlyView>
        {
            protected override void OnDespawned(CurrencyFlyView item)
            {
                item.Stop();
                item.gameObject.SetActive(false);
            }

            protected override void OnSpawned(CurrencyFlyView item)
            {
                item.gameObject.SetActive(true);
            }
        }
    }
}