using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Features.Dogs.Views
{
    public class DogBreedItemView : MonoBehaviour
    {
        [SerializeField] private Button m_button;
        [SerializeField] private TextMeshProUGUI m_nameText;
        [SerializeField] private GameObject m_loaderRoot;

        private string m_breedId;
        private readonly Subject<string> m_clicked = new();

        public IObservable<string> clicked => m_clicked;

        public string breedId => m_breedId;

        private void Awake()
        {
            m_button.OnClickAsObservable()
                .Subscribe(_ => m_clicked.OnNext(m_breedId))
                .AddTo(this);
        }

        public void SetData(int _index, string _breedId, string _breedName)
        {
            m_breedId = _breedId;
            m_nameText.text = $"{_index} - {_breedName}";
            HideLoader();
            SetInteractable(true);
        }

        public void ShowLoader()
        {
            m_loaderRoot.SetActive(true);
            SetInteractable(false);
        }

        public void HideLoader()
        {
            m_loaderRoot.SetActive(false);
            SetInteractable(true);
        }

        public void Clear()
        {
            m_breedId = null;
            m_nameText.text = string.Empty;
            HideLoader();
        }

        public void SetInteractable(bool _isInteractable)
        {
            m_button.interactable = _isInteractable;
        }

        public sealed class Pool : MemoryPool<DogBreedItemView>
        {
            protected override void OnSpawned(DogBreedItemView _item)
            {
                _item.gameObject.SetActive(true);
            }

            protected override void OnDespawned(DogBreedItemView _item)
            {
                _item.Clear();
                _item.gameObject.SetActive(false);
            }
        }
    }
}