using Core.Configs;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Features.Clicker
{
    public class ClickerClickVfx: MonoBehaviour
    {
        [SerializeField] private RectTransform m_buttonTransform;
        [SerializeField] private Transform m_particleSpawnPoint;
        [SerializeField] private ParticleSystem m_clickParticles;
        [SerializeField] private AudioSource m_audioSource;
        [SerializeField] private AudioClip m_clickSound;

        private CurrencyFlyView.Pool m_currencyFlyPool;
        private ClickerConfig m_config;

        [Inject]
        public void Construct(
            CurrencyFlyView.Pool _currencyFlyPool,
            ClickerConfig _config)
        {
            m_currencyFlyPool = _currencyFlyPool;
            m_config = _config;
        }

        public void Play()
        {
            PlayButtonPunch();
            PlayParticles();
            PlaySound();
            PlayCurrencyFlyAsync().Forget();
        }

        private void PlayButtonPunch()
        {
            if (m_buttonTransform == null)
                return;

            m_buttonTransform.DOKill();
            m_buttonTransform.localScale = Vector3.one;
            m_buttonTransform
                .DOPunchScale(Vector3.one * 0.12f, 0.2f, 8, 0.8f);
        }

        private void PlayParticles()
        {
            if (m_clickParticles == null)
                return;

            if (m_particleSpawnPoint != null)
            {
                m_clickParticles.transform.position = m_particleSpawnPoint.position;
            }

            m_clickParticles.Play();
        }

        private void PlaySound()
        {
            if (m_audioSource == null || m_clickSound == null)
                return;

            m_audioSource.PlayOneShot(m_clickSound);
        }

        private async UniTaskVoid PlayCurrencyFlyAsync()
        {
            CurrencyFlyView flyView = m_currencyFlyPool.Spawn();

            flyView.transform.SetParent(transform, false);
            flyView.SetText($"+{m_config.currencyPerClick}");

            await flyView.PlayAsync();

            flyView.Stop();
            m_currencyFlyPool.Despawn(flyView);
        }
    }
}