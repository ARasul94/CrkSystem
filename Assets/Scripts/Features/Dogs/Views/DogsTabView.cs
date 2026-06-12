using System.Collections.Generic;
using Features.Dogs.Models;
using Features.Navigation.Tabs;
using TMPro;
using UnityEngine;
using Zenject;

namespace Features.Dogs.Views
{
    public class DogsTabView: TabView
    {
        [Header("States")]
        [SerializeField] private GameObject m_loadingRoot;
        [SerializeField] private GameObject m_contentRoot;
        [SerializeField] private GameObject m_errorRoot;

        [Header("Content")]
        [SerializeField] private Transform m_itemsRoot;
        [SerializeField] private TextMeshProUGUI m_errorText;

        private readonly List<DogBreedItemView> m_spawnedItems = new();

        private DogBreedItemView.Pool m_itemPool;

        public IReadOnlyList<DogBreedItemView> spawnedItems => m_spawnedItems;

        [Inject]
        public void Construct(DogBreedItemView.Pool _itemPool)
        {
            m_itemPool = _itemPool;
        }

        public void ShowLoading()
        {
            SetState(_isLoading: true, _hasContent: false, _hasError: false);
        }

        public void ShowContent()
        {
            SetState(_isLoading: false, _hasContent: true, _hasError: false);
        }

        public void ShowError(string _message)
        {
            m_errorText.text = _message;
            SetState(_isLoading: false, _hasContent: false, _hasError: true);
        }

        public void Clear()
        {
            HideAllItemLoaders();
            DespawnItems();

            m_errorText.text = string.Empty;

            SetState(_isLoading: false, _hasContent: false, _hasError: false);
        }

        public void SetBreeds(IReadOnlyList<DogBreedViewData> _breeds)
        {
            DespawnItems();

            for (int i = 0; i < _breeds.Count; i++)
            {
                DogBreedViewData breed = _breeds[i];

                DogBreedItemView item = m_itemPool.Spawn();
                item.transform.SetParent(m_itemsRoot, false);
                item.transform.SetSiblingIndex(i);
                item.SetData(i + 1, breed.m_id, breed.m_name);

                m_spawnedItems.Add(item);
            }

            ShowContent();
        }

        public void ShowItemLoader(string _breedId)
        {
            foreach (DogBreedItemView item in m_spawnedItems)
            {
                if (item.breedId == _breedId)
                    item.ShowLoader();
                else
                    item.HideLoader();
            }
        }

        public void HideItemLoader(string _breedId)
        {
            foreach (DogBreedItemView item in m_spawnedItems)
            {
                if (item.breedId == _breedId)
                    item.HideLoader();
            }
        }

        public void HideAllItemLoaders()
        {
            foreach (DogBreedItemView item in m_spawnedItems)
                item.HideLoader();
        }

        private void DespawnItems()
        {
            for (int i = 0; i < m_spawnedItems.Count; i++)
                m_itemPool.Despawn(m_spawnedItems[i]);

            m_spawnedItems.Clear();
        }

        private void SetState(bool _isLoading, bool _hasContent, bool _hasError)
        {
            m_loadingRoot.SetActive(_isLoading);
            m_contentRoot.SetActive(_hasContent);
            m_errorRoot.SetActive(_hasError);
        }
    }
}