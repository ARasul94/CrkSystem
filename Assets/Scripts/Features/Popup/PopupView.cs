using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Popup
{
    public class PopupView: MonoBehaviour
    {
        [Header("Root")]
        [SerializeField] private GameObject m_root;
        [SerializeField] private CanvasGroup m_canvasGroup;

        [Header("Window")]
        [SerializeField] private RectTransform m_window;

        [Header("Content")]
        [SerializeField] private TMP_Text m_titleText;
        [SerializeField] private TMP_Text m_descriptionText;

        [Header("Controls")]
        [SerializeField] private Button m_okButton;
        [SerializeField] private Button m_backgroundCloseButton;

        [Header("Settings")]
        [SerializeField] private float m_openDuration = 0.2f;
        [SerializeField] private float m_closeDuration = 0.15f;
        
        public IObservable<Unit> closeClicked => m_closeClicked;
        
        private GameObject root => m_root != null ? m_root : gameObject;

        private readonly Subject<Unit> m_closeClicked = new();

        private Sequence m_animation;

        private bool m_isDestroyed;

        private void Awake()
        {
            m_okButton.OnClickAsObservable()
                .Subscribe(_ => m_closeClicked.OnNext(Unit.Default))
                .AddTo(this);

            if (m_backgroundCloseButton != null)
            {
                m_backgroundCloseButton.OnClickAsObservable()
                    .Subscribe(_ => m_closeClicked.OnNext(Unit.Default))
                    .AddTo(this);
            }

            HideImmediate();
        }

        private void OnDestroy()
        {
            m_isDestroyed = true;
        }

        public async UniTask ShowAsync(string _title, string _description)
        {
            m_animation?.Kill();

            root.SetActive(true);

            m_titleText.text = _title;
            m_descriptionText.text = _description;

            await UniTask.Yield();

            m_canvasGroup.alpha = 0f;
            m_canvasGroup.interactable = false;
            m_canvasGroup.blocksRaycasts = true;

            m_window.localScale = Vector3.one * 0.9f;

            m_animation = DOTween.Sequence();
            m_animation.Join(m_canvasGroup.DOFade(1f, m_openDuration));
            m_animation.Join(m_window.DOScale(1f, m_openDuration).SetEase(Ease.OutBack));

            await m_animation.AsyncWaitForCompletion().AsUniTask();

            m_canvasGroup.interactable = true;
        }

        public async UniTask HideAsync()
        {
            if (!root.activeSelf)
                return;

            m_animation?.Kill();

            m_canvasGroup.interactable = false;
            m_canvasGroup.blocksRaycasts = false;

            m_animation = DOTween.Sequence();
            m_animation.Join(m_canvasGroup.DOFade(0f, m_closeDuration));
            m_animation.Join(m_window.DOScale(0.9f, m_closeDuration).SetEase(Ease.InBack));

            await m_animation.AsyncWaitForCompletion().AsUniTask();

            HideImmediate();
        }

        public void HideImmediate()
        {
            if (m_isDestroyed)
                return;
            
            m_animation?.Kill();
            m_animation = null;

            m_canvasGroup.alpha = 0f;
            m_canvasGroup.interactable = false;
            m_canvasGroup.blocksRaycasts = false;

            if (m_window != null)
                m_window.localScale = Vector3.one;

            root.SetActive(false);
        }

#if UNITY_EDITOR
        private void Reset()
        {
            m_root = gameObject;
            m_canvasGroup = GetComponent<CanvasGroup>();
            m_window = GetComponentInChildren<RectTransform>();
        }
#endif
    }
}