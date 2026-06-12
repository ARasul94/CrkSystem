using System.Collections.Generic;
using System.Threading;
using Core.RequestQueue;
using Cysharp.Threading.Tasks;
using Features.Dogs.Models;
using Features.Dogs.Services;

namespace Features.Dogs.Requests
{
    public class DogBreedsRequest : IRequestCommand<IReadOnlyList<DogBreedViewData>>
    {
        private readonly DogsApiService m_dogsApiService;

        public object owner { get; }
        public string tag => DogRequestTags.BREEDS;

        public DogBreedsRequest(
            object _owner,
            DogsApiService _dogsApiService)
        {
            owner = _owner;
            m_dogsApiService = _dogsApiService;
        }

        public UniTask<IReadOnlyList<DogBreedViewData>> ExecuteAsync(
            CancellationToken _cancellationToken)
        {
            return m_dogsApiService.GetBreedsAsync(_cancellationToken);
        }
    }
}