using System;
using System.Collections.Generic;
using System.Threading;
using Core.Http;
using Core.RequestQueue;
using Cysharp.Threading.Tasks;
using Features.Dogs.Models;
using Features.Dogs.Requests;
using Features.Dogs.Services;
using Features.Dogs.Views;
using Features.Navigation;
using Features.Navigation.Presenters;
using Features.Popup;
using UniRx;
using UnityEngine;
using Zenject;

namespace Features.Dogs.Presenters
{
    public class DogsTabPresenter: TabPresenterBase, IInitializable, IDisposable
    {
        public override TabType tabType => TabType.DOGS;
        
        private readonly DogsTabView m_tabView;
        private readonly DogsApiService m_dogsApiService;
        private readonly IRequestQueue m_requestQueue;
        private readonly IPopupService m_popupService;

        private readonly CompositeDisposable m_disposables = new();
        private readonly CompositeDisposable m_itemDisposables = new();

        private CancellationTokenSource m_tabLifetimeCts;

        private string m_selectedBreedId;
        private bool m_isShown;
        
        public DogsTabPresenter(DogsTabView _view,
            DogsApiService _dogsApiService,
            IRequestQueue _requestQueue,
            IPopupService _popupService) : base(_view)
        {
            m_tabView = _view;
            m_dogsApiService = _dogsApiService;
            m_requestQueue = _requestQueue;
            m_popupService = _popupService;
        }
        
        public void Initialize()
        {
            m_tabView.Hide();
            m_tabView.Clear();
        }
        
        public override void Show()
        {
            if (m_isShown)
                return;

            m_isShown = true;

            Debug.Log("[DogsPresenter] Show");

            base.Show();

            StartTabLifetime();

            LoadBreedsAsync(m_tabLifetimeCts.Token).Forget();
        }

        public override void Hide()
        {
            if (!m_isShown)
                return;

            m_isShown = false;

            Debug.Log("[DogsPresenter] Hide");

            StopTabLifetime();

            m_requestQueue.CancelByOwner(this);

            m_selectedBreedId = null;

            m_itemDisposables.Clear();

            m_tabView.Clear();
            base.Hide();
        }
        
        public void Dispose()
        {
            StopTabLifetime();

            m_requestQueue.CancelByOwner(this);

            m_itemDisposables.Dispose();
            m_disposables.Dispose();
        }
        
        private void StartTabLifetime()
        {
            StopTabLifetime();
            m_tabLifetimeCts = new CancellationTokenSource();
        }
        
        private void StopTabLifetime()
        {
            if (m_tabLifetimeCts == null)
                return;

            m_tabLifetimeCts.Cancel();
            m_tabLifetimeCts.Dispose();
            m_tabLifetimeCts = null;
        }
        
        private async UniTaskVoid LoadBreedsAsync(CancellationToken _cancellationToken)
        {
            try
            {
                m_tabView.ShowLoading();

                var request = new DogBreedsRequest(
                    _owner: this,
                    _dogsApiService: m_dogsApiService);

                RequestHandle<IReadOnlyList<DogBreedViewData>> handle =
                    m_requestQueue.Enqueue(request);

                IReadOnlyList<DogBreedViewData> breeds = await handle.task;

                if (_cancellationToken.IsCancellationRequested)
                    return;

                m_tabView.SetBreeds(breeds);

                BindBreedItems();

                Debug.Log($"[DogsPresenter] Breeds loaded: {breeds.Count}");
            }
            catch (OperationCanceledException)
            {
                Debug.Log("[DogsPresenter] Breeds request canceled");
            }
            catch (HttpRequestFailedException exception)
            {
                Debug.LogError($"[DogsPresenter] HTTP error: {exception.Message}");

                if (!_cancellationToken.IsCancellationRequested)
                    m_tabView.ShowError("Не удалось загрузить породы собак");
            }
            catch (Exception exception)
            {
                Debug.LogError($"[DogsPresenter] Unexpected error: {exception}");

                if (!_cancellationToken.IsCancellationRequested)
                    m_tabView.ShowError("Ошибка обработки списка пород");
            }
        }
        
        private void BindBreedItems()
        {
            m_itemDisposables.Clear();

            foreach (DogBreedItemView item in m_tabView.spawnedItems)
            {
                item.clicked
                    .Subscribe(OnBreedClicked)
                    .AddTo(m_itemDisposables);
            }
        }
        
        private void OnBreedClicked(string _breedId)
        {
            if (string.IsNullOrWhiteSpace(_breedId))
                return;

            Debug.Log($"[DogsPresenter] Breed clicked: {_breedId}");

            CancelCurrentDetailsRequest();

            m_selectedBreedId = _breedId;

            m_tabView.HideAllItemLoaders();
            m_tabView.ShowItemLoader(_breedId);

            LoadBreedDetailsAsync(
                _breedId,
                m_tabLifetimeCts.Token).Forget();
        }
        
        private void CancelCurrentDetailsRequest()
        {
            m_requestQueue.CancelByOwnerAndTag(
                this,
                DogRequestTags.BREED_DETAILS);

            if (!string.IsNullOrWhiteSpace(m_selectedBreedId))
            {
                m_tabView.HideItemLoader(m_selectedBreedId);
            }
        }
        
        private async UniTaskVoid LoadBreedDetailsAsync(
            string _breedId,
            CancellationToken _cancellationToken)
        {
            try
            {
                var request = new DogBreedDetailsRequest(
                    _owner: this,
                    _dogsApiService: m_dogsApiService,
                    _breedId: _breedId);

                RequestHandle<DogBreedDetailsViewData> handle =
                    m_requestQueue.Enqueue(request);

                DogBreedDetailsViewData details = await handle.task;

                if (_cancellationToken.IsCancellationRequested)
                    return;

                if (m_selectedBreedId != _breedId)
                    return;

                m_tabView.HideItemLoader(_breedId);

                m_popupService.ShowInfo(
                    details.m_name,
                    details.m_description);

                Debug.Log($"[DogsPresenter] Breed details loaded: {details.m_name}");
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"[DogsPresenter] Details request canceled: {_breedId}");

                if (!_cancellationToken.IsCancellationRequested)
                    m_tabView.HideItemLoader(_breedId);
            }
            catch (HttpRequestFailedException exception)
            {
                Debug.LogError($"[DogsPresenter] HTTP error: {exception.Message}");

                if (!_cancellationToken.IsCancellationRequested)
                {
                    m_tabView.HideItemLoader(_breedId);
                    m_tabView.ShowError("Не удалось загрузить описание породы");
                }
            }
            catch (Exception exception)
            {
                Debug.LogError($"[DogsPresenter] Unexpected details error: {exception}");

                if (!_cancellationToken.IsCancellationRequested)
                {
                    m_tabView.HideItemLoader(_breedId);
                    m_tabView.ShowError("Ошибка обработки описания породы");
                }
            }
        }
    }
}