using System;
using Cysharp.Threading.Tasks;
using UniRx;
using Zenject;

namespace Features.Popup
{
    public class PopupService : IPopupService, IInitializable, IDisposable
    {
        private readonly PopupView m_popupView;
        private readonly CompositeDisposable m_disposables = new();

        public PopupService(PopupView _popupView)
        {
            m_popupView = _popupView;
        }

        public void Initialize()
        {
            m_popupView.closeClicked
                .Subscribe(_ => Hide())
                .AddTo(m_disposables);

            m_popupView.HideImmediate();
        }

        public void ShowInfo(string _title, string _description)
        {
            ShowInfoAsync(_title, _description).Forget();
        }

        public void Hide()
        {
            HideAsync().Forget();
        }

        public void Dispose()
        {
            m_disposables.Dispose();
            m_popupView.HideImmediate();
        }

        private async UniTaskVoid ShowInfoAsync(string _title, string _description)
        {
            await m_popupView.ShowAsync(_title, _description);
        }

        private async UniTaskVoid HideAsync()
        {
            await m_popupView.HideAsync();
        }
    }
}