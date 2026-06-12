using System.Threading;
using Core.RequestQueue;
using Cysharp.Threading.Tasks;
using Features.Dogs.Models;
using Features.Dogs.Services;

namespace Features.Dogs.Requests
{
    public class DogBreedDetailsRequest : IRequestCommand<DogBreedDetailsViewData>
    {
        private readonly DogsApiService m_dogsApiService;
        private readonly string m_breedId;

        public object owner { get; }
        public string tag => DogRequestTags.BREED_DETAILS;

        public DogBreedDetailsRequest(
            object _owner,
            DogsApiService _dogsApiService,
            string _breedId)
        {
            owner = _owner;
            m_dogsApiService = _dogsApiService;
            m_breedId = _breedId;
        }

        public UniTask<DogBreedDetailsViewData> ExecuteAsync(
            CancellationToken _cancellationToken)
        {
            return m_dogsApiService.GetBreedDetailsAsync(
                m_breedId,
                _cancellationToken);
        }
    }
}