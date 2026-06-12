using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Core.Http;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using Features.Dogs.Models;
using UnityEngine;

namespace Features.Dogs.Services
{
    public class DogsApiService
    {
        private const int DEFAULT_BREEDS_LIMIT = 10;

        private readonly IHttpClient m_httpClient;
        private readonly ApiConfig m_apiConfig;

        public DogsApiService(
            IHttpClient _httpClient,
            ApiConfig _apiConfig)
        {
            m_httpClient = _httpClient;
            m_apiConfig = _apiConfig;
        }

        public async UniTask<IReadOnlyList<DogBreedViewData>> GetBreedsAsync(
            CancellationToken _cancellationToken)
        {
            string json = await m_httpClient.GetAsync(
                m_apiConfig.dogBreedsUrl,
                _cancellationToken);

            DogBreedsResponse response =
                JsonUtility.FromJson<DogBreedsResponse>(json);

            if (response?.data == null)
                throw new InvalidOperationException("Dog breeds response does not contain data.");

            return response.data
                .Where(_item => _item != null && _item.attributes != null)
                .Take(DEFAULT_BREEDS_LIMIT)
                .Select(_item => new DogBreedViewData(
                    _item.id,
                    _item.attributes.name))
                .ToArray();
        }

        public async UniTask<DogBreedDetailsViewData> GetBreedDetailsAsync(
            string _breedId,
            CancellationToken _cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_breedId))
                throw new ArgumentException("Breed id is null or empty.", nameof(_breedId));

            string url = m_apiConfig.GetDogBreedDetailsUrl(_breedId);

            string json = await m_httpClient.GetAsync(url, _cancellationToken);

            DogBreedDetailsResponse response =
                 JsonUtility.FromJson<DogBreedDetailsResponse>(json);

            if (response?.data == null || response.data.attributes == null)
                throw new InvalidOperationException("Dog breed details response does not contain data.");

            return new DogBreedDetailsViewData(
                response.data.id,
                response.data.attributes.name,
                response.data.attributes.description);
        }
    }
}