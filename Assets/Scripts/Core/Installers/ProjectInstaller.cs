using Core.Http;
using Core.RequestQueue;
using DefaultNamespace;
using UnityEngine;
using Zenject;

namespace Core.Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        [Header("Configs")]
        [SerializeField] private ApiConfig m_apiConfig;
        
        public override void InstallBindings()
        {
            Debug.Log("[ProjectInstaller] InstallBindings");

            BindConfigs();
            BindRequestQueue();
            BindHttpClient();
        }
        
        private void BindConfigs()
        {
            if (m_apiConfig == null)
            {
                throw new MissingReferenceException(
                    $"{nameof(ApiConfig)} is not assigned in {nameof(ProjectInstaller)}");
            }

            Container
                .Bind<ApiConfig>()
                .FromInstance(m_apiConfig)
                .AsSingle();
        }

        private void BindRequestQueue()
        {
            Container
                .Bind<IRequestQueue>()
                .To<RequestQueueService>()
                .AsSingle();
        }
        
        private void BindHttpClient()
        {
            Container
                .Bind<IHttpClient>()
                .To<RestClient>()
                .AsSingle();
        }
    }
}