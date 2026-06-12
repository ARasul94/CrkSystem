using System;
using System.Threading;
using Core.Configs;
using Cysharp.Threading.Tasks;
using Features.Navigation;
using Features.Navigation.Presenters;
using Features.Navigation.Tabs;
using UniRx;
using UnityEngine;
using Zenject;

namespace Features.Clicker
{
    public class ClickerTabPresenter: TabPresenterBase, IInitializable, IDisposable
    {
        public override TabType tabType => TabType.CLICKER;
        
        private readonly ClickerTabView m_clickerTabView;
        private readonly ClickerModel m_model;
        private readonly ClickerService m_service;
        private readonly ClickerConfig m_config;

        private readonly CompositeDisposable m_disposables = new();

        private CancellationTokenSource m_lifetimeCts;
        private bool m_isShown;
        
        public ClickerTabPresenter(ClickerTabView _view,
            ClickerModel _model,
            ClickerService _service,
            ClickerConfig _config) : base(_view)
        {
            m_clickerTabView = _view;
            m_model = _model;
            m_service = _service;
            m_config = _config;
        }
        
        public override void Show()
        {
            if (m_isShown)
                return;

            m_isShown = true;

            Debug.Log("[ClickerPresenter] Show");

            base.Show();
            m_clickerTabView.gameObject.SetActive(true);

            StartLoops();
        }

        public override void Hide()
        {
            if (!m_isShown)
                return;

            m_isShown = false;

            Debug.Log("[ClickerPresenter] Hide");

            StopLoops();

            base.Hide();
        }
        
        public void Dispose()
        {
            StopLoops();

            m_disposables.Dispose();
            m_model.Dispose();
        }

        public void Initialize()
        {
            BindView();
            BindModel();

            m_clickerTabView.SetCurrency(m_model.currentCurrency);
            m_clickerTabView.SetEnergy(m_model.currentEnergy);
            m_clickerTabView.SetClickAvailable(CanClick());
        }
        
        private void BindView()
        {
            m_clickerTabView.clicked
                .Subscribe(_ => TryClick())
                .AddTo(m_disposables);
        }

        private void BindModel()
        {
            m_model.currency
                .Subscribe(m_clickerTabView.SetCurrency)
                .AddTo(m_disposables);

            m_model.energy
                .Subscribe(_energy =>
                {
                    m_clickerTabView.SetEnergy(_energy);
                    m_clickerTabView.SetClickAvailable(CanClick());
                })
                .AddTo(m_disposables);
        }
        
        private void TryClick()
        {
            bool success = m_service.TryClick();

            if (!success)
                return;

            m_clickerTabView.PlayClickFeedback();
        }

        private bool CanClick()
        {
            return m_model.currentEnergy >= m_config.energyCostPerClick;
        }
        
        private void StartLoops()
        {
            StopLoops();

            m_lifetimeCts = new CancellationTokenSource();

            AutoClickLoopAsync(m_lifetimeCts.Token).Forget();
            EnergyRestoreLoopAsync(m_lifetimeCts.Token).Forget();
        }

        private void StopLoops()
        {
            if (m_lifetimeCts == null)
                return;

            m_lifetimeCts.Cancel();
            m_lifetimeCts.Dispose();
            m_lifetimeCts = null;
        }
        
        private async UniTaskVoid AutoClickLoopAsync(CancellationToken _cancellationToken)
        {
            var interval = TimeSpan.FromSeconds(m_config.autoClickIntervalSeconds);
            while (!_cancellationToken.IsCancellationRequested)
            {
                await UniTask.Delay(
                    interval,
                    cancellationToken: _cancellationToken);

                if (_cancellationToken.IsCancellationRequested)
                    return;

                TryClick();
            }
        }

        private async UniTaskVoid EnergyRestoreLoopAsync(CancellationToken _cancellationToken)
        {
            var interval =  TimeSpan.FromSeconds(m_config.energyRestoreIntervalSeconds);
            while (!_cancellationToken.IsCancellationRequested)
            {
                await UniTask.Delay(
                    interval,
                    cancellationToken: _cancellationToken);

                if (_cancellationToken.IsCancellationRequested)
                    return;

                m_service.RestoreEnergy();
            }
        }
    }
}